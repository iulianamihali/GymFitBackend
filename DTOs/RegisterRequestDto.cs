using System.ComponentModel.DataAnnotations;

namespace GymFit.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string DateOfBirth { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }
        [Required]
        public string UserType { get; set; }
        public string Specialization {  get; set; }
        public int YearsOfExperience { get; set; }
        public string Certification {  get; set; }
        public decimal PricePerHour { get; set; }
        public string StartInterval { get; set; }
        public string EndInterval { get; set; }

    }
}
