using Backend_AIHost.DTOs.Container;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend_AIHost.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ContainerController : ControllerBase
    {
        private readonly IContainerService _containerService;

        public ContainerController(IContainerService containerService)
        {
            _containerService = containerService;
        }

        [HttpPost]
        public async Task<IActionResult> Start([FromBody] ContainerActionDto dto)
        {
            var result = await _containerService.StartAsync(dto.VpsId, dto.ContainerId);
            return result.Success ? Ok(result.Message) : StatusCode(500, result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Stop([FromBody] ContainerActionDto dto)
        {
            var result = await _containerService.StopAsync(dto.VpsId, dto.ContainerId);
            return result.Success ? Ok(result.Message) : StatusCode(500, result.Message);
        }

        [HttpDelete("{vpsId}/{containerId}")]
        public async Task<IActionResult> Delete(int vpsId, string containerId)
        {
            var result = await _containerService.DeleteAsync(vpsId, containerId);
            return result.Success ? Ok(result.Message) : StatusCode(500, result.Message);
        }
    }
}
