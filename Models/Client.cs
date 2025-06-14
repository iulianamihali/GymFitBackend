using System;
using System.Collections.Generic;

namespace GymFit.Models;

public partial class Client
{
    public Guid UserId { get; set; }

    public string? Goal { get; set; }

    public string? HealthNotes { get; set; }

    public virtual ICollection<ClientCourseEnrollment> ClientCourseEnrollments { get; set; } = new List<ClientCourseEnrollment>();

    public virtual ICollection<ClientTrainerEnrollment> ClientTrainerEnrollments { get; set; } = new List<ClientTrainerEnrollment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();

    public virtual User User { get; set; } = null!;
}
