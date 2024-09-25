﻿namespace Hublog.Repository.Entities.Model
{
    public class SystemInfoModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string Platform { get; set; }
        public string OSName { get; set; }
        public string OSBuild { get; set; }
        public string SystemType { get; set; }
        public string IPAddress { get; set; }
        public string AppType { get; set; }
        public string HublogVersion { get; set; }
        public int Status { get; set; }
    }
}
