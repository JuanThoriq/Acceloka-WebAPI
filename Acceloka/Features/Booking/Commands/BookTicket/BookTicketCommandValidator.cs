using FluentValidation;

namespace Acceloka.Features.Booking.Commands.BookTicket
{
    public class BookTicketCommandValidator : AbstractValidator<BookTicketCommand>
    {
        public BookTicketCommandValidator()
        {
            // Minimal satu item
            RuleFor(cmd => cmd.Tickets)
                .NotNull().WithMessage("Tickets list cannot be null.")
                .NotEmpty().WithMessage("Tickets list cannot be empty.");

            RuleForEach(cmd => cmd.Tickets).ChildRules(ticket =>
            {
                // TicketCode required
                ticket.RuleFor(x => x.TicketCode)
                    .NotEmpty().WithMessage("TicketCode is required.");

                // Quantity >= 1
                ticket.RuleFor(x => x.Quantity)
                    .GreaterThanOrEqualTo(1).WithMessage("Quantity must be >= 1.");
            });
        }
    }
}
