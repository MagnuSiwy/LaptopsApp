using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Magnuszewski.LaptopsApp.Interfaces;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

string daoAssemblyPath = @"C:\Users\student\Documents\LaptopsApp\DAOSQL\bin\Debug\net8.0-windows\DAOSQL.dll";
Assembly daoAssembly = Assembly.LoadFrom(daoAssemblyPath);

Type laptopStorageType = daoAssembly.GetTypes()
    .FirstOrDefault(t => typeof(ILaptopStorage).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

if (laptopStorageType == null)
{
    throw new InvalidOperationException("No implementation of ILaptopStorage found in the specified assembly.");
}

builder.Services.AddSingleton(typeof(ILaptopStorage), laptopStorageType);

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