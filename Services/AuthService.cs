using GymFit.Data;
using GymFit.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;


namespace GymFit.Services
{
    public class AuthService
    {
        private readonly GymFitContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(GymFitContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public User? ValidateLogin(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if(user == null || user.PasswordHash != password)
            {
                return null;
            }
            return user;


        }

        public String GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("email",user.Email),
                new Claim("name", $"{user.FirstName}, {user.LastName}"),
                new Claim("role", user.UserType.ToString())
            };

            var secretKey = _configuration["JwtSettings:SecretKey"];
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
