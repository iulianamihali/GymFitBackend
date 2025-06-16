namespace GymFit.DTOs
{
    public class AddSessionRequestDto
    {
        public Guid ClientId { get; set; }
        public Guid? TrainerId { get; set; }
        public DateTime StartDateTime { get; set; }
        public int DurationInMinutes { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
    }
}
