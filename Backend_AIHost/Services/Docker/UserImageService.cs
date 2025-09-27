using Backend_AIHost.Data;
using Backend_AIHost.DTOs.Docker;
using Backend_AIHost.DTOs.Image;
using Backend_AIHost.Models;
using Backend_AIHost.Services.Docker.Interfaces;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;

namespace Backend_AIHost.Services.Docker
{
    public class UserImageService : IUserImageService
    {
        private readonly AppDbContext _context;
        public UserImageService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<DockerContainerInfoDto> Run(RunNewContainerDto dto, string userId)
        {
            var userImage = await _context.UserImages.FirstOrDefaultAsync(e => e.ImageId == dto.ImageId);
            var vps = await _context.VPSes.FindAsync(dto.VpsId);

            if (userImage == null || vps == null)
                return new DockerContainerInfoDto
                {
                    Status = "Error"
                };

            using var client = new SshClient(vps.IP, vps.Port, vps.Username, vps.Password);
            try
            {
                client.Connect();

                var command = client.CreateCommand($"sudo docker run -d --name {dto.ContainerName} -p {dto.Port}:{dto.Port} {userImage.ImageId}");
                string containerId = command.Execute().Trim();

                var newUserContainer = new UserContainer
                {
                    UserId = userId,
                    ContainerId = containerId,
                    CreatedAt = DateTime.Now,
                    ImageId = userImage.Id,
                    ContainerName = dto.ContainerName,
                    IsActive = true,
                    ImageName = userImage.ModelName,
                    ModelId = userImage.ModelId,
                    Port = dto.Port,
                    VPSId = dto.VpsId
                };
                await _context.UserContainers.AddAsync(newUserContainer);
                await _context.SaveChangesAsync();
                return new DockerContainerInfoDto
                {
                    ModelName = userImage.ModelName,
                    Name = dto.ContainerName,
                    Port = dto.Port.ToString(),
                    Status = "Run"
                };
            }
            catch (Exception ex)
            {
                return new DockerContainerInfoDto
                { 
                    Status = "Error"
                }; 
            }
               
            
        }

        public async Task<bool> DeleteImageAsync(int vpsId, string imageId, string userId)
        {
            var vps = await _context.VPSes.FirstOrDefaultAsync(v => v.Id == vpsId && v.UserId == userId);
            if (vps == null) return false;

            using var client = new SshClient(vps.IP, vps.Port, vps.Username, vps.Password);
            client.Connect();

            // 1. Pobierz kontenery korzystające z obrazu
            var containerCmd = client.RunCommand($"sudo docker ps -a --filter ancestor={imageId} --format \"{{{{.ID}}}}\"");
            var containerIds = containerCmd.Result
                .Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // 2. Zatrzymaj i usuń każdy kontener
            foreach (var containerId in containerIds)
            {
                client.RunCommand($"sudo docker stop {containerId}");
                client.RunCommand($"sudo docker rm {containerId}");
            }

            // 3. Usuń obraz
            var deleteCmd = client.RunCommand($"sudo docker rmi {imageId}");

            client.Disconnect();

            return string.IsNullOrEmpty(deleteCmd.Error);
        }


        public double GetUserImageSize(string imageName, SshClient client)
        {
            var imageSizeCmd = client.CreateCommand($"sudo docker inspect {imageName.ToLower()} --format='{{{{.Size}}}}'");
            var result = imageSizeCmd.Execute();
            long sizeBytes = long.Parse(result.Trim());

            return sizeBytes / (1024.0 * 1024.0);
        }

        public string GetUserImageId(string imageName, SshClient client)
        {
            var imageIdCmd = client.CreateCommand($"sudo docker images -q {imageName.ToLower()}");
            var result = imageIdCmd.Execute().Trim();
            return result;
        }
    }
}
