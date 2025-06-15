namespace GymFit.DTOs.Trainer
{
    public class CoursesCreatedDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int MaxParticipants { get; set; }
        public int TotalParticipants { get; set; }
        public bool Active { get; set; }

    }
}
