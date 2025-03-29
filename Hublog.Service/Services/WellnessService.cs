using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Hublog.Repository.Entities.Model.WellNess_Model;
using Hublog.Repository.Entities.Model.UserModels;

namespace Hublog.Service.Services
{
    public class WellnessService : IWellnessService
    {
        private readonly IWellnessRepository _IWellnessRepository;
        public WellnessService(IWellnessRepository wellnessRepository)
        {
            _IWellnessRepository = wellnessRepository;
        }

        public async Task<object> GetWellness([FromQuery] int OrganizationId)
        {
            return await _IWellnessRepository.GetWellness(OrganizationId);
        }
        public async Task<WellNess> UpdateWellNess(int OrganizationId, WellNess WellNess)
        {
            return await _IWellnessRepository.UpdateWellNess(OrganizationId,WellNess);
        }
        public async Task<object> GetWellnessSummary(int organizationId, int? teamId, [FromQuery] DateTime Date)
        {
            return await _IWellnessRepository.GetWellnessSummary(organizationId, teamId, Date);
        }
        public async Task<object> GetWellnessDetails(int organizationId, int? teamId, int? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            return await _IWellnessRepository.GetWellnessDetails(organizationId, teamId, userId, startDate, endDate);
        }
        public async Task<object> GetWellnessUserDetails(int organizationId, int? teamId, int? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            return await _IWellnessRepository.GetWellnessUserDetails(organizationId, teamId, userId, startDate, endDate);
        }

        public async Task<object> InsertWellnessAsync(WellNess wellness)
        {
           var insertWellness=await _IWellnessRepository.InsertWellnessAsync(wellness);
            if (insertWellness == null)
            {
                throw new InvalidOperationException("Insert Operation Failed");
            }
            return insertWellness;
        }
    }
}
