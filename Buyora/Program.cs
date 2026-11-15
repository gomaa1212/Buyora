
using Buyora.DependencyInjection;
using BuyoraDataAccess.DbInitializer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalizationDependencyInjection();

builder.Services.AddGeneralRegisterDependencyInjection(builder.Configuration);

builder.Services.AddUnitOFWorkDependencyInjection();

builder.Services.AddServiceDependencyInjection();

builder.Services.AddIdentityDependencyInjection(builder.Configuration);

// Languages
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("ar-EG")
    };

    options.DefaultRequestCulture = new RequestCulture("ar-EG");

    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(
        0,
        new CookieRequestCultureProvider()
    );
});

builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddPolicy("chat", ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 300,
            Window = TimeSpan.FromMinutes(1)
        }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.Use(async (context, next) =>
{
    var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
    culture.NumberFormat = CultureInfo.InvariantCulture.NumberFormat;

    CultureInfo.CurrentCulture = culture;  
   
    await next();
});

app.UseRouting();

app.UseRateLimiter();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

await SeedDatabase();

app.MapControllerRoute(
    name:"MyArea",
    pattern:"{Area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new {area="Customer"})
    .WithStaticAssets();

app.Run();

async Task SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitilizer = scope.ServiceProvider.GetRequiredService<IDbInitilizer>();
        await dbInitilizer.InitilizeAsync();
    }
}