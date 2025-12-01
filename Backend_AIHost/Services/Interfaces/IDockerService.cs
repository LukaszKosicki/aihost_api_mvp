using Backend_AIHost.DTOs.Docker;

namespace Backend_AIHost.Services.Interfaces
{
    public interface IDockerService
    {
        Task<List<DockerImageInfoDto>> GetImagesAsync(int vpsId, string userId);
        Task<List<DockerContainerInfoDto>> GetContainersAsync(int vpsId, string userId);

    }
}
