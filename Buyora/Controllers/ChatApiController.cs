using System.Security.Claims;
using System.Text.Json;
using BuyoraDataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Buyora.Controllers;

[ApiController]
[Route("api/chat")]
[EnableRateLimiting("chat")]
public sealed class ChatApiController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IDataProtector _customerTokenProtector;
    private readonly IConfiguration _configuration;

    public ChatApiController(
        AppDbContext db,
        IDataProtectionProvider dataProtection,
        IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
        _customerTokenProtector = dataProtection.CreateProtector("Buyora.Chat.CustomerToken.v1");
    }

    [AllowAnonymous]
    [HttpGet("products")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] string? sort = "relevance",
        [FromQuery] bool discountsOnly = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 8,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 20);
        var siteBaseUrl = (_configuration["App:BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}").TrimEnd('/');

        var products = _db.Product.AsNoTracking()
            .Where(p => p.IsActive && p.Category.IsActive);

        // بحث بأكتر من كلمة: كل كلمة لازم تكون موجودة في الاسم أو الوصف
        if (!string.IsNullOrWhiteSpace(search))
        {
            var words = search.Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Take(5);

            foreach (var w in words)
            {
                var word = w;
                products = products.Where(p =>
                    p.NameAr.Contains(word) || p.NameEn.Contains(word)
                    || (p.DescriptionAr != null && p.DescriptionAr.Contains(word))
                    || (p.DescriptionEn != null && p.DescriptionEn.Contains(word)));
            }
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var cat = category.Trim();
            products = products.Where(p => p.Category.NameAr.Contains(cat) || p.Category.NameEn.Contains(cat));
        }

        if (discountsOnly)
            products = products.Where(p => p.DiscountPrice.HasValue && p.DiscountPrice.Value > 0 && p.DiscountPrice.Value < p.Price);

        if (minPrice.HasValue && minPrice.Value > 0)
            products = products.Where(p =>
                (p.DiscountPrice.HasValue && p.DiscountPrice.Value > 0 && p.DiscountPrice.Value < p.Price
                    ? p.DiscountPrice.Value : p.Price) >= minPrice.Value);

        if (maxPrice.HasValue && maxPrice.Value > 0)
            products = products.Where(p =>
                (p.DiscountPrice.HasValue && p.DiscountPrice.Value > 0 && p.DiscountPrice.Value < p.Price
                    ? p.DiscountPrice.Value : p.Price) <= maxPrice.Value);

        products = sort?.Trim().ToLowerInvariant() switch
        {
            "price_asc" => products.OrderBy(p =>
                p.DiscountPrice.HasValue && p.DiscountPrice.Value > 0 && p.DiscountPrice.Value < p.Price
                    ? p.DiscountPrice.Value : p.Price),

            "price_desc" => products.OrderByDescending(p =>
                p.DiscountPrice.HasValue && p.DiscountPrice.Value > 0 && p.DiscountPrice.Value < p.Price
                    ? p.DiscountPrice.Value : p.Price),

            // أكبر خصم بالنسبة المئوية
            "discount" => products.OrderByDescending(p =>
                p.DiscountPrice.HasValue && p.DiscountPrice.Value > 0 && p.DiscountPrice.Value < p.Price && p.Price > 0
                    ? (p.Price - p.DiscountPrice.Value) / p.Price
                    : 0m),

            _ => products.OrderByDescending(p => p.Id)
        };

        var totalCount = await products.CountAsync(cancellationToken);

        var items = await products.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => new
            {
                p.Id,
                nameAr = p.NameAr,
                nameEn = p.NameEn,
                descriptionAr = p.DescriptionAr,
                descriptionEn = p.DescriptionEn,
                categoryAr = p.Category.NameAr,
                categoryEn = p.Category.NameEn,
                originalPrice = p.Price,
                finalPrice = p.DiscountPrice.HasValue && p.DiscountPrice.Value > 0 && p.DiscountPrice.Value < p.Price
                    ? p.DiscountPrice.Value : p.Price,
                discountPercent = p.DiscountPrice.HasValue && p.DiscountPrice.Value > 0 && p.DiscountPrice.Value < p.Price
                    ? Math.Round((p.Price - p.DiscountPrice.Value) * 100m / p.Price, 0)
                    : 0m,
                inStock = p.Quantity > 0,
                imageUrl = p.ProductImages.Where(i => i.IsMain).Select(i => i.Url).FirstOrDefault(),
                productUrl = $"{siteBaseUrl}/Customer/Product/Details/{p.Id}"
            }).ToListAsync(cancellationToken);

        return Ok(new { page, pageSize, totalCount, items });
    }

    // الموقع بيناديها بكوكي الـ Identity، وبعدين بيبعت التوكن القصير لـ n8n
    [Authorize]
    [IgnoreAntiforgeryToken]
    [HttpPost("session-token")]
    public IActionResult CreateCustomerSessionToken()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var payload = JsonSerializer.Serialize(
            new CustomerTokenPayload(userId, DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds()));

        return Ok(new { token = _customerTokenProtector.Protect(payload), expiresInSeconds = 600 });
    }

    [AllowAnonymous]
    [HttpGet("me/orders")]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken = default)
    {
        var userId = GetCustomerIdFromBearerToken();
        if (userId is null) return Unauthorized(new { message = "A valid customer session token is required." });

        var orders = await _db.Orders.AsNoTracking()
            .Where(o => o.ApplicationUserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Take(10)
            .Select(o => new
            {
                o.Id,
                o.OrderDate,
                o.Status,
                o.TotalAmount,
                o.ShippingDate,
                o.TrackingNumber,
                o.Carrier,
                items = o.OrderDetails.Select(d => new
                {
                    productNameAr = d.Product.NameAr,
                    productNameEn = d.Product.NameEn,
                    d.Count,
                    d.UnitPrice
                })
            }).ToListAsync(cancellationToken);

        return Ok(new { items = orders });
    }

    [AllowAnonymous]
    [HttpGet("me/orders/{id:int}")]
    public async Task<IActionResult> GetMyOrder(int id, CancellationToken cancellationToken = default)
    {
        var userId = GetCustomerIdFromBearerToken();
        if (userId is null) return Unauthorized(new { message = "A valid customer session token is required." });

        var order = await _db.Orders.AsNoTracking()
            .Where(o => o.Id == id && o.ApplicationUserId == userId)
            .Select(o => new
            {
                o.Id,
                o.OrderDate,
                o.Status,
                o.TotalAmount,
                o.ShippingDate,
                o.TrackingNumber,
                o.Carrier,
                items = o.OrderDetails.Select(d => new
                {
                    productNameAr = d.Product.NameAr,
                    productNameEn = d.Product.NameEn,
                    d.Count,
                    d.UnitPrice
                })
            }).FirstOrDefaultAsync(cancellationToken);

        return order is null ? NotFound() : Ok(order);
    }

    private string? GetCustomerIdFromBearerToken()
    {
        var authorization = Request.Headers["Authorization"].ToString();
        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return null;

        try
        {
            var payload = JsonSerializer.Deserialize<CustomerTokenPayload>(
                _customerTokenProtector.Unprotect(authorization[7..].Trim()));

            if (payload is null || payload.ExpiresAt <= DateTimeOffset.UtcNow.ToUnixTimeSeconds()) return null;
            return payload.UserId;
        }
        catch
        {
            return null;
        }
    }

    private sealed record CustomerTokenPayload(string UserId, long ExpiresAt);
}
