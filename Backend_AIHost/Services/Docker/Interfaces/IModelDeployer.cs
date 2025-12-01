namespace Backend_AIHost.Services.Docker.Interfaces
{
    public interface IModelDeployer
    {
        Task DeployDockerAsync(string host, string username, string password, string modelInternalName, int port);
    }
}
