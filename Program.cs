using dotenv.net;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using TestReactOther.BackGroundScheduler;
using TestReactOther.Controllers;
using TestReactOther.Mail;
using TestReactOther.Models;
using TestReactOther.Repositories;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load(); // Charge le fichier .env

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
});
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
});
builder.Services.AddHangfireServer();
builder.Configuration["MailSettings:UserName"] = Environment.GetEnvironmentVariable("USERNAME");
builder.Configuration["MailSettings:Password"] = Environment.GetEnvironmentVariable("PASSWORD");
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddScoped<MailSettings>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<UtilisateurRepository>();
builder.Services.AddScoped<EvenementRepository>();
builder.Services.AddScoped<EvenementController>();
builder.Services.AddScoped<Worker>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();


var app = builder.Build();

// Pour le BackgroundScheduler


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();