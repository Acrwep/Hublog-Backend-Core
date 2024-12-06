using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.UserModels
{
    public  class UserActivity
    {
        public int Id { get; set; }

        public int  UserId { get; set; }

        public DateTime TriggeredTime { get; set; }
    }
}
