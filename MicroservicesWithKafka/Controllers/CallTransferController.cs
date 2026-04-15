using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MicroservicesWithKafka.Models;
    using MicroservicesWithKafka.Services;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class CallTransferController : ControllerBase
    {
        private readonly ICallTransferService _callTransferService;

        public CallTransferController(ICallTransferService callTransferService)
        {
            _callTransferService = callTransferService;
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<CallTransferResponse>> 
            TransferCall([FromBody] CallTransferRequest request)
        {
            var response = await _callTransferService.TransferCallAsync(request);
            return Ok(response);
        }
    }
}
