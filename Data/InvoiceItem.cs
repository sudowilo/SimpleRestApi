using System;
using System.Collections.Generic;

namespace SimpleRestApi.Data;

public partial class InvoiceItem
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public long Fee { get; set; }

    public long? TotalPrice { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual Item? Item { get; set; }
}
