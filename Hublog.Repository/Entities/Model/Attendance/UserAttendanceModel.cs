namespace Hublog.Repository.Entities.Model.Attendance
{
    public class UserAttendanceModel
    {
        public int OrganizationId { get; set; } //(int, not null)
        public int Id { get; set; } //(int, not null)
        public int UserId { get; set; } //(int, not null)
        public DateTime AttendanceDate { get; set; } //(datetime, not null)

        public DateTime? Start_Time { get; set; } //(datetime,  null)
        public DateTime? End_Time { get; set; } //(datetime,  null)
        public DateTime? Total_Time { get; set; } //(datetime,  null)
        public DateTime? Late_Time { get; set; } //(datetime,  null)
        public int Status { get; set; } //(int, not null)
        public string? Punchout_type { get; set; }
    }
}
