namespace GymFit.DTOs
{
    public class TrainerCardResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string  Specialization { get; set; }
        public double Rating { get; set; }
        public string Certification { get; set; }
        public decimal PricePerHour { get; set; }
        public string StartInterval { get; set; }
        public string EndInterval { get; set; }
        public int YearsOfExperience { get; set; }
    }
}
