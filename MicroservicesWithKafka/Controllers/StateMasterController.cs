using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [Route("api/[controller]")]
    [ApiController]
    public class StateMasterController : ControllerBase
    {
        private readonly IStateMasterService _stateMasterService;

        public StateMasterController(IStateMasterService stateMasterService)
        {
            _stateMasterService = stateMasterService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StateMaster>>> GetAllStates()
        {
            var states = await _stateMasterService.GetAllStatesAsync();
            return Ok(states);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StateMaster>> GetStateById(decimal id)
        {
            var state = await _stateMasterService.GetStateByIdAsync(id);
            if (state == null)
                return NotFound();

            return Ok(state);
        }

        [HttpPost]
        public async Task<ActionResult<StateMaster>> CreateState(StateMaster state)
        {
            var createdState = await _stateMasterService.CreateStateAsync(state);
            return CreatedAtAction(nameof(GetStateById), new { id = createdState.State_id }, createdState);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateState(decimal id, StateMaster state)
        {
            if (id != state.State_id)
                return BadRequest("State ID mismatch");

            var result = await _stateMasterService.UpdateStateAsync(state);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteState(decimal id)
        {
            var result = await _stateMasterService.DeleteStateAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
