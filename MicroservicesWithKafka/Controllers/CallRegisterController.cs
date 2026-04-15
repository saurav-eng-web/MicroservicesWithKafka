
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MicroservicesWithKafka.Services.Kafka;
using System.Text.Json;

[Authorize] // 🔒 Applies to all actions below
[ApiController]
[Route("api/[controller]")]
public class CallRegisterController : ControllerBase
{
    private readonly ICallRegisterService _service;
    private readonly KafkaProducerService _kafkaProducer;
    public CallRegisterController(ICallRegisterService service, KafkaProducerService kafkaProducer)
    {
        _service = service;
        _kafkaProducer = kafkaProducer;
    }

    //[Authorize(Policy = "CallCenterPolicy")] // 👈 Restrict to other role
    [HttpPost]
    [Route("create-complaint")]
    public async Task<IActionResult> CreateComplaint([FromBody] CallRegisterModel model)
    {

        var jsonPayload = JsonSerializer.Serialize(model);
        await _kafkaProducer.SendMessageAsync(
            topicName: "call-payload",  // separate topic for raw/unvalidated payloads
            key: Guid.NewGuid().ToString(),
            message: jsonPayload
        );
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                statusCode = -1,
                statusMsg = "Invalid request data"
            });

        var returnNo = await _service.CreateComplaintAsync(model);

        if (string.IsNullOrEmpty(returnNo))
        {
            return StatusCode(500, new
            {
                statusCode = -99,
                statusMsg = "Internal server error"
            });
        }

        return Ok(new
        {
            statusCode = 1,
            statusMsg = "Complaint saved successfully",
            returnNo
        });
    }

    // this for old data sync in mongo db
    [HttpGet("getticket/{ticketNo}")]
    public async Task<IActionResult> GetComplaintByTicketNo(string ticketNo)
    {
        var complaint = await _service.GetComplaintByTicketNoAsync(ticketNo);

        if (complaint == null)
            return NotFound($"Complaint with TicketNo '{ticketNo}' not found.");

        return Ok(complaint);
    }

    [HttpGet("getstatus/{status}/{subStatus}/{role}/{bid}")]
    public async Task<IActionResult> GetComplaintsByStatus(string status, string subStatus, int? role, int? bid)
    {
        var complaints = await _service.GetComplaintsByStatusAsync(status, subStatus, role, bid);

        if (complaints == null || complaints.Count == 0)
            return NotFound($"No complaints found with status '{status}' and substatus '{subStatus}'.");

        return Ok(complaints);
    }

    // ✅ Approve or Update Complaint
    [HttpPost("approveOrUpdate")]
    public async Task<IActionResult> ApproveOrUpdateComplaint([FromQuery] string ticketNo,[FromQuery] int userId,
        [FromQuery] string action,[FromQuery] string? remarks,[FromQuery] string? ascCode,[FromQuery] string? dropdownSelection,
        [FromQuery] string? complaintStatus,[FromQuery] string? enteredBy,[FromQuery] string? category)
    {
        var success = await _service.ApproveOrUpdateComplaintAsync(
            ticketNo, userId, action, remarks, ascCode, dropdownSelection, complaintStatus, enteredBy, category);

        if (!success)
            return BadRequest("Failed to approve or update complaint.");

        return Ok(new { Message = "Complaint approved/updated successfully.", TicketNo = ticketNo });
    }


    [HttpPost("UpdateComplaint/{ticketNo}")]
    public async Task<IActionResult> UpdateComplaint(string ticketNo, [FromBody] CallRegisterModel updatedModel)
    {
        var updatedComplaints = await _service.UpdateComplaintAsync(ticketNo, updatedModel);

        if (updatedComplaints == null || !updatedComplaints.Any())
            return NotFound(new { message = "Complaint not found or update failed.", ticketNo });

        return Ok(new
        {
            message = "Complaint updated successfully.",
            ticketNo,
            data = updatedComplaints
        });
    }



}
