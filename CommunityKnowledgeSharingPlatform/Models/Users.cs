using Microsoft.AspNetCore.Identity;

namespace CommunityKnowledgeSharingPlatform.Models
{
    public class Users : IdentityUser
    {
        
        // Navigation properties
        public ICollection<Posts> Posts { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Points> Points { get; set; }
        public ICollection<Notifications> Notifications { get; set; }
        public ICollection<Votes> Votes { get; set; }
    }
}
