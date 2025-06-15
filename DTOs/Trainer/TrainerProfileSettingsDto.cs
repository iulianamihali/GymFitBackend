namespace GymFit.DTOs.Trainer
{
    public class TrainerProfileSettingsDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public string Certification { get; set; }
        public decimal PricePerHour { get; set; }
        public string StartInterval { get; set; }
        public string EndInterval { get; set; }
    }


}
