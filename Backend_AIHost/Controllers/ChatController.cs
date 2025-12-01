using Backend_AIHost.DTOs.Chat;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_AIHost.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatService _chatService;
        private readonly IUserService _userService;


        public ChatController(ILogger<ChatController> logger, IChatService chatService, IUserService userService)
        {
            _logger = logger;
            _chatService = chatService;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public IActionResult GetChat(int id)
        {
            var userId = _userService.GetUserId();
            var chats = _chatService.GetActiveChats(userId);

            return Ok(chats);

                
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveChats()
        {
            var userId = _userService.GetUserId();
            var chats = await _chatService.GetActiveChats(userId);
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ChatRequest request)
        {
            var userId = _userService.GetUserId();
            var result = await _chatService.SendMessageAsync(request.Message, request.ContainerId, userId);
            return Ok(result);
        }
    }
}
