using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Organization;

namespace Hublog.Service.Interface
{
    public interface IOrganizationService
    {
        Task<Organizations> InsertAsync(Organizations organization);
        Task<object> UpdateAsync(Organizations organization);
        Task<List<Organizations>> GetAllAsync();
        Task<bool> CheckDomainAvailabilityAsync(string domain);
    }
}
