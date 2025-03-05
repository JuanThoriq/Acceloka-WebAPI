using MediatR;
using Acceloka.Entities;
using Acceloka.Exceptions;
using Acceloka.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Features.Booking.Queries.GetBookedTicket
{
    public class GetBookedTicketQueryHandler
        : IRequestHandler<GetBookedTicketQuery, List<BookedTicketDetailCategory>>
    {
        private readonly AccelokaContext _db;

        public GetBookedTicketQueryHandler(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<List<BookedTicketDetailCategory>> Handle(
            GetBookedTicketQuery request,
            CancellationToken cancellationToken)
        {
            var bookedTicketId = request.BookedTicketId;

            // 1. Ambil baris BookedTickets yang sesuai BookedTicketId, join ke Tickets & Categories
            //    Menggunakan LINQ query syntax (boleh, karena ada join)
            var bookedRows = await (
                from b in _db.BookedTickets
                join t in _db.Tickets on b.TicketId equals t.TicketId
                join c in _db.Categories on t.CategoryId equals c.CategoryId
                where b.BookedTicketId == bookedTicketId
                select new
                {
                    b.Quantity,
                    t.TicketCode,
                    t.TicketName,
                    t.EventDate,
                    c.CategoryName
                }
            ).ToListAsync(cancellationToken);

            // 2. Validasi: jika tidak ada baris, berarti BookedTicketId tidak terdaftar
            if (!bookedRows.Any())
            {
                throw new InvalidValidationException("The specified BookedTicketId is not registered.");
            }

            // 3. Group by categoryName
            var grouped = bookedRows
                .GroupBy(x => x.CategoryName)
                .Select(g => new BookedTicketDetailCategory
                {
                    CategoryName = g.Key,
                    QtyPerCategory = g.Sum(x => x.Quantity),
                    Tickets = g.Select(i => new BookedTicketItemDetail
                    {
                        TicketCode = i.TicketCode,
                        TicketName = i.TicketName,
                        Quantity = i.Quantity,
                        EventDate = i.EventDate
                    }).ToList()
                })
                .ToList();

            return grouped;
        }
    }
}
