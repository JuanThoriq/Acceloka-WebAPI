using MediatR;
using Acceloka.Models.Response;
using System;

namespace Acceloka.Features.Booking.Commands.RevokeTicket
{
    public class RevokeTicketCommand : IRequest<RevokeTicketResponse>
    {
        public Guid BookedTicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public int Qty { get; set; }

        public RevokeTicketCommand(Guid bookedTicketId, string ticketCode, int qty)
        {
            BookedTicketId = bookedTicketId;
            TicketCode = ticketCode;
            Qty = qty;
        }
    }
}
