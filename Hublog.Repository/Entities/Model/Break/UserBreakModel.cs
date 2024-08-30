namespace Hublog.Repository.Entities.Model.Break
{
    public class UserBreakModel
    {
        public int OrganizationId { get; set; } //(int, not null)
        public int BreakEntryId { get; set; } //(int, not null)

        public int Id { get; set; } //(int, not null)
        public int UserId { get; set; } //(int, not null)

        public DateTime BreakDate { get; set; } //(datetime, not null)

        public DateTime Start_Time { get; set; } //(datetime, not null)
        public DateTime? End_Time { get; set; } //(datetime, not null)
        public int Status { get; set; } //(int, not null)
    }
}
