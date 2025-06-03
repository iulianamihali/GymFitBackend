using System;
using System.Collections.Generic;

namespace GymFit.Models;

public partial class ClientCourseEnrollment
{
    public Guid Id { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public Guid? ClientId { get; set; }

    public Guid? CourseId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Course? Course { get; set; }
}
