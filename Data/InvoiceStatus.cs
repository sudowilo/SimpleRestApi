using System;
using System.Collections.Generic;

namespace SimpleRestApi.Data;

public partial class InvoiceStatus
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
