namespace GymFit.DTOs
{
    public class CourseDetailsResponseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int MaxParticipants { get; set; }
        public string TrainerName { get; set; }
    }
}
