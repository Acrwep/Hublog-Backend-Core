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

        public async Task<Designation> GetDesignationById(int organizationId, int designationId)
        {
            return await _designationRepository.GetDesignationById(organizationId, designationId);
        }

        public async Task<bool> DeleteDesignation(int organizationId, int designationId)
        {
            var result = await _designationRepository.DeleteDesignation(organizationId, designationId);
            return result > 0;
        }

        #region InsertDesignation
        public async Task<Designation> InsertDesignation(Designation designation)
        {
            designation.Active = true;
            TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            designation.Created_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istZone);

            var result = await _designationRepository.InsertDesignation(designation);
            if (result > 0)
            {
                return designation;
            }
            else
            {
                throw new Exception("Could not create designation");
            }
        }
        #endregion

        #region UpdateDesignation
        public async Task<(Designation UpdatedDesignation, string Message)> UpdateDesignation(Designation designation)
        {
            var (rowsAffected, message) = await _designationRepository.UpdateDesignation(designation);
            if (rowsAffected > 0)
            {
                return (designation, message);
            }
            return (null, message);
        }
        #endregion
    }
}
