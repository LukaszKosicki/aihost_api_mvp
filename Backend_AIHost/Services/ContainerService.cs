namespace Backend_AIHost.Services
{
    using Backend_AIHost.Data;
    using Backend_AIHost.Services.Interfaces;
    using Renci.SshNet;

    public class ContainerService : IContainerService
    {
        private readonly AppDbContext _context;

        public ContainerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> StartAsync(int vpsId, string containerId)
        {
            var vps = _context.VPSes.FirstOrDefault(v => v.Id == vpsId);
            if (vps == null)
                return (false, "VPS not found.");

            using var client = new SshClient(vps.IP, vps.Port, vps.Username, vps.Password);
            try
            {
                client.Connect();

                var startCmd = client.RunCommand($"sudo docker start \"{containerId}\"");
                if (!string.IsNullOrEmpty(startCmd.Error))
                    return (false, $"Error starting container: {startCmd.Error}");

                var checkCmd = client.RunCommand($"sudo docker ps --filter \"name={containerId}\" --format '{{{{.Names}}}}'");
                var isRunning = checkCmd.Result.Trim() == containerId;

                return isRunning
                    ? (true, $"Container '{containerId}' started successfully.")
                    : (false, $"Failed to start container '{containerId}'.");
            }
            catch (Exception ex)
            {
                return (false, $"Exception: {ex.Message}");
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();
            }
        }

        public async Task<(bool Success, string Message)> StopAsync(int vpsId, string containerId)
        {
            var vps = _context.VPSes.FirstOrDefault(v => v.Id == vpsId);
            if (vps == null)
                return (false, "VPS not found.");

            using var client = new SshClient(vps.IP, vps.Port, vps.Username, vps.Password);
            try
            {
                client.Connect();

                var stopCmd = client.RunCommand($"sudo docker stop \"{containerId}\"");
                if (!string.IsNullOrEmpty(stopCmd.Error))
                    return (false, $"Error stopping container: {stopCmd.Error}");

                var checkCmd = client.RunCommand($"sudo docker ps --filter \"name={containerId}\" --format '{{{{.Names}}}}'");
                var isStopped = string.IsNullOrWhiteSpace(checkCmd.Result);

                return isStopped
                    ? (true, containerId)
                    : (false, $"Failed to stop container '{containerId}'.");
            }
            catch (Exception ex)
            {
                return (false, $"Exception: {ex.Message}");
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int vpsId, string containerId)
        {
            var vps = _context.VPSes.FirstOrDefault(v => v.Id == vpsId);
            if (vps == null)
                return (false, "VPS not found.");

            using var client = new SshClient(vps.IP, vps.Port, vps.Username, vps.Password);
            try
            {
                client.Connect();

                client.RunCommand($"sudo docker stop \"{containerId}\"");
                var deleteCmd = client.RunCommand($"sudo docker rm \"{containerId}\"");

                if (!string.IsNullOrEmpty(deleteCmd.Error))
                    return (false, $"Error deleting container: {deleteCmd.Error}");

                var checkCmd = client.RunCommand($"sudo docker ps -a --filter \"name={containerId}\" --format '{{{{.Names}}}}'");
                var containerExists = checkCmd.Result.Trim() == containerId;

                return !containerExists
                    ? (true, $"Container '{containerId}' deleted successfully.")
                    : (false, $"Failed to delete container '{containerId}'.");
            }
            catch (Exception ex)
            {
                return (false, $"Exception: {ex.Message}");
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();
            }
        }
    }

}
