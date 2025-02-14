namespace Acceloka.Models.Response
{
    public class BookedTicketDetailCategory
    {
        public string CategoryName { get; set; } = string.Empty;
        public int QtyPerCategory { get; set; }
        public List<BookedTicketItemDetail> Tickets { get; set; } = new();
    }
}
