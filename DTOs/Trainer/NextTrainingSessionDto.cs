namespace GymFit.DTOs.Trainer
{
    public class NextTrainingSessionDto
    {
        public string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public string ClientName { get; set; }
        public int DurationInMinutes { get; set; }
    }
}