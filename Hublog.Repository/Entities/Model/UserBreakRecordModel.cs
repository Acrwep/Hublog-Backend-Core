namespace Hublog.Repository.Entities.Model
{
    public class UserBreakRecordModel
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string BreakType { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public DateTime BreakDate { get; set; }
        public string BreakDuration { get; set; }
    }
}
