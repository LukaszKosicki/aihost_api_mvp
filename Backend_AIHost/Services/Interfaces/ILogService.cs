using Renci.SshNet;

namespace Backend_AIHost.Services.Interfaces
{
    public interface ILogService
    {
        public Task HandleLog(SshCommand cmd, string connectionId, CancellationToken cancellationToken);
        public Task SendLog(string connectionId, string message);
        public Task HandleLogShell(SshClient client, string command, string connectionId, CancellationToken cancellationToken);
    }
}
