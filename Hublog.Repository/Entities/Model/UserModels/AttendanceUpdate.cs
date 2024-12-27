using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.UserModels
{
    public class AttendanceUpdate
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime Date { get; set; }
    }
}
