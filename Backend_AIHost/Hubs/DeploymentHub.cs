using Microsoft.AspNetCore.SignalR;

namespace Backend_AIHost.Hubs
{
    public class DeploymentHub : Hub
    {
        public Task<string> GetConnectionId()
        {
            return Task.FromResult(Context.ConnectionId);
        }
    }
}
