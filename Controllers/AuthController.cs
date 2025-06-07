using GymFit.Data;
using GymFit.DTOs;
using GymFit.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
namespace GymFit.Controllers
   
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService; 
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            var user = _authService.ValidateLogin(request.Email, request.Password);
            if(user == null)
            {
                return Unauthorized("Incorrect email or password.");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    name = $"{user.FirstName},{user.LastName}",
                    email = user.Email,
                    role = user.UserType.ToString()
                }


            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDto request)
        {
            var newUser = _authService.RegisterUser(request);
            var token = _authService.GenerateJwtToken(newUser);
            var response = new LoginResponseDto
            {
                Token = token,
                UserId = newUser.Id,
                UserName = $"{newUser.FirstName}, {newUser.LastName}",
                Email = newUser.Email,
                UserType = newUser.UserType
            };
            return Ok(response);
        }

    }
}
