//using Microsoft.AspNetCore.SignalR;
//using Hublog.Repository.Entities.Model.LivestreamModal;
//using System.Text.Json; // Ensure you have this using directive

//namespace Hublog.API.Hub
//{
//    public class LiveStreamHub : Microsoft.AspNetCore.SignalR.Hub
//    {
//        public async Task SendLiveData(object payload)
//        {
//            try
//            {
//                // Convert 'object' payload to JSON string
//                var jsonPayload = JsonSerializer.Serialize(payload);

//                // Define case-insensitive deserialization options
//                var options = new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                };
//                // Deserialize JSON string into LiveDataPayload class
//                var data = JsonSerializer.Deserialize<LivestreamModal>(jsonPayload);

//                if (data == null)
//                {
//                    Console.WriteLine("Invalid data format received.");
//                    return;
//                }

//                Console.WriteLine($"Server received data: {data.UserId}, {data.OrganizationId}, {data.ActiveApp}");

//                // Broadcast to all connected clients
//                await Clients.All.SendAsync("ReceiveLiveData", data.UserId, data.OrganizationId, data.ActiveApp, data.ActiveUrl, data.LiveStreamStatus, data.ActiveAppLogo, data.ActiveScreenshot, data.Latitude, data.Longitude);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error processing payload: {ex.Message}");
//            }
//        }
//    }
//}
