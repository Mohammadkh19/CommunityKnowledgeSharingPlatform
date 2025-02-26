﻿using System.ComponentModel.DataAnnotations;

namespace CommunityKnowledgeSharingPlatform.DTOs
{
    public class RegisterDTO
    {

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

        [Required] public string Username { get; set; }

    }
}
