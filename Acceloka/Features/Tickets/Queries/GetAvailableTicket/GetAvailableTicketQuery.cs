using MediatR;
using Acceloka.Models.Response;
using System;

namespace Acceloka.Features.Tickets.Queries.GetAvailableTicket
{
    public class GetAvailableTicketQuery : IRequest<(List<AvailableTicketResponse> datas, int totalRecords)>
    {
        // Properti-properti yang sebelumnya ada di AvailableTicketParam
        public string? CategoryName { get; set; }
        public string? TicketCode { get; set; }
        public string? TicketName { get; set; }
        public decimal? Price { get; set; }
        public DateTimeOffset? DateMin { get; set; }
        public DateTimeOffset? DateMax { get; set; }
        public string? OrderBy { get; set; } // e.g. "ticketCode", "price", etc.
        public string? OrderState { get; set; } // "asc" or "desc"
        public int Page { get; set; } = 1; // default page = 1
    }
}
