using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Project
{
    public class ProjectModal
    {
        public int Id { get; set; } //(int, not null)
        public int OrganizationId { get; set; } //(int, not null)
        public string Name { get; set; } //(varchar(100), not null)
        public string Description { get; set; } //(varchar(200), null)
        public DateTime Start_date { get; set; } //(datetime, not null)
        public DateTime End_date { get; set; } //(datetime, not null)
        public string Status { get; set; } //(bit, not null)

    }
}
