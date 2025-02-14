using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models.Request
{
    public class EditBookedTicketItem
    {
        [Required(ErrorMessage = "TicketCode is required.")]
        public string TicketCode { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
