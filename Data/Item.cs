using System;
using System.Collections.Generic;

namespace SimpleRestApi.Data;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
