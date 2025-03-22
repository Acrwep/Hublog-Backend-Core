using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        public string ActiveAppLogo { get; set; }

        [JsonPropertyName("activeScreenshot")]
        public string ActiveScreenshot { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
