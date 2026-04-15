using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]  // 🔒 Secure all endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class ActionMasterController : ControllerBase
    {
        private readonly IActionMasterService _actionService;

        public ActionMasterController(IActionMasterService actionService)
        {
            _actionService = actionService;
        }

        // INSERT
        [HttpPost("insert")]
        public async Task<IActionResult> Insert([FromBody] ActionMaster model)
        {
            var newId = await _actionService.InsertActionAsync(model);
            return Ok(new { Message = "Inserted Successfully", ID = newId });
        }

        // UPDATE
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] ActionMaster model)
        {
            var result = await _actionService.UpdateActionAsync(model);
            return Ok(new { Message = result });
        }

        // DELETE
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _actionService.DeleteActionAsync(id);
            return Ok(new { Message = result });
        }

        // GET with Filters
        [HttpPost("get-actions")]
        public async Task<IActionResult> GetActions([FromBody] ActionMaster filter)
        {
            var json = await _actionService.GetActionsAsync(filter);

            if (string.IsNullOrEmpty(json) || json == "{}")
            {
                return NotFound(new { message = "No actions found" });
            }

            return Content(json, "application/json");
        }


      
    }
}
