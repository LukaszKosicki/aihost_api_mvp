namespace Backend_AIHost.Models
{
    public class AIChat
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VPSId { get; set; }
        public int ModelId { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
    }
}
