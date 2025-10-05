using Backend_AIHost.Data;
using Backend_AIHost.DTOs.Admin.AIModel;
using Backend_AIHost.Models;
using Backend_AIHost.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend_AIHost.Services
{
    public class AdminModelService : IAdminModelService
    {
        private readonly AppDbContext _context;
        public AdminModelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AIModelCreateDto> CreateAsync(AIModelCreateDto dto)
        {
            var model = new AIModel
            {
                Name = dto.Name,
                MinRequiredRamMb = dto.MinRequiredRamMb,
                IsActive = dto.IsActive,
                SupportsGPU = dto.SupportsGPU,
                ModelInternalName = dto.ModelInternalName,
                DefaultPort = dto.DefaultPort
            };
            _context.AIModels.Add(model);
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<AIModelCreateDto> UpdateAsync(int id, AIModelCreateDto dto)
        {
            var model = await _context.AIModels.FindAsync(id);
            if (model == null)
                return null;

            model.Name = dto.Name;
            model.MinRequiredRamMb = dto.MinRequiredRamMb;
            model.IsActive = dto.IsActive;
            model.SupportsGPU = dto.SupportsGPU;
            model.ModelInternalName = dto.ModelInternalName;
            model.DefaultPort = dto.DefaultPort;
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<IEnumerable<AllModelsDto>> GetAllAsync()
        {
            return await _context.AIModels
                .Select(v => new AllModelsDto
                {
                    Id = v.Id,
                    Port = v.DefaultPort,
                    Name = v.Name,
                    MinRequiredRamMb = v.MinRequiredRamMb
                })
                .ToListAsync();
        }

        public async Task<AIModelCreateDto> GetByIdAsync(int id)
        {
            return await _context.AIModels
                 .Where(v => v.Id == id)
                 .Select(v => new AIModelCreateDto
                 {
                     Name = v.Name,
                     MinRequiredRamMb = v.MinRequiredRamMb,
                     IsActive = v.IsActive,
                     SupportsGPU = v.SupportsGPU,
                     ModelInternalName = v.ModelInternalName,
                     DefaultPort = v.DefaultPort
                 })
                 .FirstOrDefaultAsync();
        }
    }
}
