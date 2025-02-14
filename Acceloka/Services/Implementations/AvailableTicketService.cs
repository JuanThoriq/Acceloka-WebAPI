using Acceloka.Entities;
using Acceloka.Models.Request;
using Acceloka.Models.Response;
using Acceloka.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Acceloka.Exceptions;


namespace Acceloka.Services.Implementations
{
    public class AvailableTicketService : IAvailableTicketService
    {
        private readonly AccelokaContext _db;

        public AvailableTicketService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<(List<AvailableTicketResponse>, int)> GetAvailableTickets(AvailableTicketParam param)
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
            if (!string.IsNullOrEmpty(param.CategoryName))
            {
                query = query.Where(x => x.CategoryName.Contains(param.CategoryName));
            }
            if (!string.IsNullOrEmpty(param.TicketCode))
            {
                query = query.Where(x => x.TicketCode.Contains(param.TicketCode));
            }
            if (!string.IsNullOrEmpty(param.TicketName))
            {
                query = query.Where(x => x.TicketName.Contains(param.TicketName));
            }
            if (param.Price.HasValue)
            {
                // harga <= yang diinput
                query = query.Where(x => x.Price <= param.Price.Value);
            }
            if (param.DateMin.HasValue)
            {
                query = query.Where(x => x.EventDate >= param.DateMin.Value);
            }
            if (param.DateMax.HasValue)
            {
                query = query.Where(x => x.EventDate <= param.DateMax.Value);
            }

            // 4. Sorting
            string orderBy = string.IsNullOrEmpty(param.OrderBy) ? "ticketCode" : param.OrderBy.ToLower();
            string orderState = string.IsNullOrEmpty(param.OrderState) ? "asc" : param.OrderState.ToLower();

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
            int totalRecords = await selectQuery.CountAsync();

            int pageSize = 10;
            int page = param.Page.HasValue && param.Page.Value > 0 ? param.Page.Value : 1;
            int skip = (page - 1) * pageSize;

            selectQuery = selectQuery.Skip(skip).Take(pageSize);

            // 7. Eksekusi query
            var result = await selectQuery.ToListAsync();
            if (!result.Any())
            {
                throw new InvalidValidationException("No tickets found for the given criteria.");
            }
            return (result, totalRecords);
        }
    }
}
