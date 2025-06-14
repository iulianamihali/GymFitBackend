namespace GymFit.DTOs
{
    public class EnrollmentClientTrainerModelDto
    {
        public Guid ClientId { get; set; }
        public Guid TrainerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
