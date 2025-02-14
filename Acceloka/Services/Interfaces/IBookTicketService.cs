using Acceloka.Models.Request;
using Acceloka.Models.Response;


namespace Acceloka.Services.Interfaces
{
    public interface IBookTicketService
    {
        Task<BookTicketResponse> BookTicket(BookTicketRequest request);
    }
}
