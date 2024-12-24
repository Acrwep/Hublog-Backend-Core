namespace Hublog.Repository.Entities.Model.Break
{
    public class BreaksReport
    {
        public string Full_Name { get; set; }
        public string Team_Name { get; set; }
        public string BreakName { get; set; }
        public DateTime BreakDate { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public string BreakDuration { get; set; }
        public int TeamId { get; set; }
        public int OrganizationId { get; set; }
    }
}
