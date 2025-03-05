using Acceloka.Exceptions;
using Acceloka.Features.Tickets.Queries.GetAvailableTicket;
using Acceloka.Models.Request;
using Acceloka.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TicketController> _logger; // Logger untuk TicketController


        public TicketController(IMediator mediator, ILogger<TicketController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: api/<TicketController>
        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> GetAvailableTicket([FromQuery] GetAvailableTicketQuery query)
        {
            // Log informasi awal pemanggilan endpoint
            _logger.LogInformation("Received request to GET available tickets with param: {@param}", query);
            try
            {
                var (datas, totalRecords) = await _mediator.Send(query);

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
                   query
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
                    query
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
