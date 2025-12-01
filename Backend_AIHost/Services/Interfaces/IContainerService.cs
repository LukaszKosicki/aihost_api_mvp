namespace Backend_AIHost.Services.Interfaces
{
    public interface IContainerService
    {
        Task<(bool Success, string Message)> StartAsync(int vpsId, string containerId);
        Task<(bool Success, string Message)> StopAsync(int vpsId, string containerId);
        Task<(bool Success, string Message)> DeleteAsync(int vpsId, string containerId);
    }

}
