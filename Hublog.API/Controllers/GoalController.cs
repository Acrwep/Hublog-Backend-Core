using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Goals;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _goalService;
        public GoalController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        [HttpPost]
        [Route("InsertGoals")]
        public async Task<IActionResult> InsertGoals(Goal goal)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var createdGoal = await _goalService.InsertGoals(goal);
                    if (createdGoal != null)
                    {
                        return Ok(createdGoal);
                    }
                    else
                    {
                        return StatusCode(500, "Could not create Breakmaster");
                    }
                }
                else
                {
                    return BadRequest("Model state is not valid");
                }
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut]
        [Route("UpdateGoals")]
        public async Task<IActionResult> UpdateGoals(Goal goal)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var updatedGoal = await _goalService.UpdateGoals(goal);
                    if (updatedGoal != null)
                    {
                        return Ok(updatedGoal);
                    }
                    else
                    {
                        return NotFound("Goals Not Found");
                    }
                }
                else
                {
                    return BadRequest("Model state is not valid");
                }
              
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error updating Goals");
            }
           
        }

        [HttpGet]
        [Route("GetGoals")]
        public async Task<IActionResult> GetGoals(int organizationId)
        {
            try
            {
               
                var Goals = await _goalService.GetGoals(organizationId);
                if(Goals == null)
                {
                    return NotFound("Goals value Not Founded");
                }
                return Ok(Goals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
