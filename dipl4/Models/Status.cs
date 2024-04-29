using System;
using System.Collections.Generic;

namespace dipl4.Models;

public partial class Status
{
    public int Idstatus { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
