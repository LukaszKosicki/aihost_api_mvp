using Backend_AIHost.DTOs.VPS;
using Backend_AIHost.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Backend_AIHost.Services.Interfaces
{
    public interface IVpsService
    {
        Task<CheckVPSDataResult> CheckConnectionAsync(VpsConnectionDto dto);
        Task<VpsSystemInfoViewModel> GetSystemInfoAsync(int vpsId);
        Task<IEnumerable<VPSDto>> GetAllUserVPSAsync(string userId);
        Task<VPSDto> GetVPSByIdAsync(int id, string userId);
        Task<VPSDto> UpdateAsync(int id, VPSCreateDto dto, string userId);
        Task<VPSDto> CreateAsync(VPSCreateDto dto, string userId);
        Task<bool> DeleteVPSAsync(int id, string userId);
    }
}
