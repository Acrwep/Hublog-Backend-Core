using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Activity;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Service.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _IActivityService;
        public ActivityService(IActivityRepository IActivityService)
        {
            _IActivityService = IActivityService;
        }
        public async Task<object> GetActivityBreakDown(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            return await _IActivityService.GetActivityBreakDown(organizationId, teamId, userId, fromDate, toDate);
        }
        public async Task<object> MostLeast_Teamwise_Activity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return await _IActivityService.MostLeast_Teamwise_Activity(organizationId, teamId, fromDate, toDate);
        }
    }
}
