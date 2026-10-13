
using Buyora.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalizationDependencyInjection();

builder.Services.AddGeneralRegisterDependencyInjection(builder.Configuration);

builder.Services.AddUnitOFWorkDependencyInjection();

var supportedCultures = new[] { "en-US", "ar-EG" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("ar-EG")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization(localizationOptions);

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();