namespace CommunityKnowledgeSharingPlatform.Models
{
    public class Profiles
    {
        public int ProfileId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string? Address { get; set; }

        public string UserId { get; set; }
        public Users User { get; set; }
    }
}
