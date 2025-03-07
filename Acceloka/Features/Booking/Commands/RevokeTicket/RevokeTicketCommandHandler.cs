using MediatR;
using Acceloka.Entities;
using Acceloka.Exceptions;
using Acceloka.Models.Response;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Acceloka.Features.Booking.Commands.RevokeTicket
{
    public class RevokeTicketCommandHandler : IRequestHandler<RevokeTicketCommand, RevokeTicketResponse>
    {
        private readonly AccelokaContext _db;

        public RevokeTicketCommandHandler(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<RevokeTicketResponse> Handle(RevokeTicketCommand command, CancellationToken cancellationToken)
        {
            // 1. Cari row BookedTickets (dengan join ke Tickets & Categories) 
            //    agar kita bisa menampilkan ticketName, categoryName, dsb.
            var row = await (
                from b in _db.BookedTickets
                join t in _db.Tickets on b.TicketId equals t.TicketId
                join c in _db.Categories on t.CategoryId equals c.CategoryId
                where b.BookedTicketId == command.BookedTicketId
                      && t.TicketCode == command.TicketCode
                select new
                {
                    b,           // row BookedTickets
                    t.TicketCode,
                    t.TicketName,
                    c.CategoryName
                }
            ).FirstOrDefaultAsync(cancellationToken);

            if (row == null)
            {
                // BookedTicketId tidak terdaftar atau ticketCode tidak ditemukan di booking ini
                throw new InvalidValidationException("Booking data not found or ticket code is not registered.");
            }

            // 2. Validasi qty: quantity yang diminta untuk direvoke tidak boleh lebih besar dari yang sudah dipesan
            if (command.Qty > row.b.Quantity)
            {
                throw new InvalidValidationException($"The requested quantity {command.Qty} exceeds the previously booked quantity {row.b.Quantity}.");
            }

            if (command.Qty <= 0)
            {
                throw new InvalidValidationException("Quantity must be greater than zero.");
            }

            // 3. Update kolom Quantity dan Quota
            row.b.Quantity -= command.Qty;
            int sisaQuantity = row.b.Quantity;

            var ticket = await _db.Tickets.FindAsync(new object[] { row.b.TicketId }, cancellationToken);
            if (ticket != null)
            {
                // Menambahkan quota kembali
                ticket.Quota += command.Qty;
                // EF Core sudah tracking ticket, jadi nanti di SaveChangesAsync akan update

            }

            if (sisaQuantity <= 0)
            {
                // 4. Hapus baris jika Quantity 0
                _db.BookedTickets.Remove(row.b);
            }

            await _db.SaveChangesAsync(cancellationToken);

            // 5. Cek apakah BookedTicketId masih punya baris lain?
            bool stillExists = await _db.BookedTickets
                .AnyAsync(bk => bk.BookedTicketId == command.BookedTicketId);

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
                CategoryName = row.CategoryName,
            };

            return response;
        }
    }
}
