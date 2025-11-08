using Backend_AIHost.Data;
using Backend_AIHost.DTOs.AIModel;
using Backend_AIHost.DTOs.VPS;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_AIHost.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class AIModelController : ControllerBase
    {
        private readonly ILogger<VPSController> _logger;
        private readonly IAIModelService _aiModelService;

        public AIModelController(ILogger<VPSController> logger, IAIModelService aIModelService)
        {
            _logger = logger;
            _aiModelService = aIModelService;
        }

        // GET: api/vps
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _aiModelService.GetListAvailableModelsAsync();
            return Ok(result);
        }

    }

}
