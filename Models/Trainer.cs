using System;
using System.Collections.Generic;

namespace GymFit.Models;

public partial class Trainer
{
    public Guid UserId { get; set; }

    public string Specialization { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public string? Certification { get; set; }

    public decimal PricePerHour { get; set; }
    public string StartInterval { get; set; }
    public string EndInterval { get; set; }

    public virtual ICollection<ClientTrainerEnrollment> ClientTrainerEnrollments { get; set; } = new List<ClientTrainerEnrollment>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();

    public virtual User User { get; set; } = null!;
}
