using System;
using System.Text.Json.Serialization;

namespace Hublog.Repository.Entities.Model.LivestreamModal
{
    public class LivestreamModal
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("organizationId")]
        public int OrganizationId { get; set; }

        [JsonPropertyName("activeApp")]
        public string ActiveApp { get; set; }

        [JsonPropertyName("activeUrl")]
        public string ActiveUrl { get; set; }

        [JsonPropertyName("liveStreamStatus")]
        public bool LiveStreamStatus { get; set; }

        [JsonPropertyName("activeAppLogo")]
        public byte[] ActiveAppLogo { get; set; }   // Changed from string to byte[]

        [JsonPropertyName("activeScreenshot")]
        public byte[] ActiveScreenshot { get; set; }  // Changed from string to byte[]

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
