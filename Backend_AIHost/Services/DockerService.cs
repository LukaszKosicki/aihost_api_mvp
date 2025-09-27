using Backend_AIHost.Data;
using Backend_AIHost.DTOs.Docker;
using System.Text.RegularExpressions;
using Backend_AIHost.Services.Interfaces;
using Renci.SshNet;
using Microsoft.EntityFrameworkCore;

namespace Backend_AIHost.Services
{
    public class DockerService : IDockerService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DockerService> _logger;

        public DockerService(AppDbContext context, ILogger<DockerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<DockerImageInfoDto>> GetImagesAsync(int vpsId, string userId)
        {
            var images = new List<DockerImageInfoDto>();
            var vpn = await _context.VPSes.FirstOrDefaultAsync(v => v.Id == vpsId && v.UserId == userId);
            if (vpn == null)
            {
                return images;
            }

            try
            {
                using var client = new SshClient(vpn.IP, vpn.Port, vpn.Username, vpn.Password);
                client.Connect();

                var imageCmd = client.RunCommand("sudo docker images --format \"{{.Repository}}:{{.Tag}}|{{.ID}}\"");
                var imageOutput = imageCmd.Result;

                var containerCmd = client.RunCommand("sudo docker ps -a --format \"{{.Image}}\"");
                var containerOutput = containerCmd.Result;

                client.Disconnect();

                var usedImages = containerOutput
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .ToHashSet();

                var lines = imageOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        var imageName = parts[0];
                        var imageId = parts[1];
                        bool hasContainer = usedImages.Contains(imageName) || usedImages.Contains(imageId);

                        images.Add(new DockerImageInfoDto
                        {
                            Name = imageName,
                            ImageId = imageId,
                            HasContainer = hasContainer,
                            ModelName = _context.UserImages.FirstOrDefault(ui => ui.ImageId == imageId)?.ModelName ?? "N/A",
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania obrazów Dockera dla VPS {Id}", vpsId);
            }

            return images;
        }

        public async Task<List<DockerContainerInfoDto>> GetContainersAsync(int vpsId, string userId)
        {
            var containers = new List<DockerContainerInfoDto>();
            var vpn = await _context.VPSes.FirstOrDefaultAsync(v => v.Id == vpsId && v.UserId == userId);
            if (vpn == null)
            {
                return containers;
            }

            try
            {
                using var client = new SshClient(vpn.IP, vpn.Port, vpn.Username, vpn.Password);
                client.Connect();

                var cmd = client.RunCommand("sudo docker ps -a --format \"{{.ID}}|{{.Names}}|{{.Status}}|{{.Ports}}\"");
                string output = cmd.Result;

                client.Disconnect();

                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        string rawStatus = parts[2];
                        string mappedStatus;

                        if (rawStatus.Contains("Up", StringComparison.OrdinalIgnoreCase))
                            mappedStatus = "running";
                        else if (rawStatus.Contains("Exited", StringComparison.OrdinalIgnoreCase))
                            mappedStatus = "stopped";
                        else if (rawStatus.Contains("Error", StringComparison.OrdinalIgnoreCase))
                            mappedStatus = "error";
                        else
                            mappedStatus = "zatrzymany";

                        string port = "-";
                        if (!string.IsNullOrWhiteSpace(parts[3]))
                        {
                            var match = Regex.Match(parts[3], @"(\d+)->");
                            if (match.Success)
                            {
                                port = match.Groups[1].Value;
                            }
                        }

                        containers.Add(new DockerContainerInfoDto
                        {
                            Id = parts[0],
                            Name = parts[1],
                            Status = mappedStatus,
                            Port = port,
                            ModelName = _context.UserContainers.FirstOrDefault(uc => uc.ContainerId.Contains(parts[0]))?.ImageName ?? "N/A",
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania kontenerów Dockera dla VPS {Id}", vpsId);
            }

            return containers;
        }
    }
}
