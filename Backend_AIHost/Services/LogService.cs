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
            using (var cmd = client.CreateCommand(command))
            {
                var asyncResult = cmd.BeginExecute();
                using (var output = new StreamReader(cmd.OutputStream))
                using (var error = new StreamReader(cmd.ExtendedOutputStream))
                {
                    char[] buffer = new char[1024];
                    while (!asyncResult.IsCompleted && !cancellationToken.IsCancellationRequested)
                    {
                        while (!output.EndOfStream && output.Peek() >= 0)
                        {
                            int read = await output.ReadAsync(buffer, 0, buffer.Length);
                            if (read > 0)
                            {
                                string text = new string(buffer, 0, read);
                                foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                                    await SendLog(connectionId, line.Trim());
                            }
                        }

                        while (!error.EndOfStream && error.Peek() >= 0)
                        {
                            int read = await error.ReadAsync(buffer, 0, buffer.Length);
                            if (read > 0)
                            {
                                string text = new string(buffer, 0, read);
                                foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                                    await SendLog(connectionId, "[ERROR] " + line.Trim());
                            }
                        }

                        await Task.Delay(200, cancellationToken);
                    }

                    cmd.EndExecute(asyncResult);

                    await SendLog(connectionId, $"[DONE] Command finished with exit status {cmd.ExitStatus}");
                }
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
