using Hublog.API.Extensions;
using Hublog.API.Hub;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

// Scope
builder.Services.ConfigureScope(configuration);
builder.Services.ConfigureServices(configuration);
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 1024 * 1024 * 10; // Example: 10 MB
});
var app = builder.Build();

//app.UseCors(options =>
//{
//    //options.WithOrigins("https://hublog.org") //frontend production url
//    options.WithOrigins("http://localhost:3000") //frontend local url
//           .AllowAnyHeader()
//           .AllowAnyMethod();
//});

app.UseCors(options =>
{
    options.WithOrigins("https://hublog.org", "http://localhost:3000") // Allow both production and local URLs
           .AllowAnyHeader()
           .AllowAnyMethod()
               .AllowCredentials();  // Allow credentials (cookies, authentication headers)
});


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
app.UseCors("AllowReactApp");  // Use the CORS policy
app.MapHub<LiveStreamHub>("/livestreamHub");

app.MapControllers();


app.Run();
