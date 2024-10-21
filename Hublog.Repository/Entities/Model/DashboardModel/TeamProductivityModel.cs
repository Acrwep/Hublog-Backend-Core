namespace Hublog.Repository.Entities.Model.DashboardModel
{
    public class TeamProductivityModel
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public decimal WorkingHourPercentage { get; set; }
    }
}
