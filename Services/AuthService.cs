using GymFit.Data;
using GymFit.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using GymFit.DTOs;
using Microsoft.AspNetCore.Identity;



namespace GymFit.Services
{
    public class AuthService
    {
        private readonly GymFitContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthService(GymFitContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public User? ValidateLogin(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return null;
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if(result == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return user;


        }
        public User RegisterUser(RegisterRequestDto request)
        {
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Gender = request.Gender,
                DateOfBirth = DateOnly.Parse(request.DateOfBirth),
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                UserType = request.UserType
            };
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);
            _context.Users.Add(newUser);
            if (request.UserType == "Client")
                {
                Client client = new Client
                {
                    UserId = newUser.Id,
                    Goal = null,
                    HealthNotes = null
                };
                _context.Clients.Add(client);
            }
                
            else if (request.UserType == "Trainer")
                {
                Trainer trainer = new Trainer
                    {
                    UserId = newUser.Id,
                    Specialization = request.Specialization,
                    YearsOfExperience = request.YearsOfExperience,
                    Certification = request.Certification,
                    PricePerHour = request.PricePerHour,
                    StartInterval = request.StartInterval,
                    EndInterval = request.EndInterval,

                    };
                    _context.Trainers.Add(trainer);

                }
                

            _context.SaveChanges();
            return newUser;
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
