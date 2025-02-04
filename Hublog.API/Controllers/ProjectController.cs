using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Project;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("InsertProject")]
        public async Task<IActionResult> InsertProject(ProjectModal project)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is not valid.");
                return BadRequest("Model State is Not Valid");
            }

            try
            {
                var createdProject = await _projectService.InsertProject(project);
                return Ok(createdProject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the project.");
                return StatusCode(500, "Internal server error");
            }
        }

        #region GetProjects
        [HttpGet("GetProjects")]
        public async Task<IActionResult> GetProjects(int organizationId, string searchQuery = "", string status = "")
        {
            try
            {
                var result = await _projectService.GetProjects(organizationId, searchQuery, status);
                if (result.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("No Data Found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region GetProjectById
        [HttpGet("GetProjectById")]
        public async Task<IActionResult> GetProjectById(int organizationId, int projectId)
        {
            try
            {
                var result = await _projectService.GetProjectById(organizationId, projectId);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("No data found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region UpdateProject
        [HttpPut("UpdateProject")]
        public async Task<IActionResult> UpdateProject(ProjectModal project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("ModelState is not valid");
            }

            try
            {
                var (result, message) = await _projectService.UpdateProject(project);
                if (result != null)
                {
                    return Ok(new { Message = message, Project = result });
                }
                return BadRequest(new { Message = message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error updating project: {ex.Message}" });
            }
        }
        #endregion

        #region DeleteProject
        [HttpDelete("DeleteProject")]
        public async Task<IActionResult> DeleteProject(int organizationId, int projectId)
        {
            try
            {
                bool isDeleted = await _projectService.DeleteProject(organizationId, projectId);
                if (isDeleted)
                {
                    return Ok($"Project with {projectId} is deleted");
                }
                else
                {
                    return NotFound($"Project with {projectId} not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting project");
            }
        }
        #endregion
    }
}
