namespace Backend_AIHost.DTOs.AIModel
{
    public class DeployModelDto
    {
        public string ContainerName { get; set; }
        public string ConnectionId { get; set; }
        public int ModelId { get; set; }
        public int VPSId { get; set; }
        public int Port { get; set; } = 5000; // Default port
        public int ExposePort { get; set; } = 5000; // Default expose port
    }
}
