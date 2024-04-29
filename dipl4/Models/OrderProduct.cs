using System;
using System.Collections.Generic;

namespace dipl4.Models;

public partial class OrderProduct
{
    public int IdordProd { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public string? Amount { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
