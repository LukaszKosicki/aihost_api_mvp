using Backend_AIHost.DTOs.AIModel;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend_AIHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelDeployController : ControllerBase
    {
        private readonly IAIModelService _aiModelService;

        public ModelDeployController(IAIModelService aiModelService)
        {
            _aiModelService = aiModelService;
        }

        [HttpPost("deploy")]
        public async Task<IActionResult> DeployModel([FromBody] DeployModelDto dto)
        {
            var result = await _aiModelService.DeployModelAsync(dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
