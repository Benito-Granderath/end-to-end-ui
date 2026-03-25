using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authentication.Negotiate;
using RGLNR_Interface.Models;
using RGLNR_Interface.Middleware;
using RGLNR_Interface.Services;


var builder = WebApplication.CreateBuilder(args);

#pragma warning disable CA1416 // Plattformkompatibilit�t �berpr�fen


builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new ValidateSidAttribute());
});
builder.Services.Configure<LocalTestingOptions>(
    builder.Configuration.GetSection(LocalTestingOptions.SectionName));
builder.Services.AddScoped<IDbConnection>((sp) => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ActiveDirectorySearch>();
builder.Services.AddScoped<ILocalTestingContext, LocalTestingContext>();
builder.Services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme);
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
app.UseMiddleware<LocalTestingUserMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LOG_RGLNR}/{action=Index}/{id?}");

app.Run();
