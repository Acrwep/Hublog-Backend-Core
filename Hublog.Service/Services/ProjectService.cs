using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Project;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
using Hublog.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Service.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        #region InsertDesignation
        public async Task<ProjectModal> InsertProject(ProjectModal project)
        {
            var result = await _projectRepository.InsertProject(project);
            if (result > 0)
            {
                return project;
            }
            else
            {
                throw new Exception("Could not create project");
            }
        }
        #endregion

        public async Task<List<ProjectModal>> GetProjects(int organizationId, string searchQuery, string status)
        {
            return await _projectRepository.GetProjects(organizationId, searchQuery, status);
        }

        public async Task<ProjectModal> GetProjectById(int organizationId, int projectId)
        {
            return await _projectRepository.GetProjectById(organizationId, projectId);
        }

        #region UpdateProject
        public async Task<(ProjectModal UpdateProject, string Message)> UpdateProject(ProjectModal project)
        {
            var (rowsAffected, message) = await _projectRepository.UpdateProject(project);
            if (rowsAffected > 0)
            {
                return (project, message);
            }
            return (null, message);
        }
        #endregion

        public async Task<bool> DeleteProject(int organizationId, int designationId)
        {
            var result = await _projectRepository.DeleteProject(organizationId, designationId);
            return result > 0;
        }
    }
}
