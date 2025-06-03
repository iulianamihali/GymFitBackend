using System;
using System.Collections.Generic;

namespace GymFit.Models;

public partial class Schedule
{
    public Guid Id { get; set; }

    public int? Day { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public Guid? TrainerId { get; set; }

    public virtual Trainer? Trainer { get; set; }
}
