using System;
using System.Collections.Generic;

namespace GymFit.Models;

public partial class Review
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int RatingValue { get; set; }

    public string? Comment { get; set; }

    public Guid? ClientId { get; set; }

    public Guid? TrainerId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Trainer? Trainer { get; set; }
}
