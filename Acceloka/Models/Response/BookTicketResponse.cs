namespace Acceloka.Models.Response
{
    public class BookTicketResponse
    {
        public decimal PriceSummary { get; set; }       // Total semua kategori
        public List<CategorySummary> TicketsPerCategories { get; set; } = new();
    }
}
