using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Project;

namespace Hublog.Service.Interface
{
    public interface IProjectService
    {
        Task<ProjectModal> InsertProject(ProjectModal project);

        Task<List<ProjectModal>> GetProjects(int organizationId, string searchQuery, string status);

        Task<ProjectModal> GetProjectById(int organizationId, int projectId);

        Task<(ProjectModal UpdateProject, string Message)> UpdateProject(ProjectModal project);

        Task<bool> DeleteProject(int organizationId, int projectId);
    }
}
