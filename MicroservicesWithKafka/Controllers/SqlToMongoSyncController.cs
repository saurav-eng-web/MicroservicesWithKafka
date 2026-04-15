using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SqlToMongoSyncController : ControllerBase
    {
        private readonly ISqlToMongoSyncService _syncService;

        public SqlToMongoSyncController(ISqlToMongoSyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncData([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int? regionId)
        {
            try
            {
                var count = await _syncService.SyncAsync(fromDate, toDate, regionId);
                return Ok(new
                {
                    message = "✅ SQL → MongoDB Sync completed successfully",
                    fromDate,
                    toDate,
                    regionId,
                    recordsProcessed = count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Sync failed",
                    error = ex.Message
                });
            }
        }
    }
}
