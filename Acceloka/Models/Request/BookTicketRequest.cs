using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models.Request
{
    public class BookTicketRequest
    {
        [Required(ErrorMessage = "Tickets list cannot be empty.")]
        [MinLength(1, ErrorMessage = "At least one ticket is required.")]
        public List<BookTicketItem> Tickets { get; set; } = new List<BookTicketItem>();
    }
}
