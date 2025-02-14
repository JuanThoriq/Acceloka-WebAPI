using Acceloka.Entities;
using Acceloka.Exceptions;
using Acceloka.Models.Request;
using Acceloka.Models.Response;
using Acceloka.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Acceloka.Services.Implementations
{
    public class EditBookedTicketService : IEditBookedTicketService
    {
        private readonly AccelokaContext _db;

        public EditBookedTicketService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<List<EditBookedTicketResponse>> EditBookedTicket(Guid bookedTicketId, EditBookedTicketRequest request)
        {
            // 1. Pastikan BookedTicketId ada. 
            //    Karena 1 booking bisa banyak baris, kita ambil semua baris
            var bookedRows = await _db.BookedTickets
                .Include(b => b.Ticket)
                .ThenInclude(t => t.Category)   // Jika kita punya navigation property Category
                .Where(b => b.BookedTicketId == bookedTicketId)
                .ToListAsync();

            if (!bookedRows.Any())
            {
                throw new InvalidValidationException("The specified BookedTicketId is not registered.");
            }

            // 2. Siapkan list response
            var responseList = new List<EditBookedTicketResponse>();

            // 3. Loop setiap item yang di-update
            foreach (var item in request.Tickets)
            {
                // a. Cari baris BookedTickets berdasarkan ticketCode
                var row = bookedRows.FirstOrDefault(br => br.Ticket.TicketCode == item.TicketCode);
                if (row == null)
                {
                    throw new InvalidValidationException ($"Ticket code {item.TicketCode} is not registered in this booking.");
                }

                // b. Validasi quantity minimal 1
                if (item.Quantity < 1)
                {
                    throw new InvalidValidationException($"The quantity must be at least 1 for ticket code {item.TicketCode}.");
                }

                // c. Cek sisa quota di table Tickets
                var ticketInDb = row.Ticket;
                if (item.Quantity > ticketInDb.Quota)
                {
                    throw new InvalidValidationException($"The requested quantity {item.Quantity} exceeds the remaining quota {ticketInDb.Quota}.");
                }

                // d. Update BookedTickets
                row.Quantity = item.Quantity;

                // e. Buat response item
                responseList.Add(new EditBookedTicketResponse
                {
                    TicketCode = ticketInDb.TicketCode,
                    TicketName = ticketInDb.TicketName,
                    Quantity = item.Quantity,
                    CategoryName = ticketInDb.Category.CategoryName
                });
            }

            // 4. Save changes
            await _db.SaveChangesAsync();

            return responseList;
        }
    }
}
