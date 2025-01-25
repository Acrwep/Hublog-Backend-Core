using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Manual_Time;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Service.Services
{
    public class Manual_TimeService: IManual_TimeService
    {
        private readonly IManual_TimeRepository _IActivityService;
        public Manual_TimeService(IManual_TimeRepository IActivityService)
        {
            _IActivityService = IActivityService;
        }
        public async Task<Manual_Time> InsertManualTime(Manual_Time manual_Time)
        {
            return await _IActivityService.InsertManualTime(manual_Time);
        }
        public async Task<IEnumerable<GetManualList>> GetManualTime(int organizationId, int? teamid, int? userId, DateTime? startDate, DateTime? endDate)
        { 
            return await _IActivityService.GetManualTime( organizationId,teamid, userId, startDate,endDate);
        }
    }
}
