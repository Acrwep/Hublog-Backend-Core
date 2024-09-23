namespace Hublog.Repository.Entities.Model.Break
{
    public class UserTotalBreakModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BreakDate { get; set; }
        public string TotalBreakHours { get; set; } 
    }
}
