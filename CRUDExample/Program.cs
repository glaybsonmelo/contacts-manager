using Entities;
using Microsoft.EntityFrameworkCore;
using RespositoryContracts;
using ServiceContracts;
using Services;
using Repositories;
using CRUDExample;
using CRUDExample.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
} else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

if (app.Environment.IsEnvironment("Test") == false) {
    //Rotativa executable (for convert view into pdf)
    Rotativa.AspNetCore.RotativaConfiguration
        .Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { } // Make the auto-generated program accessible programmatically