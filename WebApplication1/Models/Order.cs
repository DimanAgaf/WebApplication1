using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Order
{
    public int Id { get; set; }

    public string Number { get; set; } = null!;

    public DateTime Date { get; set; }

    public int ProviderId { get; set; }

    [ValidateNever]
    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

    [ValidateNever]
    public virtual Provider Provider { get; set; } = null!;
}
