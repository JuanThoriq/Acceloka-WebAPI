using FluentValidation;
using Acceloka.Models.Request;

namespace Acceloka.Features.Booking.Commands.EditBookedTicket
{
    public class EditBookedTicketCommandValidator
        : AbstractValidator<EditBookedTicketCommand>
    {
        public EditBookedTicketCommandValidator()
        {
            RuleFor(cmd => cmd.BookedTicketId)
                .NotEmpty().WithMessage("BookedTicketId cannot be empty.");

            RuleFor(cmd => cmd.Tickets)
                .NotNull().WithMessage("Tickets cannot be null.")
                .NotEmpty().WithMessage("Tickets cannot be empty.");

            RuleForEach(cmd => cmd.Tickets).ChildRules(ticket =>
            {
                ticket.RuleFor(x => x.TicketCode)
                    .NotEmpty().WithMessage("TicketCode is required.");

                ticket.RuleFor(x => x.Quantity)
                    .GreaterThanOrEqualTo(1).WithMessage("Quantity must be >= 1.");
            });
        }
    }
}
