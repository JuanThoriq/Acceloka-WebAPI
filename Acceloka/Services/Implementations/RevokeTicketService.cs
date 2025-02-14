using Acceloka.Entities;
using Acceloka.Exceptions;
using Acceloka.Models.Response;
using Acceloka.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Acceloka.Services.Implementations
{
    public class RevokeTicketService : IRevokeTicketService
    {
        private readonly AccelokaContext _db;

        public RevokeTicketService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<RevokeTicketResponse> RevokeTicket(Guid bookedTicketId, string ticketCode, int qty)
        {
            // 1. Cari row BookedTickets (dengan join ke Tickets & Categories) 
            //    agar kita bisa menampilkan ticketName, categoryName, dsb.
            var row = await (
                from b in _db.BookedTickets
                join t in _db.Tickets on b.TicketId equals t.TicketId
                join c in _db.Categories on t.CategoryId equals c.CategoryId
                where b.BookedTicketId == bookedTicketId
                      && t.TicketCode == ticketCode
                select new
                {
                    b,           // row BookedTickets
                    t.TicketCode,
                    t.TicketName,
                    c.CategoryName
                }
            ).FirstOrDefaultAsync();

            if (row == null)
            {
                // BookedTicketId tidak terdaftar atau ticketCode tidak ditemukan di booking ini
                throw new InvalidValidationException("Booking data not found or ticket code is not registered.");
            }

            // 2. Validasi qty
            if (qty > row.b.Quantity)
            {
                throw new InvalidValidationException($"The requested quantity {qty} exceeds the previously booked quantity {row.b.Quantity}.");
            }

            // 3. Update kolom Quantity dan kolom Quota pada Table Tickets
            row.b.Quantity -= qty;

            // sisaQuantity -> row.b.Quantity setelah pengurangan
            int sisaQuantity = row.b.Quantity;

            var ticket = await _db.Tickets.FindAsync(row.b.TicketId);
            if (ticket != null)
            {
                // menambah Quota lagi
                ticket.Quota += qty;
                // EF Core sudah tracking ticket, jadi nanti di SaveChangesAsync akan update
            }

            if (sisaQuantity <= 0)
            {
                // 4. Hapus baris jika Quantity = 0
                _db.BookedTickets.Remove(row.b);
            }

            await _db.SaveChangesAsync();

            // 5. Cek apakah BookedTicketId masih punya baris lain?
            bool stillExists = await _db.BookedTickets
                .AnyAsync(bk => bk.BookedTicketId == bookedTicketId);

            if (!stillExists)
            {
                // Soal: "Jika semua kode ticket pada BookedTicketId tersebut sudah 0, hapus seluruh row"
                // Karena kita sudah hapus baris ini (if quantity=0),
                // jika tidak ada baris lain => berarti booking habis.
                // Tidak ada "header" yang perlu dihapus kecuali
                // jika kita punya table BookingHeader terpisah.
                // Di sini, tak ada lagi baris, jadi booking itu effectively done.
                // Kode ini saya siapkan apabila ada logika untuk menghapus BookedTicketId di table lain hehe
            }

            // 6. Return response
            var response = new RevokeTicketResponse
            {
                TicketCode = row.TicketCode,
                TicketName = row.TicketName,
                Quantity = sisaQuantity > 0 ? sisaQuantity : 0,
                CategoryName = row.CategoryName
            };
            return response;
        }
    }
}
