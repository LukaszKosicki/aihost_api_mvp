namespace Backend_AIHost.DTOs.Image
{
    public class RunNewContainerDto 
    {
        public string ImageId { get; set; }
        public string ContainerName { get; set; }
        public int Port { get; set; }
        public int VpsId { get; set; }
    }
}
