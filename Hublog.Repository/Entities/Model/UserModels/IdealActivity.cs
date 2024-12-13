using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.UserModels
{
    public class IdealActivity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public int Ideal_duration { get; set; }
        public DateTime Ideal_DateTime { get; set; }

    }
}
