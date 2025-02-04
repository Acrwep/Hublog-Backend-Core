using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Project;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly Dapperr _dapper;
        public ProjectRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        public async Task<int> InsertProject(ProjectModal project)
        {
            try
            {
                string query = @"
                INSERT INTO Project (Name, OrganizationId, Description, Start_Date, End_Date, Status)
                VALUES (@Name, @OrganizationId, @Description, @Start_date, @End_date, @Status)";
                return await _dapper.ExecuteAsync(query, project);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating Project", ex);
            }
        }

        #region GetProjects
        public async Task<List<ProjectModal>> GetProjects(int organizationId, string searchQuery, string status)
        {
            try
            {
                var query = @"SELECT * FROM Project WHERE OrganizationId = @OrganizationId";

                var parameters = new DynamicParameters();
                parameters.Add("OrganizationId", organizationId);

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query += " AND Name LIKE @SearchQuery";
                    parameters.Add("SearchQuery", $"%{searchQuery}%");
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND Status = @Status";
                    parameters.Add("Status", status);
                }

                return await _dapper.GetAllAsync<ProjectModal>(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching the data", ex);
            }
        }
        #endregion

        #region GetProjectById
        public async Task<ProjectModal> GetProjectById(int organizationId, int projectId)
        {
            try
            {
                var query = @"SELECT * FROM Project WHERE OrganizationId = @OrganizationId AND Id = @Id AND Active = 1";
                var parameter = new { OrganizationId = organizationId, Id = projectId };
                return await _dapper.GetAsync<ProjectModal>(query, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching the data", ex);
            }
        }
        #endregion

        #region UpdateProject
        public async Task<(int RowsAffected, string Message)> UpdateProject(ProjectModal project)
        {
            try
            {
                string query = @"UPDATE Project 
                        SET Name = @Name, 
                            OrganizationId = @OrganizationId,
                            Description = @Description,
                            Start_Date = @Start_date,
                            End_Date = @End_date,
                            Status = @Status
                        WHERE Id = @Id";
                int rowsAffected = await _dapper.ExecuteAsync(query, project);
                return (rowsAffected, "Project updated successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating project", ex);
            }
        }
        #endregion

        #region DeleteProject
        public async Task<int> DeleteProject(int organizationId, int projectId)
        {
            var query = @"DELETE FROM Project WHERE OrganizationId = @OrganizationId AND Id = @Id";
            var parameter = new { OrganizationId = organizationId, Id = projectId };
            return await _dapper.ExecuteAsync(query, parameter);
        }
        #endregion
    }
}
