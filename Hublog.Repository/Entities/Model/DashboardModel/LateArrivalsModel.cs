namespace Hublog.Repository.Entities.Model.DashboardModel
{
    public class LateArrivalsModel
    {
        public DateTime AttendanceDate { get; set; }
        public int TotalUsers { get; set; }
        public int OnTimeArrival { get; set; }
        public int LateArrival { get; set; }    
    }
}
