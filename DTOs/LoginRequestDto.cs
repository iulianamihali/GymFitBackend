﻿using System.ComponentModel.DataAnnotations;

namespace GymFit.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
