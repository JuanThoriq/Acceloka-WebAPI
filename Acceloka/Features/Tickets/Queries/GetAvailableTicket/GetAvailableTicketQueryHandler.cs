using MediatR;
using Acceloka.Models.Response;
using Acceloka.Entities;
using Microsoft.EntityFrameworkCore;
using Acceloka.Exceptions;

namespace Acceloka.Features.Tickets.Queries.GetAvailableTicket
{
    public class GetAvailableTicketQueryHandler
        : IRequestHandler<GetAvailableTicketQuery, (List<AvailableTicketResponse>, int)>
    {
        private readonly AccelokaContext _db;

        public GetAvailableTicketQueryHandler(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<(List<AvailableTicketResponse>, int)> Handle(
            GetAvailableTicketQuery request,
            CancellationToken cancellationToken)
        {
           // 1. Melakukan JOIN table Tickets dan Categories
            //    Menggunakan LINQ query syntax sesuai aturan (boleh query syntax untuk join)
            var query =
                from t in _db.Tickets
                join c in _db.Categories on t.CategoryId equals c.CategoryId
                select new
                {
                    t.TicketId,
                    t.TicketCode,
                    t.TicketName,
                    t.EventDate,
                    t.Price,
                    t.Quota,
                    c.CategoryName
                };

            // 2. Filter Quota > 0 (menampilkan tiket yang masih tersedia)
            query = query.Where(x => x.Quota > 0);

            // 3. Searching by kolom
            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                query = query.Where(x => x.CategoryName.Contains(request.CategoryName));
            }
            if (!string.IsNullOrEmpty(request.TicketCode))
            {
                query = query.Where(x => x.TicketCode.Contains(request.TicketCode));
            }
            if (!string.IsNullOrEmpty(request.TicketName))
            {
                query = query.Where(x => x.TicketName.Contains(request.TicketName));
            }
            if (request.Price.HasValue)
            {
                // harga <= yang diinput
                query = query.Where(x => x.Price <= request .Price.Value);
            }
            if (request.DateMin.HasValue)
            {
                query = query.Where(x => x.EventDate >= request.DateMin.Value);
            }
            if (request.DateMax.HasValue)
            {
                query = query.Where(x => x.EventDate <= request.DateMax.Value);
            }

            // 4. Sorting
            string orderBy = string.IsNullOrEmpty(request.OrderBy) ? "ticketCode" : request.OrderBy.ToLower();
            string orderState = string.IsNullOrEmpty(request.OrderState) ? "asc" : request.OrderState.ToLower();

            // By default order by ticketCode ascending
            // if user menuliskan orderBy = "categoryname", "eventdate", "price", "ticketname", dsb.
            query = orderBy switch
            {
                "categoryname" => (orderState == "desc")
                                  ? query.OrderByDescending(x => x.CategoryName)
                                  : query.OrderBy(x => x.CategoryName),
                "ticketname" => (orderState == "desc")
                                  ? query.OrderByDescending(x => x.TicketName)
                                  : query.OrderBy(x => x.TicketName),
                "eventdate" => (orderState == "desc")
                                  ? query.OrderByDescending(x => x.EventDate)
                                  : query.OrderBy(x => x.EventDate),
                "price" => (orderState == "desc")
                                  ? query.OrderByDescending(x => x.Price)
                                  : query.OrderBy(x => x.Price),
                _ => (orderState == "desc")
                                  ? query.OrderByDescending(x => x.TicketCode)
                                  : query.OrderBy(x => x.TicketCode)
            };

            // 5. Projection ke model/DTO response
            var selectQuery = query.Select(x => new AvailableTicketResponse
            {
                CategoryName = x.CategoryName,
                TicketCode = x.TicketCode,
                TicketName = x.TicketName,
                EventDate = x.EventDate,
                Price = x.Price,
                Quota = x.Quota
            });

            // 6. Bonus: pagination 10 item per page dan total records
            int totalRecords = await selectQuery.CountAsync(cancellationToken);

            int pageSize = 10;
            int page = request.Page <= 0 ? 1 : request.Page;
            int skip = (page - 1) * pageSize;

            selectQuery = selectQuery.Skip(skip).Take(pageSize);

            // 7. Eksekusi query
            var result = await selectQuery.ToListAsync(cancellationToken);
            if (!result.Any())
            {
                throw new InvalidValidationException("No tickets found for the given criteria.");
            }
            return (result, totalRecords);
        }
    }
}
