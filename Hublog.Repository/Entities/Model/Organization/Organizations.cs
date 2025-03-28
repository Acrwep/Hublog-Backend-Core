using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Organization
{
    public class Organizations
    {
        public int Id { get; set; }
        public string Organization_Name { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Domain { get; set; }
        public int Licence { get; set; }
        public string PlanName { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public string WebsiteUrl { get; set; }
        public string LinkdinUrl { get; set; }
        public decimal PaidAmount { get; set; }
    }
}
