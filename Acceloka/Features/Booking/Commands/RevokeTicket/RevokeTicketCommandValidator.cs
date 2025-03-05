using FluentValidation;
using Acceloka.Features.Booking.Commands.RevokeTicket;

namespace Acceloka.Features.Booking.Commands.RevokeTicket
{
    public class RevokeTicketCommandValidator : AbstractValidator<RevokeTicketCommand>
    {
        public RevokeTicketCommandValidator()
        {
            RuleFor(x => x.BookedTicketId)
                .NotEmpty().WithMessage("BookedTicketId cannot be empty.");

            RuleFor(x => x.TicketCode)
                .NotEmpty().WithMessage("TicketCode is required.");

            RuleFor(x => x.Qty)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
