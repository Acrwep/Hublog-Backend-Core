using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Activity;
using Hublog.Repository.Entities.Model.Break;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Service.Interface
{
    public interface IActivityService
    {
        Task<object> GetActivityBreakDown(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);
        Task<dynamic> Date_wise_Activity(int organizationId, int? teamId, int? userid, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);

    }
}
