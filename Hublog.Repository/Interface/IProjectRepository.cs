using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Project;

namespace Hublog.Repository.Interface
{
    public interface IProjectRepository
    {
        Task<int> InsertProject(ProjectModal project);

        Task<List<ProjectModal>> GetProjects(int organizationId, string searchQuery, string status);

        Task<ProjectModal> GetProjectById(int organizationId, int projectId);

        Task<(int RowsAffected, string Message)> UpdateProject(ProjectModal project);

        Task<int> DeleteProject(int organizationId, int projectId);
    }
}
