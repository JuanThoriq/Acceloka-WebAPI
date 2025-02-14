using Acceloka.Exceptions;
using Acceloka.Models.Request;
using Acceloka.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IAvailableTicketService _availableTicketService;
        private readonly ILogger<TicketController> _logger; // Logger untuk TicketController


        public TicketController(IAvailableTicketService availableTicketService, ILogger<TicketController> logger)
        {
            _availableTicketService = availableTicketService;
            _logger = logger;
        }

        // GET: api/<TicketController>
        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> GetAvailableTicket([FromQuery] AvailableTicketParam param)
        {
            // Log informasi awal pemanggilan endpoint
            _logger.LogInformation("Received request to GET available tickets with param: {@param}", param);
            try
            {
                var (datas, totalRecords) = await _availableTicketService.GetAvailableTickets(param);

                // Log sukses
                _logger.LogInformation(
                    "Successfully retrieved {count} tickets. Total records: {totalRecords}",
                    datas.Count,
                    totalRecords
                );

                // Return 200 OK
                return Ok(new
                {
                    tickets = datas,
                    totalTickets = totalRecords // atau totalRecords jika disimpan
                });
            }
            catch (InvalidValidationException ex)
            {
                _logger.LogWarning(ex,
                   "Validation error occurred while getting tickets with param: {@param}",
                   param
               );

                // RFC 7807
                return Problem(
                    title: "Error Getting Ticket",
                    detail: ex.Message,
                    statusCode: 404
                );
            }catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception while getting tickets with param: {@param}",
                    param
                );

                // RFC 7807
                return Problem(
                    title: "Internal Server Error",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }
    }
}
