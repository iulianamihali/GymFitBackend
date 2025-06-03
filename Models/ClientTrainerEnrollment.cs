using System;
using System.Collections.Generic;

namespace GymFit.Models;

public partial class ClientTrainerEnrollment
{
    public Guid Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public Guid? ClientId { get; set; }

    public Guid? TrainerId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Trainer? Trainer { get; set; }
}
