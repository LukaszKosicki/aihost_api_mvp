using Backend_AIHost.Data;
using Backend_AIHost.DTOs.Image;
using Backend_AIHost.Services.Docker.Interfaces;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;

namespace Backend_AIHost.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ImageController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserImageService _userImageService;

        public ImageController(IUserService userService, IUserImageService userImageService)
        {
            _userService = userService;
            _userImageService = userImageService;
        }

        [HttpPost]
        public async Task<IActionResult> RunNewContainer([FromBody] RunNewContainerDto dto)
        {
            var userId = _userService.GetUserId();
            await _userImageService.Run(dto, userId);
            return Ok();
        }

        [HttpDelete("{vpsId}/{imageId}")]
        public async Task<IActionResult> Delete(int vpsId, string imageId)
        {
            var userId = _userService.GetUserId();
            var success = await _userImageService.DeleteImageAsync(vpsId, imageId, userId);
            if (!success)
                return BadRequest("Nie udało się usunąć obrazu (być może nadal istnieją kontenery lub obraz nie istnieje).");

            return Ok($"Obraz {imageId} usunięty wraz z powiązanymi kontenerami.");
        }
    }
}
