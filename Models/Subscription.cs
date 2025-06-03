using System;
using System.Collections.Generic;

namespace GymFit.Models;

public partial class Subscription
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public DateTime ActivationDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public Guid? UserId { get; set; }

    public virtual User? User { get; set; }
}
