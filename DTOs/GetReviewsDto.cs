namespace GymFit.DTOs
{
    public class GetReviewsDto
    {
        public DateTime CreatedAt { get; set; }
        public int RatingValue { get; set; }
        public string Comment {  get; set; }
        public string ClientName { get; set; }
        public string TrainerName { get; set; }

    }
}
