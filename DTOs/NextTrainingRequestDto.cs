namespace GymFit.DTOs
{
    public class NextTrainingRequestDto
    {
        public string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public string TrainerName { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
