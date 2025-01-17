using System.ComponentModel.DataAnnotations;

namespace CommunityKnowledgeSharingPlatform.DTOs
{
    public class LoginDTO
    {
        [Required] public string? UsernameOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
