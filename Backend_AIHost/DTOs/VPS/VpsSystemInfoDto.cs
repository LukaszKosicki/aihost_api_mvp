namespace Backend_AIHost.DTOs.VPS
{
    public class VpsSystemInfoDto
    {
        public string SystemInfo { get; set; }
        public string Uptime { get; set; }
        public string DiskUsage { get; set; }
        public string MemoryUsage { get; set; }
        public string IpAddress { get; set; }
        public string OsVersion { get; set; }
        public string FriendlyName { get; set; }
        public int CpuCount { get; set; }
        public bool DockerInstalled { get; set; }
    }
}
