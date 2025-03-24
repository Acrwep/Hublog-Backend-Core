using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.DTO
{
    public class UsersDTO
    {
        public int Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string full_Name { get; set; }       
        public string Email { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? DOJ { get; set; }
        public string Phone { get; set; }
        public string UsersName { get; set; }
        public string Password { get; set; } 
        public string Gender { get; set; }
        public int OrganizationId { get; set; }
        public int RoleId { get; set; }
        public int DesignationId { get; set; }
        public int TeamId { get; set; }
        public bool Active { get; set; }
        public string DesignationName { get; set; }
        public string TeamName { get; set; }
        public string EmployeeID { get; set; }
        public bool ManagerStatus { get; set; }
    }
}
