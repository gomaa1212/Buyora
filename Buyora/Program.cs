
using Buyora.DependencyInjection;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalizationDependencyInjection();

builder.Services.AddGeneralRegisterDependencyInjection(builder.Configuration);

builder.Services.AddUnitOFWorkDependencyInjection();

builder.Services.AddServiceDependencyInjection();

builder.Services.AddIdentityDependencyInjection();

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

    CultureInfo.CurrentCulture = culture;   // تنسيق الأرقام
    // CurrentUICulture سيبها زي ما هي (الترجمة)

    await next();
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

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