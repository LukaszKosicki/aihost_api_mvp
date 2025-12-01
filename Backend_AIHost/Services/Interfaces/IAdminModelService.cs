using Backend_AIHost.DTOs.Admin.AIModel;

namespace Backend_AIHost.Services.Interfaces
{
    public interface IAdminModelService
    {
        Task<AIModelCreateDto> GetByIdAsync(int id); 
        Task<IEnumerable<AllModelsDto>> GetAllAsync();
        Task<AIModelCreateDto> UpdateAsync(int id, AIModelCreateDto dto);
        Task<AIModelCreateDto> CreateAsync(AIModelCreateDto dto);

    }
}
