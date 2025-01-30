using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Magnuszewski.LaptopsApp.Interfaces;
using Magnuszewski.LaptopsApp.DAO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ILaptopStorage, SqlLaptopStorage>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();