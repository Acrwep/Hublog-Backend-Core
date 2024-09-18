namespace Hublog.Repository.Entities.Model.DashboardModel
{
    public class AttendanceDashboardSummaryModel
    {
        public string TotalWorkingTime { get; set; }
        public string TotalBreakTime { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public decimal AttendancePercentage { get; set; }
        public int LateArrivals { get; set; }
        public int OnTimeArrivals { get; set; }
        public decimal LateArrivalsPercentage { get; set; }
        public string AverageWorkingHours { get; set; } 
    }
}
