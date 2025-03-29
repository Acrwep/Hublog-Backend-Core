using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model;
using Microsoft.AspNetCore.Mvc;
using Hublog.Repository.Entities.Model.WellNess_Model;

namespace Hublog.Repository.Interface
{
    public interface IWellnessRepository
    {
       Task<ResultModel> InsertWellness(List<UserBreakModel> userBreakModels);
       Task<object> GetWellness([FromQuery] int OrganizationId);
       Task<object> InsertWellnessAsync(WellNess wellness);
       Task<WellNess> UpdateWellNess(int OrganizationId,WellNess WellNess);
       Task<object> GetWellnessSummary(int organizationId, int? teamId, [FromQuery] DateTime Date);
        Task<object> GetWellnessDetails(int organizationId, int? teamId, int? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate);
        Task<object> GetWellnessUserDetails(int organizationId, int? teamId, int? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate);
    }
}
