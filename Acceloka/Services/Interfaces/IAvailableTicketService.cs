using Acceloka.Models.Request;
using Acceloka.Models.Response;

namespace Acceloka.Services.Interfaces
{
    public interface IAvailableTicketService
    {
        Task<(List<AvailableTicketResponse>, int)> GetAvailableTickets(AvailableTicketParam param);
    }
}
