using Hublog.API.Extensions;

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

var app = builder.Build();

app.UseCors(options =>
{
    //options.WithOrigins("https://hublog.org") //frontend production url
    options.WithOrigins("http://localhost:3000") //frontend local url
           .AllowAnyHeader()
           .AllowAnyMethod();
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

app.MapControllers();

app.Run();
