namespace GymFit.DTOs
{
    public class LoginResponseDto
    {
        public string Token {  get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserType {  get; set; }
    }
}
