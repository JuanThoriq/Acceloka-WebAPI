using FluentValidation;

namespace Acceloka.Features.Tickets.Queries.GetAvailableTicket
{
    public class GetAvailableTicketQueryValidator : AbstractValidator<GetAvailableTicketQuery>
    {
        public GetAvailableTicketQueryValidator()
        {
            // Contoh validasi Page minimal 1
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be >= 1.");

            // Bisa tambahkan validasi lain, misal Price >= 0, dsb.
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Price.HasValue)
                .WithMessage("Price cannot be negative.");
        }
    }
}
