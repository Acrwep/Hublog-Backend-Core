using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _designationRepository;
        public DesignationService(IDesignationRepository designationRepository)
        {
            _designationRepository = designationRepository;
        }
        public async Task<List<Designation>> GetDesignationAll(int organizationId)
        {
            return await _designationRepository.GetDesignationAll(organizationId);
        }
    }
}
