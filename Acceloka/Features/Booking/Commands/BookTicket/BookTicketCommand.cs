using MediatR;
using Acceloka.Models.Response;
using System.Collections.Generic;
using Acceloka.Models.Request;

namespace Acceloka.Features.Booking.Commands.BookTicket
{
    // Command: merepresentasikan request user untuk booking
    public class BookTicketCommand : IRequest<BookTicketResponse>
    {
        // Kita meniru structure BookTicketRequest yang sudah ada
        // Boleh menyalin property "Tickets" langsung ke sini,
        // atau merujuk ke BookTicketRequest (tergantung style).
        public List<BookTicketItem> Tickets { get; set; } = new();
    }
}
