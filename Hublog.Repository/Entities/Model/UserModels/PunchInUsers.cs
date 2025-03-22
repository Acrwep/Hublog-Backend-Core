using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.UserModels
{
    public class PunchInUsers
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string StartTime { get; set; }
    }
}
