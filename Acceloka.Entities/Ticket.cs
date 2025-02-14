﻿using System;
using System.Collections.Generic;

namespace Acceloka.Entities;

public partial class Ticket
{
    public Guid TicketId { get; set; }

    public string TicketCode { get; set; } = null!;

    public string TicketName { get; set; } = null!;

    public DateTimeOffset EventDate { get; set; }

    public decimal Price { get; set; }

    public int Quota { get; set; }

    public int CategoryId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual ICollection<BookedTicket> BookedTickets { get; set; } = new List<BookedTicket>();

    public virtual Category Category { get; set; } = null!;
}
