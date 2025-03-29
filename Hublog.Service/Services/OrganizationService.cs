using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        public OrganizationService(IOrganizationRepository organizationRepository)
        {
            _organizationRepository=organizationRepository;
        }

      
        public async Task<List<Organizations>> GetAllAsync()
        {
            return await _organizationRepository.GetAllAsync();
        }

        public async Task<Organizations> InsertAsync(Organizations organization)
        {
            var createdOrganization = await _organizationRepository.InsertAsync(organization);
            if (createdOrganization == null)
            {
                throw new InvalidOperationException("The Email is already exists");
            }
            return createdOrganization;
        }

        public async Task<object> UpdateAsync(Organizations organization)
        {

            var updatedOrganization=await _organizationRepository.UpdateAsync(organization);
            if (updatedOrganization == null)
            {
                throw new InvalidOperationException("The Email is already exists");
            }
            return updatedOrganization;

        }

        public async Task<bool> CheckDomainAvailabilityAsync(string domain)
        {
            return await _organizationRepository.CheckDomainAvailabilityAsync(domain);
        }

    }
}
