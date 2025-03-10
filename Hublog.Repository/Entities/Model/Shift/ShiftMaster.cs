using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Hublog.Repository.Entities.Model.Shift
{
    public class ShiftMaster
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public TimeSpan Start_time { get; set; }

        [JsonIgnore]
        public TimeSpan End_time { get; set; }

        [JsonPropertyName("start_time")]
        public string StartTimeString
        {
            get => Start_time.ToString(@"hh\:mm\:ss");
            set => Start_time = TimeSpan.Parse(value);
        }

        [JsonPropertyName("end_time")]
        public string EndTimeString
        {
            get => End_time.ToString(@"hh\:mm\:ss");
            set => End_time = TimeSpan.Parse(value);
        }
        public bool Status { get; set; }
    }
}
