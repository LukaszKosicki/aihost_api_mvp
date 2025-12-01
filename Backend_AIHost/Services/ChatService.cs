using Backend_AIHost.Data;
using Backend_AIHost.DTOs.Chat;
using Backend_AIHost.Models;
using Backend_AIHost.Models.Responses;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Backend_AIHost.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ApiChecker _apiChecker;
        public ChatService(AppDbContext context, HttpClient httpClient, ApiChecker apiChecker)
        {
            _context = context;
            _httpClient = httpClient;
            _apiChecker = apiChecker;
        }

        public async Task<IEnumerable<ActiveChatDto>> GetActiveChats(string userId)
        {
            var userChats = await _context.UserContainers
                .Where(e => e.UserId == userId)
                .Include(e => e.VPS)
                .ToListAsync();

            foreach (var chat in userChats)
            {
                if (chat.VPS == null || string.IsNullOrWhiteSpace(chat.VPS.IP) || chat.Port == 0)
                {
                    chat.IsActive = false;
                    continue;
                }

                var url = $"http://{chat.VPS.IP}:{chat.Port}/generate";
                try
                {
                    chat.IsActive = await _apiChecker.CheckHealthAsync(url);
                }
                catch
                {
                    chat.IsActive = false;
                }
            }

            var result = userChats
                .Where(e => e.IsActive)
                .Select(e => new ActiveChatDto
                {
                    Name = e.ContainerName,
                    Path = $"/aichat/{e.Id}"
                })
                .ToList();

            return result;
        }


        public async Task<string> SendMessageAsync(string message, int containerId, string userId)
        {
            var container = await _context.UserContainers.FirstOrDefaultAsync(e => e.Id == containerId && e.UserId == userId);

            if (container == null) return null;

            var vps = _context.VPSes.Find(container.VPSId);
            var url = $"http://{vps.IP}:{container.Port}/generate";
            
            var response = await _httpClient.PostAsJsonAsync(url, new { prompt = $"User: {message}\nAI:",
                max_tokens = 50,
                stop = new[] { "\nQ:", "\nAI:" },
                temperature = 0.2,
                frequency_penalty = 0.5,
                presence_penalty = 0.5
            });

            if (!response.IsSuccessStatusCode)
                return "[Error] Failed to get response from model.";

            var result = await response.Content.ReadFromJsonAsync<List<AITextModelResponse>>();
            var text = result?.FirstOrDefault()?.GeneratedText;

            if (string.IsNullOrWhiteSpace(text))
                return "[Error] Empty model response.";

            // Spróbuj wyciągnąć ostatnią linię po "AI:"
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines.Reverse())
            {
                if (line.StartsWith("AI:"))
                    return line.Replace("AI:", "").Trim();
            }

            // Jeśli nie znaleziono "AI:", zwróć całość lub coś sensownego
            return text.Trim();
        }
    }
}
