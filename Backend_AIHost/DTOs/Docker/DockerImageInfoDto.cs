namespace Backend_AIHost.DTOs.Docker
{
    public class DockerImageInfoDto
    {
        public string Name { get; set; }           // np. "nginx:latest"
        public string ImageId { get; set; }        // np. "e1ab2fcdb204"
        public string ModelName { get; set; }    // Nazwa modelu AI, jeśli obraz jest oparty na modelu
        public bool HasContainer { get; set; }     // true jeśli istnieje kontener oparty na tym obrazie
    }
}
