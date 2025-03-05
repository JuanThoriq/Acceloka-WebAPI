using MediatR;
using Acceloka.Models.Response;
using System;
using System.Collections.Generic;

namespace Acceloka.Features.Booking.Queries.GetBookedTicket
{
    public class GetBookedTicketQuery : IRequest<List<BookedTicketDetailCategory>>
    {
        public Guid BookedTicketId { get; set; }

        public GetBookedTicketQuery(Guid bookedTicketId)
        {
            BookedTicketId = bookedTicketId;
        }
    }
}
