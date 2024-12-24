using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model
{
    public class DynamicReportRequest
    {
        public int OrganizationId { get; set; }
        public int? TeamId { get; set; }
        public int? UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // User Information (Filter Parameters)
        public bool? FirstName { get; set; }
        public bool? LastName { get; set; }
        public bool? EmployeeId { get; set; }
        public bool? Email { get; set; }
        public bool? TeamName { get; set; }
        public bool? Manager { get; set; }

        // Working Time (Filter Parameters)
        public bool? TotalWorkingtime { get; set; }
        public bool? TotalOnlinetime { get; set; }
        public bool? TotalBreaktime { get; set; }
        public bool? AverageBreaktime { get; set; }

        // Activity (Filter Parameters)
        public bool? TotalActivetime { get; set; }
        public bool? ActivitePercent { get; set; }
        public bool? TotalIdletime { get; set; }
        public bool? AverageIdletime { get; set; }

        // Productive 
        public bool? TotalProductivetime { get; set; }
        public bool? ProductivityPercent { get; set; }
        public bool? AverageProductivetime { get; set; }
        public bool? Totalunproductivetime { get; set; }
        public bool? Averageunproductivetime { get; set; }
        public bool? Totalneutraltime { get; set; }
        public bool? Averageneutraltime { get; set; }

        // Correct Data Types for Output (String fields for time values)\
        public int? Id { get; set; }
        public string FirstNameOutput { get; set; }
        public string LastNameOutput { get; set; }
        public string EmployeeIdOutput { get; set; }
        public string EmailOutput { get; set; }
        public string TeamNameOutput { get; set; }

        // Time-based output properties (Renamed to avoid conflict)
        public string TotalWorkingtimeOutput { get; set; }
        public string TotalOnlinetimeOutput { get; set; }
        public string TotalBreaktimeOutput { get; set; }
        public string AverageBreaktimeOutput { get; set; }
        public string TotalActivetimeOutput { get; set; }
        public string ActivitePercentOutput { get; set; }
        public string TotalIdletimeOutput { get; set; }
        public string AverageIdletimeOutput { get; set; }
        public string Total_Productivetime { get; set; }
        public Double Productivity_Percent { get; set; }
        public string Average_Productivetime { get; set; }
        public string Total_unproductivetime { get; set; }
        public string Average_unproductivetime { get; set; }
        public string Total_neutraltime { get; set; }
        public string Average_neutraltime { get; set; }

    }
}
