using Backend_AIHost.DTOs.Docker;
using Backend_AIHost.DTOs.Image;
using Renci.SshNet;

namespace Backend_AIHost.Services.Docker.Interfaces
{
    public interface IUserImageService
    {
        double GetUserImageSize(string imageName, SshClient client);
        string GetUserImageId(string imageName, SshClient client);
        Task<DockerContainerInfoDto> Run(RunNewContainerDto dto, string userId);
        Task<bool> DeleteImageAsync(int vpsId, string imageId, string userId);
    }
}
