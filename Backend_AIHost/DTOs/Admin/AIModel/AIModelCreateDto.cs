namespace Backend_AIHost.DTOs.Admin.AIModel
{
    public class AIModelCreateDto
    {
        public int MinRequiredRamMb { get; set; }
        public string Name { get; set; }
        public string ModelInternalName { get; set; }
        public int DefaultPort { get; set; }
        public string Tags { get; set; }
        public bool SupportsGPU { get; set; }
        public bool IsActive { get; set; }
    }
}
