using System.Text.RegularExpressions;
using Backend_AIHost.Data;
using Backend_AIHost.DTOs.Docker;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;

namespace Backend_AIHost.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class DockerController : ControllerBase
    {
        private readonly ILogger<DockerController> _logger;
        private readonly IDockerService _dockerService;
        private readonly IUserService _userService;

        public DockerController(ILogger<DockerController> logger, IDockerService dockerService, IUserService userService)
        {
            _logger = logger;
            _dockerService = dockerService;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImages(int id)
        {
            var userId = _userService.GetUserId();
            var images = await _dockerService.GetImagesAsync(id, userId);
            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContainers(int id)
        {
            var userId = _userService.GetUserId();
            var containers = await _dockerService.GetContainersAsync(id, userId);
            return Ok(containers);
        }
    }
}
