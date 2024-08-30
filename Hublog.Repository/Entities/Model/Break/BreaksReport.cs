namespace Hublog.Repository.Entities.Model.Break
{
    public class BreaksReport
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string BreakName { get; set; }
        public DateTime BreakDate { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public int TeamId { get; set; }
        public int OrganizationId { get; set; }
    }
}
