namespace CommunityKnowledgeSharingPlatform.DTOs
{
    public class ProfileDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? Bio { get; set; }

    }
}



