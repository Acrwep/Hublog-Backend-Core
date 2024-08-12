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

        #region GetDesignationById
        public async Task<Designation> GetDesignationById(int organizationId, int designationId)
        {
            try
            {
                var query = @"SELECT * FROM Designation WHERE OrganizationId = @OrganizationId AND Id = @Id AND Active = 1";
                var parameter = new { OrganizationId = organizationId, Id = designationId };
                return await _dapper.GetAsync<Designation>(query, parameter);
            }
            catch(Exception ex)
            {
                throw new Exception("Error fetching the data", ex);
            }
        }
        #endregion

        #region  InsertDesignation
        public async Task<int> InsertDesignation(Designation designation)
        {
            try
            {
                string query = @"INSERT INTO Designation (Name, Active, Description, Created_date, OrganizationId)
                             VALUES (@Name, @Active, @Description, @Created_date, @OrganizationId)";
                return await _dapper.ExecuteAsync(query, designation);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating Designation", ex);
            }
        }
        #endregion
    }
}
