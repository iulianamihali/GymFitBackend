using System;
using System.Collections.Generic;

namespace GymFit.Models;

public partial class Course
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int MaxParticipants { get; set; }

    public Guid? TrainerId { get; set; }

    public virtual ICollection<ClientCourseEnrollment> ClientCourseEnrollments { get; set; } = new List<ClientCourseEnrollment>();

    public virtual Trainer? Trainer { get; set; }
}
