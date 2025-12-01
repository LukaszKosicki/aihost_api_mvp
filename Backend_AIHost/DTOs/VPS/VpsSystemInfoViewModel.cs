namespace Backend_AIHost.DTOs.VPS
{
    public class VpsSystemInfoViewModel
    {
        public string Hostname { get; set; }
        public string Kernel { get; set; }
        public string Architecture { get; set; }
        public string IpAddress { get; set; }
        public string OsVersion { get; set; }
        public string FriendlyName { get; set; }

        public TimeSpan Uptime { get; set; }

        public long TotalDiskGb { get; set; }
        public long UsedDiskGb { get; set; }
        public long FreeDiskGb { get; set; }
        public int DiskUsagePercent { get; set; }

        public int TotalRamMb { get; set; }
        public int UsedRamMb { get; set; }
        public int FreeRamMb { get; set; }
        public int RamUsagePercent { get; set; }

        public int CpuCount { get; set; }
        public bool DockerInstalled { get; set; }
    }
}
