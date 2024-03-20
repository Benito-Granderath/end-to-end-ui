using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Server.HttpSys; 

var builder = WebApplication.CreateBuilder(args);

#pragma warning disable CA1416 // Plattformkompatibilität überprüfen


builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDbConnection>((sp) => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LOG_RGLNR}/{action=Index}/{id?}");

app.Run();
