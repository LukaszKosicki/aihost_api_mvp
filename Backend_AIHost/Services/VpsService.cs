using Backend_AIHost.Data;
using Backend_AIHost.DTOs.VPS;
using Backend_AIHost.Mappers.VPS;
using Backend_AIHost.Models.Responses;
using Backend_AIHost.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;

namespace Backend_AIHost.Services
{
    public class VpsService : IVpsService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public VpsService(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<bool> DeleteVPSAsync(int id, string userId)
        {
            var vps = await _context.VPSes
                .Where(v => v.Id == id && v.UserId == userId)
                .FirstOrDefaultAsync();
            if (vps == null)
                return false;
            _context.VPSes.Remove(vps);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<VPSDto> CreateAsync(VPSCreateDto dto, string userId)
        {
            var vps = new Models.VPS
            {
                IP = dto.IP,
                Port = dto.Port,
                Username = dto.Username,
                Password = dto.Password,
                FriendlyName = dto.FriendlyName,
                UserId = userId
            };
            _context.VPSes.Add(vps);
            await _context.SaveChangesAsync();
            return new VPSDto
            {
                Id = vps.Id,
                IP = vps.IP,
                Port = vps.Port,
                UserName = vps.Username,
                FriendlyName = vps.FriendlyName
            };
        }

        public async Task<VPSDto> UpdateAsync(int id, VPSCreateDto dto, string userId)
        {
            var vps = await _context.VPSes
                .Where(v => v.Id == id && v.UserId == userId)
                .FirstOrDefaultAsync();

            if (vps == null)
                return null;

            vps.Update(dto.IP, dto.Port, dto.FriendlyName, dto.Username, dto.Password);
            _context.VPSes.Update(vps);
            await _context.SaveChangesAsync();

            return new VPSDto
            {
                Id = vps.Id,
                IP = vps.IP,
                Port = vps.Port,
                UserName = vps.Username,
                FriendlyName = vps.FriendlyName
            };
        }

        public async Task<VPSDto> GetVPSByIdAsync(int id, string userId)
        {
            var vps = await _context.VPSes
                .Where(v => v.Id == id && v.UserId == userId)
                .Select(v => new VPSDto
                {
                    Id = v.Id,
                    IP = v.IP,
                    Port = v.Port,
                    UserName = v.Username,
                    FriendlyName = v.FriendlyName
                }).FirstOrDefaultAsync();

            return vps;
        }
        public async Task<IEnumerable<VPSDto>> GetAllUserVPSAsync(string userId)
        {
           var vpsList = await _context.VPSes.Where(v => v.UserId == userId)
                .Select(v => new VPSDto
                {
                    Id = v.Id,
                    IP = v.IP,
                    Port = v.Port,
                    UserName = v.Username,
                    FriendlyName = v.FriendlyName
                }).ToListAsync();
            return vpsList;
        }
        public async Task<CheckVPSDataResult> CheckConnectionAsync(VpsConnectionDto dto)
        {
            try
            {
                using var client = new SshClient(dto.Host, dto.Port, dto.UserName, dto.Password);
                client.Connect();

                var result = client.IsConnected;
                client.Disconnect();

                return new CheckVPSDataResult
                {
                    Description = "Congratulations! Your details are correct. You have successfully connected to your VPS.",
                    Title = "Connection successful",
                    Variant = "success"
                };
            }
            catch
            {
                return new CheckVPSDataResult
                {
                    Description = "The data provided is incorrect. It was not possible to connect to the VPS based on this information.",
                    Title = "Connection failed!",
                    Variant = "error"
                };
            }
        }

        public async Task<VpsSystemInfoViewModel> GetSystemInfoAsync(int vpsId)
        {
            var vps = await _context.VPSes.FirstOrDefaultAsync(e => e.Id == vpsId && e.UserId == _userService.GetUserId());

            if (vps == null) return null;

            using var client = new SshClient(vps.IP, vps.Port, vps.Username, vps.Password);

            try
            {
                client.Connect();

                var result = new VpsSystemInfoDto
                {
                    SystemInfo = Run(client, "uname -a"),
                    Uptime = Run(client, "uptime -p"),
                    DiskUsage = Run(client, "df -h / | tail -n 1"),
                    MemoryUsage = Run(client, "free -m | grep Mem"),
                    CpuCount = int.Parse(Run(client, "nproc")),
                    DockerInstalled = !Run(client, "docker --version").ToLower().Contains("not found"),
                    IpAddress = Run(client, "hostname -I | awk '{print $1}'"),
                    OsVersion = Run(client, "grep PRETTY_NAME /etc/os-release | cut -d '\"' -f2")
                };

                client.Disconnect();

                if (result != null) return VpsSystemInfoMapper.Map(result); 
                return null;
            }
            catch
            {
                return null; // lub rzuć wyjątek
            }
        }

        private string Run(SshClient client, string command)
        {
            var cmd = client.RunCommand(command);
            return cmd.Result.Trim();
        }
    }
}
