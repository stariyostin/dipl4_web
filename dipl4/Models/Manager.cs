using System;
using System.Collections.Generic;

namespace dipl4.Models;

public partial class Manager
{
    public int Idmanager { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
