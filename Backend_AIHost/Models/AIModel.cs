namespace Backend_AIHost.Models
{
    public class AIModel
    {
        public int Id { get; set; }
        public int MinRequiredRamMb { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ModelInternalName { get; set; }
        public string Slug { get; set; }
        public int DefaultPort { get; set; }
        public bool SupportsGPU { get; set; }
        public bool IsActive { get; set; } // Default to true
        public DateTime CreatedAt { get; set; }
    }
}
