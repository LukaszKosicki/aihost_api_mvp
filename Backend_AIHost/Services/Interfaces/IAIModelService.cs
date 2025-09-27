using Backend_AIHost.DTOs.AIModel;
using Backend_AIHost.Models.Responses;

namespace Backend_AIHost.Services.Interfaces
{
    public interface IAIModelService
    {
        Task<DeployResult> DeployModelAsync(DeployModelDto dto);
        Task<IEnumerable<AvailableModelDto>> GetListAvailableModelsAsync();
    }
}
