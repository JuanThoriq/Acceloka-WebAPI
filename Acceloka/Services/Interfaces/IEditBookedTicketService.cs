using Acceloka.Models.Request;
using Acceloka.Models.Response;

namespace Acceloka.Services.Interfaces
{
    public interface IEditBookedTicketService
    {
        Task<List<EditBookedTicketResponse>> EditBookedTicket(Guid bookedTicketId, EditBookedTicketRequest request);
    }
}
