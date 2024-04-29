using System;
using System.Collections.Generic;

namespace dipl4.Models;

public partial class Order
{
    public int Idorder { get; set; }

    public int UserId { get; set; }

    public int StatusId { get; set; }

    public DateTime DateOfCreate { get; set; }

    public DateTime DeadLine { get; set; }

    public int? ManagerId { get; set; }

    public virtual Manager? Manager { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual Status Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
