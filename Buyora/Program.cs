
using Buyora.DependencyInjection;
using BuyoraDataAccess.DbInitializer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Shared hosting may recycle the app pool and cannot persist keys in a Windows profile.
// Keep Identity cookies and the short-lived chat customer token valid across restarts.
if (!builder.Environment.IsDevelopment())
{
    var keysPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "private", "keys"));
    Directory.CreateDirectory(keysPath);
    builder.Services.AddDataProtection()
        .SetApplicationName("Buyora")
        .PersistKeysToFileSystem(new DirectoryInfo(keysPath));
}

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

// Product images are uploaded while the app is running, so they are not part
// of the static asset manifest generated at build time.
app.UseStaticFiles();

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
