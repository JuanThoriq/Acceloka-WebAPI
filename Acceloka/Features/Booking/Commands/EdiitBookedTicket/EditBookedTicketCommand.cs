using MediatR;
using Acceloka.Models.Response;
using Acceloka.Models.Request;
using System;
using System.Collections.Generic;

namespace Acceloka.Features.Booking.Commands.EditBookedTicket
{
    public class EditBookedTicketCommand : IRequest<List<EditBookedTicketResponse>>
    {
        public Guid BookedTicketId { get; set; }
        public List<EditBookedTicketItem> Tickets { get; set; } = new List<EditBookedTicketItem>();

        public EditBookedTicketCommand(Guid bookedTicketId, List<EditBookedTicketItem> tickets)
        {
            BookedTicketId = bookedTicketId;
            Tickets = tickets;
        }
    }
}
