using Backend_AIHost.Data;
using Backend_AIHost.DTOs.VPS;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_AIHost.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class VPSController : ControllerBase
    {
        private readonly ILogger<VPSController> _logger;
        private readonly AppDbContext _context;
        private readonly IVpsService _vpsService;
        private readonly IUserService _userService;
        public VPSController(ILogger<VPSController> logger, AppDbContext context, IVpsService vpsService, IUserService userService)
        {
            _logger = logger;
            _context = context;
            _vpsService = vpsService;
            _userService = userService;
        }
        // GET: api/vps
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userService.GetUserId();
            var vpsList = await _vpsService.GetAllUserVPSAsync(userId);
            return Ok(vpsList);
        }

        // GET: api/vps/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
           var userId = _userService.GetUserId();
           var vps = await _vpsService.GetVPSByIdAsync(id, userId);

            if (vps == null)
                return NotFound();

            return Ok(vps);
        }

        // PUT: api/vps/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VPSCreateDto vpsDto)
        {
            var userId = _userService.GetUserId();
            var vps = await _vpsService.UpdateAsync(id, vpsDto, userId);
            if (vps == null)
                return NotFound();
            return Ok(vps);
        }

        // POST: api/vps
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VPSCreateDto vpsDto)
        {
            var userId = _userService.GetUserId();
            var vps = await _vpsService.CreateAsync(vpsDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = vps.Id }, vps);

        }

        // DELETE: api/vps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vps = await _vpsService.DeleteVPSAsync(id, _userService.GetUserId());

            if (!vps)
                return NotFound();

            return NoContent();
        }

        // POST: api/vps/check-connection
        [HttpPost("check-connection")]
        public async Task<IActionResult> CheckConnection([FromBody] VpsConnectionDto dto)
        {
            var success = await _vpsService.CheckConnectionAsync(dto);
            return Ok(success);
        }

        [HttpGet("system-info/{id}")]
        public async Task<IActionResult> GetSystemInfo(int id)
        {
            var info = await _vpsService.GetSystemInfoAsync(id);

            if (info == null)
                return NotFound();

            return Ok(info);
        }
    }
}
