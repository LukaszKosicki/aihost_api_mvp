using Backend_AIHost.Data;
using Backend_AIHost.DTOs.Admin.AIModel;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_AIHost.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("[controller]")]
    public class AdminModelsController : ControllerBase
    {
        private readonly ILogger<VPSController> _logger;
        private readonly AppDbContext _context;
        private readonly IAdminModelService _adminModelService;

        public AdminModelsController(ILogger<VPSController> logger, AppDbContext context, IAdminModelService adminModelService)
        {
            _logger = logger;
            _context = context;
            _adminModelService = adminModelService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var model = _adminModelService.GetByIdAsync(id);
            if (model == null)
                return NotFound();
            return Ok(model);
        }

        // GET: api/adminModels
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var modelList = await _adminModelService.GetAllAsync();
            if (modelList == null || !modelList.Any())
                return NoContent();

            return Ok(modelList);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AIModelCreateDto dto)
        {
            var model = await _adminModelService.UpdateAsync(id, dto);
            if (model == null)
                return NotFound();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AIModelCreateDto dto)
        {
            await _adminModelService.CreateAsync(dto);
            return Ok();
        }
    }
}
