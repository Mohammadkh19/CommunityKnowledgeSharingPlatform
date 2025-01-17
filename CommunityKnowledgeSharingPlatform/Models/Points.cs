namespace CommunityKnowledgeSharingPlatform.Models
{
    public class Points
    {
        public int PointId { get; set; }
        public int PointsNumber { get; set; }
        public string PointsType { get; set; }
        public DateTime ReceivedDate { get; set; }

        public string UserId { get; set; }
        public Users User { get; set; }
    }
}
