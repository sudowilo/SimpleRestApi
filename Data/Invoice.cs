using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleRestApi.Data;

public partial class Invoice
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public DateOnly? InvoiceDate { get; set; }

    public int? StatusId { get; set; }

    public long? TotalAmount { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual InvoiceStatus? Status { get; set; }
}
