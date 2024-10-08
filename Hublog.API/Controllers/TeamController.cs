﻿using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ILogger<TeamController> _logger;
        public TeamController(ITeamService teamService, ILogger<TeamController> logger)
        {
            _logger = logger;
            _teamService = teamService;
        }

        #region GetTeams
        [HttpGet("GetTeam")]
        public async Task<IActionResult> GetTeams(int organizationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model state is not valid");
            }
            try
            {
                var result = await _teamService.GetTeams(organizationId);
                if (result.Any())
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching data");
            }
        }
        #endregion

        #region CreateTeam
        [HttpPost("CreateTeam")]
        public async Task<IActionResult> CreateTeam([FromBody] Team team)
        {
            var result = await _teamService.CreateTeam(team);

            if (result.IsSuccessful)
            {
                return Ok(result.CreatedTeam);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        #endregion

        #region UpdateTeam
        [HttpPut("UpdateTeam")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] TeamDTO teamDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model State is Not Valid");
            }

            try
            {
                var (message, updatedTeam) = await _teamService.UpdateTeam(id, teamDto);
                if (updatedTeam != null)
                {
                    return Ok(updatedTeam);
                }
                else if (message != null)
                {
                    if (message.Contains("User is already mapped to this team"))
                    {
                        return Conflict(message); 
                    }
                    else
                    {
                        return NotFound(message);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occurred");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating team");
            }
        }

        //[HttpPut("UpdateTeam")]
        //public async Task<IActionResult> UpdateTeam(int id, Team team)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var updatedTeam = await _teamService.UpdateTeam(id, team);
        //            if(updatedTeam != null)
        //            {
        //                return Ok(updatedTeam);
        //            }
        //            else
        //            {
        //                return NotFound("No data found");
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            return StatusCode(500, "Error Updating Team");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("Model state is not valid");
        //    }
        //}
        #endregion

        #region DeleteTeam
        [HttpDelete("DeleteTeam")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model state is not valid");
            }
            try
            {
                var result = await _teamService.DeleteTeam(id);
                if (result.Contains("Error"))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
        #endregion
    }
}
