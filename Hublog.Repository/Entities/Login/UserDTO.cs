namespace Hublog.Repository.Entities.Login
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? DOJ { get; set; }
        public string Phone { get; set; }
        public string UsersName { get; set; }
        public string Gender { get; set; }
        public int OrganizationId { get; set; }
        public int RoleId { get; set; }
        public int DesignationId { get; set; }
        public int TeamId { get; set; }
        public bool Active { get; set; }
        public string RoleName { get; set; }
        public string AccessLevel { get; set; }
        public string DesignationName { get; set; }
        public string TeamName { get; set; }
        public string EmployeeID { get; set; }
    }

}
