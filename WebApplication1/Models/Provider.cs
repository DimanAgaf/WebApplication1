using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Provider
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
