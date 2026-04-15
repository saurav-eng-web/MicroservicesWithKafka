using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services.Mongo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Enums;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class CallSearchController : ControllerBase
    {
        private readonly MongoDBService _mongoService;

        public CallSearchController(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        //[HttpGet("search")]
        //     public async Task<IActionResult> SearchCalls(
        //     [FromQuery] string? serialNo,
        //     [FromQuery] string? callNumber,
        //     [FromQuery] string? phone)
        //{
        //    var result = await _mongoService.SearchCallsAsync(serialNo, callNumber, phone);

        //    if (result == null || !result.Any())
        //        return NotFound("No matching records found.");

        //    return Ok(result);
        //}


        // ==== this if for use inly call center role
        //[HttpGet("search")]
        //public async Task<IActionResult> Search([FromQuery] string? serialNo, [FromQuery] string? callNumber, [FromQuery] string? phone)
        //{
        //    var results = await _mongoService.SearchCallsAsync(serialNo, callNumber, phone);

        //    if (results == null || results.Count == 0)
        //        return NotFound(new { message = "No matching records found." });

        //    return Ok(results);
        //}

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? searchText)
        {
            string roleName = User.FindFirst("rolename")?.Value ?? string.Empty;

            int bid = int.Parse(User.FindFirst("Branch_ID")?.Value ?? "0");
            int regionId = int.Parse(User.FindFirst("Cat_MapID")?.Value ?? "0");
            int asmid = int.Parse(User.FindFirst("ASMID")?.Value ?? "0");
            int sfId = int.Parse(User.FindFirst("SF_ID")?.Value ?? "0");

            ScopeType scopeType = ScopeType.None;
            int scopeId = 0;

            // Role- Scope mapping
            switch (roleName)
            {
                case "SF/SSD":
                    scopeType = ScopeType.Sf;
                    scopeId = sfId;
                    break;

                case "ASM":
                    scopeType = ScopeType.Asm;
                    scopeId = asmid;
                    break;

                case "ZONE": // Branch / Region
                    scopeType = ScopeType.Region;
                    scopeId = regionId;
                    break;

                case "BR":
                    scopeType = ScopeType.Bid;
                    scopeId = bid;
                    break;
                
                case "ADMIN":
                case "CC":
                case "HOuser":
                case "HOuser1":
                case "NWH":
                case "Finance":
                case "TechnicalExe":
                case "NWHExe":
                    scopeType = ScopeType.None; // full access
                    break;


            }

            // Safety check
            if (scopeType != ScopeType.None && scopeId <= 0)
                return Forbid("Invalid scope for role");

            var results = await _mongoService.SearchCallsAsync(
                searchText,
                scopeType,
                scopeId
            );

            if (results == null || results.Count == 0)
            {
                return StatusCode(204, new
                {
                    statusCode = 204,
                    statusMessage = "No matching records found",
                    data = new List<CallSearchResponseModel>()
                });
            }

            return Ok(new
            {
                statusCode = 200,
                statusMessage = "Records found",
                data = results
            });
        }






    }
}
