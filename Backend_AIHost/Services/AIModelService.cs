using Backend_AIHost.Data;
using Backend_AIHost.DTOs.AIModel;
using Backend_AIHost.Enums;
using Backend_AIHost.Helpers;
using Backend_AIHost.Hubs;
using Backend_AIHost.Models;
using Backend_AIHost.Models.Responses;
using Backend_AIHost.Services.Docker;
using Backend_AIHost.Services.Docker.Interfaces;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;

namespace Backend_AIHost.Services
{
    public class AIModelService : IAIModelService
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserImageService _userImageService;
        private readonly ILogService _logService;

        public AIModelService(AppDbContext dbContext, IUserImageService userImageService, ILogService logService)
        {
            _dbContext = dbContext;
            _userImageService = userImageService;
            _logService = logService;
        }

        public async Task<IEnumerable<AvailableModelDto>> GetListAvailableModelsAsync()
        {
            return await _dbContext.AIModels
                .Select(v => new AvailableModelDto
                {
                    Value = v.Id,
                    Label = v.Name
                })
                .ToListAsync();
        }

        public async Task<DeployResult> DeployModelAsync(DeployModelDto dto)
        {
            var model = await _dbContext.AIModels.FindAsync(dto.ModelId);
            var vps = await _dbContext.VPSes.FindAsync(dto.VPSId);

            if (model == null)
                return new DeployResult(false, "Nie znaleziono modelu.");

            string remotePath = $"/home/{vps.Username}/";

            try
            {
                // 1. Przygotowanie pliku Python
                await _logService.SendLog(dto.ConnectionId, "Creating a app.py file...");
                var py_file = PyFileGenerator.GenerateAppPy(model.ModelInternalName, 50, dto.ExposePort);
                SftpHelper.UploadFile(vps.IP, 22, vps.Username, vps.Password, py_file, remotePath + "app.py");

                // 2. Przygotowanie Dockerfile
                await _logService.SendLog(dto.ConnectionId, "Creating a Dockerfile...");
                var dockerfileContent = DockerfileGenerator.GenerateDockerfileWithoutPyFile(model.ModelInternalName, dto.ExposePort);
                await _logService.SendLog(dto.ConnectionId, "Uploading Dockerfile to VPS...");
                SftpHelper.UploadFile(vps.IP, 22, vps.Username, vps.Password, dockerfileContent, remotePath + "Dockerfile");

                // 3. Utworzenie klienta SSH
                var client = new SshClient(vps.IP, vps.Port, vps.Username, vps.Password);
                client.Connect();

                try
                {
                    await _logService.SendLog(dto.ConnectionId, "Building Docker image...");

                    // Uruchamiamy build w tle
                    var cts = new CancellationTokenSource();
                    var logTask = _logService.HandleLogShell(client,
                        $"cd {remotePath} && sudo docker build -t {model.ModelInternalName.ToLower()} .",
                        dto.ConnectionId, cts.Token);

                    await _logService.SendLog(dto.ConnectionId, "Docker build started... logs streaming.");

                    // Opcjonalne krótkie opóźnienie, aby build się rozkręcił
                    await Task.Delay(2000);

                    // 4. Pobranie informacji o obrazie
                    double sizeMB = _userImageService.GetUserImageSize(model.ModelInternalName, client);
                    string imageId = _userImageService.GetUserImageId(model.ModelInternalName, client);
                    await _logService.SendLog(dto.ConnectionId, $"Image size: {sizeMB} Mb");

                    var userImage = new UserImage
                    {
                        UserId = vps.UserId,
                        ImageName = model.ModelInternalName,
                        ModelId = model.Id,
                        CreatedAt = DateTime.UtcNow,
                        SizeMb = (int)sizeMB,
                        VPSId = vps.Id,
                        IsActive = true,
                        Tag = "latest",
                        ImageId = imageId,
                        ModelName = model.Name
                    };
                    _dbContext.UserImages.Add(userImage);

                    // 5. Uruchomienie kontenera
                    await _logService.SendLog(dto.ConnectionId, "Starting the container...");
                    var command = client.CreateCommand($"sudo docker run -d --name {dto.ContainerName} -p {dto.Port}:{dto.ExposePort} {model.ModelInternalName.ToLower()}");
                    string containerId = command.Execute().Trim();

                    var container = new UserContainer
                    {
                        UserId = vps.UserId,
                        ContainerName = dto.ContainerName,
                        ContainerId = containerId,
                        ModelId = model.Id,
                        VPSId = vps.Id,
                        Port = dto.Port,
                        CreatedAt = DateTime.UtcNow,
                        Status = ContainerStatus.Running,
                        ImageName = model.Name,
                    };
                    _dbContext.UserContainers.Add(container);

                    await _dbContext.SaveChangesAsync();

                    await _logService.SendLog(dto.ConnectionId, "Container started successfully.");

                    // 6. Czekamy na zakończenie logowania builda
                    await logTask;

                    return new DeployResult(true, "Your AI Model has been launched.");
                }
                finally
                {
                    if (client.IsConnected)
                        client.Disconnect();

                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                return new DeployResult(false, $"Błąd połączenia: {ex.Message}");
            }
        }

    }
}
