namespace Hublog.Repository.Entities.Model.UserModels
{
    public class Users
    {
        public int Id { get; set; } //(int, not null)
        public string First_Name { get; set; } //(varchar(100), not null)
        public string Last_Name { get; set; } //(varchar(100), null)
        public string full_Name { get; set; }   
        public string Email { get; set; } //(varchar(100), not null)
        public DateTime? DOB { get; set; } //(date, null)
        public DateTime? DOJ { get; set; } //(date, null)
        public string Phone { get; set; } //(varchar(100), not null)
        public string UsersName { get; set; } //(varchar(100), not null)
        public string Password { get; set; } //(varchar(100), not null)
        public string Gender { get; set; } //(varchar(100), null)
        public int OrganizationId { get; set; } //(int, not null)
        public int RoleId { get; set; } //(int, not null)
        public int DesignationId { get; set; } //(int, not null)
        public int TeamId { get; set; } //(int, not null)
        public bool Active { get; set; } //(bit, not null)
        public string RoleName { get; set; } //(varchar(100), not null)
        public string AccessLevel { get; set; } //(varchar(100), not null)
        public string DesignationName { get; set; } //(varchar(100), not null)
        public string TeamName { get; set; } //(varchar(100), not null)
        public string EmployeeID { get; set; } //(varchar(100), null)
        public bool ManagerStatus { get; set; } 


        public string Subject { get; set; } = "Welcome to Hublog!";
        //public string Body { get; set; }

    }
}
