using System;
using System.Collections.Generic;

namespace SimpleRestApi.Data;

public partial class Invoice
{
    public int Id { get; set; }

    public int InvoiceNumber { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public int StatusId { get; set; }

    public long? TotalAmount { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual InvoiceStatus Status { get; set; } = null!;
}
