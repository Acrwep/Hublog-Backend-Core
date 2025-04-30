using System.Net.WebSockets;
using System.Text;
using Hublog.API.Extensions;
//using Hublog.API.Hub;
using Hublog.Repository.Entities.Model;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//var configuration = builder.Configuration;
var configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json")
.Build();

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
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

var allowedRootDomains = builder.Configuration
    .GetSection("CorsSettings:AllowedRootDomains")
    .Get<List<string>>() ?? new List<string>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMultipleOrgSubdomains", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return allowedRootDomains.Any(domain =>
                        uri.Host == domain || uri.Host.EndsWith("." + domain));
                }
                return false;
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin", builder =>
//    {
//        builder.WithOrigins("https://workstatus.qubinex.com", "http://localhost:3000")
//               .AllowAnyHeader()
//               .AllowAnyMethod()
//               .AllowCredentials();
//    });
//});
var app = builder.Build();

//app.UseCors(options =>
//{
//    options.WithOrigins("https://workstatus.qubinex.com", "http://localhost:3000") // Allow both production and local URLs
//           .AllowAnyHeader()
//           .AllowAnyMethod()
//           .AllowCredentials();
//});

app.UseStaticFiles();
app.UseCors("AllowMultipleOrgSubdomains");
//app.UseCors("AllowSpecificOrigin");


//app.MapHub<LiveStreamHub>("/livestreamHub");

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

// 👇 Enable WebSockets
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
    ReceiveBufferSize = 4 * 1024
};
app.UseWebSockets(webSocketOptions);
app.Map("/ws/livestream", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("WebSocket connected");

        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received: {message}");

            var echoBytes = Encoding.UTF8.GetBytes("Echo: " + message);
            await webSocket.SendAsync(new ArraySegment<byte>(echoBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});



app.MapControllers();


app.Run();