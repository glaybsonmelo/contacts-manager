using Entities;
using Microsoft.EntityFrameworkCore;
using RespositoryContracts;
using ServiceContracts;
using Services;
using Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

//add services into IoC container

builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonService>();


//Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    }
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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