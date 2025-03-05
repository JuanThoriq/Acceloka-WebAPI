using FluentValidation;

namespace Acceloka.Features.Booking.Queries.GetBookedTicket
{
    public class GetBookedTicketQueryValidator : AbstractValidator<GetBookedTicketQuery>
    {
        public GetBookedTicketQueryValidator()
        {
            RuleFor(x => x.BookedTicketId)
                .NotEmpty().WithMessage("BookedTicketId cannot be empty.");
        }
    }
}
