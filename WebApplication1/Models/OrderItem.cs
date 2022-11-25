using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = null!;

    [ValidateNever]
    public virtual Order Order { get; set; } = null!;
}
