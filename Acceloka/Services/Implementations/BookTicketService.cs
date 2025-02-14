using Acceloka.Entities;
using Acceloka.Exceptions;
using Acceloka.Models.Request;
using Acceloka.Models.Response;
using Acceloka.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Acceloka.Services.Implementations
{
    public class BookTicketService : IBookTicketService
    {
        private readonly AccelokaContext _db;

        public BookTicketService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<BookTicketResponse> BookTicket(BookTicketRequest request)
        {
            // 1. Validasi input
            if (request.Tickets == null || request.Tickets.Count == 0)
            {
                throw new InvalidValidationException("The list of requested tickets is empty.");
            }

            // 2. Ambil data ticketCode yang di-request
            var ticketCodes = request.Tickets.Select(x => x.TicketCode).Distinct().ToList();

            // 3. Ambil data tiket dari DB
            var ticketsInDb = await _db.Tickets
                .Include(t => t.Category) // if we have a navigation property Category
                .Where(t => ticketCodes.Contains(t.TicketCode))
                .ToListAsync();

            // a. Cek apakah semua ticketCode valid
            if (ticketsInDb.Count != ticketCodes.Count)
            {
                throw new InvalidValidationException("Some ticket codes are not registered.");
            }

            // b. Waktu booking
            var now = DateTimeOffset.UtcNow;

            // c. Lakukan validasi: quota habis, quantity melebihi quota, eventDate <= booking date
            foreach (var item in request.Tickets)
            {
                var dbTicket = ticketsInDb.FirstOrDefault(t => t.TicketCode == item.TicketCode);
                if (dbTicket == null)
                {
                    throw new InvalidValidationException($"Ticket code {item.TicketCode} is not registered.");
                }

                if (dbTicket.Quota <= 0)
                {
                    throw new InvalidValidationException($"Ticket {dbTicket.TicketCode} has no remaining quota.");
                }

                if (item.Quantity > dbTicket.Quota)
                {
                    throw new InvalidValidationException($"Quantity {item.Quantity} exceeds the remaining quota {dbTicket.Quota} for ticket {dbTicket.TicketCode}.");
                }

                if (dbTicket.EventDate <= now)
                {
                    throw new InvalidValidationException($"Event date {dbTicket.EventDate} must be greater than the booking date {now}.");
                }
            }

            // 4. Simpan data ke BookedTickets
            //    Buat satu BookedTicketId untuk satu transaksi
            var bookedTicketId = Guid.NewGuid();

            // Siapkan list untuk menghitung summary
            var responseItems = new List<(string categoryName, string ticketCode, string ticketName, decimal totalPrice)>();

            foreach (var item in request.Tickets)
            {
                var dbTicket = ticketsInDb.First(t => t.TicketCode == item.TicketCode);

                // Insert baris BookedTickets
                var newBooked = new BookedTicket
                {
                    BookedTicketId = bookedTicketId,
                    TicketId = dbTicket.TicketId,
                    Quantity = item.Quantity,
                    // CreatedAt dan UpdatedAt sudah memiliki default value pada Database
                };
                _db.BookedTickets.Add(newBooked);

                // Hitung total price per item
                decimal totalPrice = dbTicket.Price * item.Quantity;

                // Kumpulkan data untuk response
                responseItems.Add((dbTicket.Category.CategoryName, dbTicket.TicketCode, dbTicket.TicketName, totalPrice));

                // Update quota di Tickets
                dbTicket.Quota -= item.Quantity;
            }

            await _db.SaveChangesAsync();

            // 5. Group by categoryName & Summaries
            var grouped = responseItems
                .GroupBy(x => x.categoryName)
                .Select(g => new CategorySummary
                {
                    CategoryName = g.Key,
                    SummaryPrice = g.Sum(x => x.totalPrice),
                    Tickets = g.Select(i => new BookedTicketItemResponse
                    {
                        TicketCode = i.ticketCode,
                        TicketName = i.ticketName,
                        Price = i.totalPrice
                    }).ToList()
                })
                .ToList();

            // Hitung total summary all categories
            decimal totalAll = grouped.Sum(c => c.SummaryPrice);

            // 6. Buat response final
            var response = new BookTicketResponse
            {
                PriceSummary = totalAll,
                TicketsPerCategories = grouped
            };

            return response;
        }
    }

}
