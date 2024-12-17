using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Activity;
using Hublog.Repository.Entities.Model.Break;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Repository.Interface
{
    public interface IActivityRepository
    {
        Task<object> GetActivityBreakDown(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);
        Task<dynamic> MostLeast_Teamwise_Activity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);

    }
}
