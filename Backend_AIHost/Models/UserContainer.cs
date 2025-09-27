using Backend_AIHost.Enums;

namespace Backend_AIHost.Models
{
    public class UserContainer
    {
        public int Id { get; set; }
        public int VPSId { get; set; }
        public int ModelId { get; set; }
        public int ImageId { get; set; } // ID obrazu, z którego został utworzony kontener
        public int Port { get; set; }
        public bool IsActive { get; set; } = true; // Czy kontener jest aktywny
        public string ContainerId { get; set; } // ID kontenera w Dockerze
        public string ContainerName { get; set; }
        public string ImageName { get; set; }
        public string UserId { get; set; }
        public VPS VPS { get; set; }
        public DateTime CreatedAt { get; set; }
        public ContainerStatus Status { get; set; } = ContainerStatus.Unknown;

    }
}
