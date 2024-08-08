namespace Hublog.Repository.Entities.Model
{
    public class UserAttendanceDetailModel
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public bool Active { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public DateTime Total_Time { get; set; }
        public DateTime Late_Time { get; set; }
        public int Status { get; set; }
    }
}
