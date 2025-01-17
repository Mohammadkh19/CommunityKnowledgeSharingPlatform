using Microsoft.Extensions.Hosting;

namespace CommunityKnowledgeSharingPlatform.Models
{
    public class Categories
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        // Navigation properties
        public ICollection<Posts> Posts { get; set; }
    }
}
