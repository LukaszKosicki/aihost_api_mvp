using Backend_AIHost.DTOs.Chat;

namespace Backend_AIHost.Services.Interfaces
{
    public interface IChatService
    {
        public Task<string> SendMessageAsync(string message, int containerId, string userId);
        public Task<IEnumerable<ActiveChatDto>> GetActiveChats(string userId);
    }
}
