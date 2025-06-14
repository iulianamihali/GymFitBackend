namespace GymFit.DTOs
{
    public class CoursesCardResponseDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int MaxParticipants { get; set; }
        public string TrainerName { get; set; }
        public bool Active { get; set; }
        public int TotalParticipants { get; set; }
    }
}
