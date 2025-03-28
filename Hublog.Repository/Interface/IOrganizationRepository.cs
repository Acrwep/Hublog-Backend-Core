using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Organization;

namespace Hublog.Repository.Interface
{
    public interface IOrganizationRepository
    {
        Task<Organizations> InsertAsync(Organizations organization);
        Task<object> UpdateAsync(Organizations organization);
        Task<List<Organizations>> GetAllAsync();
    }
}
