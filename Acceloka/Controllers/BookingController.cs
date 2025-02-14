using Acceloka.Entities;
using Acceloka.Exceptions;
using Acceloka.Models.Request;
using Acceloka.Services.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookTicketService _bookTicketService;
        private readonly IBookedTicketDetailService _bookedTicketDetailService;
        private readonly IRevokeTicketService _revokeTicketService;
        private readonly IEditBookedTicketService _editBookedTicketService;
        private readonly ILogger<BookingController> _logger; // Injeksi logger

        // DI Services
        public BookingController( IBookTicketService bookTicketService, IBookedTicketDetailService bookedTicketDetailService, IRevokeTicketService revokeTicketService, IEditBookedTicketService editBookedTicketService, ILogger<BookingController> logger)
        {
            _bookTicketService = bookTicketService;
            _bookedTicketDetailService = bookedTicketDetailService;
            _revokeTicketService = revokeTicketService;
            _editBookedTicketService = editBookedTicketService;
            _logger = logger;
        }

        // GET api/v1/get-booked-ticket/{bookedTicketId}
        [HttpGet("get-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> GetBookedTicket(Guid bookedTicketId)
        {
            _logger.LogInformation("Received GET request for booked ticket with ID: {bookedTicketId}", bookedTicketId);

            try
            {
                var data = await _bookedTicketDetailService.GetBookedTicketDetail(bookedTicketId);
                _logger.LogInformation("Successfully retrieved booked ticket details for ID: {bookedTicketId}", bookedTicketId);
                // Sukses -> return 200 OK
                return Ok(data);
            }
            catch (InvalidValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error while retrieving booked ticket with ID: {bookedTicketId}", bookedTicketId);
                // RFC 7807
                return Problem(
                    title: "Error Getting Booked Ticket",
                    detail: ex.Message,
                    statusCode: 404
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while retrieving booked ticket with ID: {bookedTicketId}", bookedTicketId);
                // RFC 7807
                return Problem(
                    title: "Internal Server Error",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }

        // POST api/v1/book-ticket
        [HttpPost("book-ticket")]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketRequest request)
        {
            _logger.LogInformation("Received POST request for booking tickets: {@Request}", request);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for booking request: {@ModelState}", ModelState);
                    // RFC 7807
                    return Problem(
                        title: "Bad Request",
                        detail: "Invalid Input Format",
                        statusCode: 400
                    );
                }

                var result = await _bookTicketService.BookTicket(request);
                _logger.LogInformation("Successfully booked tickets.");
                // Sukses -> 201 Created atau 200 OK
                return Ok(result);
            }
            catch (InvalidValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error occurred while booking tickets.");
                // RFC 7807
                return Problem(
                    title: "Error Booking Ticket",
                    detail: ex.Message,
                    statusCode: 404
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while booking tickets.");
                // RFC 7807
                return Problem(
                    title: "Internal Server Error",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }

        //// PUT api/v1/edit-booked-ticket/{bookedTicketId}
        [HttpPut("edit-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> EditBookedTicket(Guid bookedTicketId, [FromBody] EditBookedTicketRequest request)
        {
            _logger.LogInformation("Received PUT request to edit booked ticket with ID: {bookedTicketId} and request data: {@Request}", bookedTicketId, request);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for editing booked ticket with ID: {bookedTicketId}. ModelState: {@ModelState}", bookedTicketId, ModelState);
                    // RFC 7807
                    return Problem(
                        title: "Bad Request",
                        detail: "Invalid Input Format",
                        statusCode: 400
                    );
                }

                var result = await _editBookedTicketService.EditBookedTicket(bookedTicketId, request);
                _logger.LogInformation("Successfully edited booked ticket with ID: {bookedTicketId}", bookedTicketId);
                return Ok(result);
            }
            catch (InvalidValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error occurred while editing booked ticket with ID: {bookedTicketId}", bookedTicketId);
                // RFC 7807
                return Problem(
                    title: "Error Editing Booked Ticket",
                    detail: ex.Message,
                    statusCode: 404
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while editing booked ticket with ID: {bookedTicketId}", bookedTicketId);
                // RFC 7807
                return Problem(
                    title: "Internal Server Error",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }

        // DELETE: api/v1/revoke-ticket/{bookedTicketId}/{ticketCode}/{qty}
        [HttpDelete("revoke-ticket/{bookedTicketId}/{ticketCode}/{qty}")]
        public async Task<IActionResult> RevokeTicket(Guid bookedTicketId, string ticketCode, int qty)
        {
            _logger.LogInformation("Received DELETE request to revoke ticket. BookedTicketId: {bookedTicketId}, TicketCode: {ticketCode}, Quantity: {qty}", bookedTicketId, ticketCode, qty);
            try
            {
                var data = await _revokeTicketService.RevokeTicket(bookedTicketId, ticketCode, qty);
                _logger.LogInformation("Successfully revoked ticket. BookedTicketId: {bookedTicketId}, TicketCode: {ticketCode}", bookedTicketId, ticketCode);
                return Ok(data);
            }
            catch (InvalidValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error occurred while revoking ticket. BookedTicketId: {bookedTicketId}, TicketCode: {ticketCode}", bookedTicketId, ticketCode);
                // RFC 7807
                return Problem(
                    title: "Error Revoking Ticket",
                    detail: ex.Message,
                    statusCode: 404
                );
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while revoking ticket. BookedTicketId: {bookedTicketId}, TicketCode: {ticketCode}", bookedTicketId, ticketCode);
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
