using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models.Request
{
    public class EditBookedTicketRequest
    {
        [Required(ErrorMessage = "Tickets list cannot be empty.")]
        [MinLength(1, ErrorMessage = "At least one ticket is required.")]
        public List<EditBookedTicketItem> Tickets { get; set; } = new List<EditBookedTicketItem>();
    }
}