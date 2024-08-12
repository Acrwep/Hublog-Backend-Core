using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly Dapperr _dapper;
        public DesignationRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        #region GetDesignationAll
        public async Task<List<Designation>> GetDesignationAll(int organizationId)
        {
            try
            {
                var query = @"SELECT * FROM Designation WHERE OrganizationId = @OrganizationId";
                var parameter = new { OrganizationId = organizationId };
                return await _dapper.GetAllAsync<Designation>(query, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching the data", ex);
            }
        }
        #endregion 
    }
}
