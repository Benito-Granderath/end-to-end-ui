using RGLNR_Interface.Models;
using RGLNR_Interface.Middleware;
using RGLNR_Interface.Services;
using RGLNR_Interface.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<EnsureAuthenticatedFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<EnsureAuthenticatedFilter>();
});
builder.Services.Configure<LocalTestingOptions>(
    builder.Configuration.GetSection(LocalTestingOptions.SectionName));
builder.Services.AddScoped<IActiveDirectoryService, ActiveDirectoryService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IStagingDataService, StagingDataService>();
builder.Services.AddScoped<ILocalTestingContext, LocalTestingContext>();
#pragma warning disable CA1416 // Platform compatibility
builder.Services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme);
#pragma warning restore CA1416
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<LocalTestingUserMiddleware>();
}
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LOG_RGLNR}/{action=Index}/{id?}");

app.Run();
