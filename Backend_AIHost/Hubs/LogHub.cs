using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Renci.SshNet;

namespace Backend_AIHost.Hubs
{
    public class LogHub : Hub
    {
        private static readonly Dictionary<string, CancellationTokenSource> _tokens = new();

        private readonly ILogService _logService;

        public LogHub(ILogService logService)
        {
            _logService = logService;
        }

        public Task<string> GetConnectionId()
        {
            return Task.FromResult(Context.ConnectionId);
        }

        public Task StartLogs(string host, string username, string password, string command)
        {
            var connectionId = Context.ConnectionId;

            var client = new SshClient(host, username, password);
            client.Connect();

            var cts = new CancellationTokenSource();
            _tokens[connectionId] = cts;

            // Odpal logowanie w tle
            _ = _logService.HandleLogShell(client, command, connectionId, cts.Token);

            return Task.CompletedTask;
        }

        public Task StopLogs()
        {
            var connectionId = Context.ConnectionId;

            if (_tokens.TryGetValue(connectionId, out var cts))
            {
                cts.Cancel();   // zatrzymuje HandleLogShell
                _tokens.Remove(connectionId);
            }

            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            StopLogs();
            return base.OnDisconnectedAsync(exception);
        }
    }
}
