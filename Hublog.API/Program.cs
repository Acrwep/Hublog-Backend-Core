using System.Data;
using System.Data.SqlClient;
using Hublog.API.Extensions;
using Hublog.API.Hub;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using static Hublog.Repository.Common.Dapperr;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//var configuration = builder.Configuration;
var configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json")
.Build();

builder.Services.AddControllers();
builder.Services.AddLogging();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Scope
builder.Services.ConfigureScope(configuration);
builder.Services.ConfigureServices(configuration);
builder.Services.AddSingleton<IAuthorizationHandler, AdminOrManagerHandler>();



builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 1024 * 1024 * 10; // Example: 10 MB
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://hublog.org", "http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
var app = builder.Build();

//app.UseCors(options =>
//{
//    options.WithOrigins("https://hublog.org", "http://localhost:3000") // Allow both production and local URLs
//           .AllowAnyHeader()
//           .AllowAnyMethod()
//           .AllowCredentials();
//});

app.UseStaticFiles();
app.UseCors("AllowSpecificOrigin");


app.MapHub<LiveStreamHub>("/livestreamHub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.Run();



