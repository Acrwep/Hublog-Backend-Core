using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.AlertModel
{
    public class Alert_Rule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AlertThreshold { get; set; }
        public int PunchoutThreshold { get; set; }
        public bool Status { get; set; }
        public int OrganizationId { get; set; } //(int, not null)
    }
}
