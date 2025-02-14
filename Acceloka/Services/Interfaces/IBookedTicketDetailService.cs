using Acceloka.Models.Response;

namespace Acceloka.Services.Interfaces
{
    public interface IBookedTicketDetailService
    {
        Task<List<BookedTicketDetailCategory>> GetBookedTicketDetail(Guid bookedTicketId);
    }
}
