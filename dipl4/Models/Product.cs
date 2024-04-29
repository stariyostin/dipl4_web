using System;
using System.Collections.Generic;

namespace dipl4.Models;

public partial class Product
{
    public int Idproduct { get; set; }

    public string Name { get; set; } = null!;

    public string TotalAmount { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
