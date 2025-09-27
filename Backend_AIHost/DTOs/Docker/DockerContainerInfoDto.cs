namespace Backend_AIHost.DTOs.Docker
{
    public class DockerContainerInfoDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ModelName { get; set; }
        public string Port { get; set; }
    }
}
