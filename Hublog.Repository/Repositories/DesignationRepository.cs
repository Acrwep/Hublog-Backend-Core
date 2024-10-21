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
        public async Task<List<Designation>> GetDesignationAll(int organizationId, string searchQuery)
        {
            try
            {
                var query = @"SELECT * FROM Designation 
                              WHERE OrganizationId = @OrganizationId AND Name LIKE @SearchQuery";
                var parameter = new { OrganizationId = organizationId, SearchQuery = $"%{searchQuery}%", };
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

        #region UpdateDesignation
        public async Task<(int RowsAffected, string Message)> UpdateDesignation(Designation designation)
        {
            try
            {
                if (designation.Active == false)
                {
                    bool isMapped = await IsDesignationMappedToUser(designation.Id);
                    if (isMapped)
                    {
                        return (0, "Designation is currently mapped to a user and cannot be deactivated.");
                    }
                }

                string query = @"UPDATE Designation 
                        SET Name = @Name, 
                            Active = @Active,
                            Description = @Description, 
                            Created_date = @Created_date, 
                            OrganizationId = @OrganizationId 
                        WHERE Id = @Id";
                int rowsAffected = await _dapper.ExecuteAsync(query, designation);
                return (rowsAffected, "Designation updated successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating designation", ex);
            }
        }

        #endregion

        #region IsDesignationMappedToUser
        public async Task<bool> IsDesignationMappedToUser(int designationId)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE DesignationId = @DesignationId";
            var count = await _dapper.ExecuteScalarAsync<int>(query, new { DesignationId = designationId });
            return count > 0;
        }
        #endregion

        #region DeleteDesignation
        public async Task<int> DeleteDesignation(int organizationId, int designationId)
        {
            var query = @"DELETE Designation WHERE OrganizationId = @OrganizationId AND Id = @Id";
            var parameter = new { OrganizationId = organizationId, Id = designationId };
            return await _dapper.ExecuteAsync(query, parameter);
        }
        #endregion 
    }
}
