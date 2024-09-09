using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.DTO
{
    public class GetApplicationUsage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public int OrganizationId { get; set; }
        public string ApplicationName { get; set; }
        public string TotalUsage { get; set; }
        public string Details { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UsageDate { get; set; } 
    }
}
