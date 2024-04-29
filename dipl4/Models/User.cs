using System;
using System.Collections.Generic;

namespace dipl4.Models;

public partial class User
{
    public int Iduser { get; set; }

    public string ClientName { get; set; } = null!;

    public string ClientContact { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
