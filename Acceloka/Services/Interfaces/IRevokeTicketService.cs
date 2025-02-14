using Acceloka.Models.Response;

namespace Acceloka.Services.Interfaces
{
    public interface IRevokeTicketService
    {
        Task<RevokeTicketResponse> RevokeTicket(Guid bookedTicketId, string ticketCode, int qty);
    }
}
