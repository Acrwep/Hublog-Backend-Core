﻿namespace Hublog.Repository.Entities.DTO
{
    public class CombinedUsageDto
    {
        public string Type { get; set; }
        public string Details { get; set; }
        public string TotalUsage { get; set; }  
        public decimal UsagePercentage { get; set; }
    }
}