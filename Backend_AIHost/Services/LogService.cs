using Backend_AIHost.Hubs;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Renci.SshNet;

namespace Backend_AIHost.Services
{ 
    public class LogService : ILogService
    {
        private readonly IHubContext<LogHub> _hubContext;

        public LogService(IHubContext<LogHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleLogShell(SshClient client, string command, string connectionId, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var stream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024))
                using (var reader = new StreamReader(stream))
                {
                    var buffer = new char[1024];

                    // Uruchamiamy komendę w shellu
                    stream.WriteLine(command);

                    while (client.IsConnected && !cancellationToken.IsCancellationRequested)
                    {
                        // Jeśli jest coś do odczytania
                        while (stream.DataAvailable)
                        {
                            int bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length);
                            if (bytesRead > 0)
                            {
                                var output = new string(buffer, 0, bytesRead);

                                foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                                {
                                    await SendLog(connectionId, line.Trim());
                                }
                            }
                        }

                        await Task.Delay(50, cancellationToken);
                    }
                }
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();

                client.Dispose();
            }
        }

        public async Task HandleLog(SshCommand cmd, string connectionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var asyncResult = cmd.BeginExecute();

                using (var outputReader = new StreamReader(cmd.OutputStream))
                using (var errorReader = new StreamReader(cmd.ExtendedOutputStream))
                {
                    while (!asyncResult.IsCompleted && !cancellationToken.IsCancellationRequested)
                    {
                        while (!outputReader.EndOfStream)
                        {
                            var outputLine = await outputReader.ReadLineAsync();
                            if (!string.IsNullOrEmpty(outputLine))
                                await SendLog(connectionId, outputLine);
                        }

                        while (!errorReader.EndOfStream)
                        {
                            var errorLine = await errorReader.ReadLineAsync();
                            if (!string.IsNullOrEmpty(errorLine))
                            {
                                if (errorLine.StartsWith("#") || errorLine.Contains("using docker driver"))
                                    await SendLog(connectionId, errorLine);
                                else
                                    await SendLog(connectionId, "[ERR] " + errorLine);
                            }
                        }

                        await Task.Delay(50, cancellationToken);
                    }

                    cmd.EndExecute(asyncResult);
                }
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public async Task SendLog(string connectionId, string message)
        {
            if (!string.IsNullOrEmpty(connectionId))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveLog", message);
            }
            else
            {
                await _hubContext.Clients.All.SendAsync("ReceiveLog", message);
            }
        }
    }

}
