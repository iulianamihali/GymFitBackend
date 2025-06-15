namespace GymFit.DTOs
{
    public class LeaveReviewModelDto
    {
        public int RatingValue { get; set; }
        public string Comment { get; set; }
        public Guid ClientId { get; set; }
        public Guid TrainerId { get; set; }
    }
}
