using System;
using System.Collections.Generic;

namespace Acceloka.Entities;

public partial class BookedTicket
{
    public Guid BookedTicketId { get; set; }

    public Guid TicketId { get; set; }

    public int Quantity { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;
}
