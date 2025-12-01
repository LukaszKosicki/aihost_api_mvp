namespace Backend_AIHost.DTOs.Admin.AIModel
{
    public class AllModelsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MinRequiredRamMb { get; set; }
        public int Port { get; set; }
    }
}
