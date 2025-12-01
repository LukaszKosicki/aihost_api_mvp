using Backend_AIHost.DTOs.VPS;
using System.Text.RegularExpressions;

namespace Backend_AIHost.Mappers.VPS
{
    public static class VpsSystemInfoMapper
    {
        public static VpsSystemInfoViewModel Map(VpsSystemInfoDto raw)
        {
            // System info
            var parts = raw.SystemInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string hostname = parts.Length > 1 ? parts[1] : "unknown";
            string kernel = parts.Length > 2 ? parts[2] : "unknown";
            string arch = parts.FirstOrDefault(p => p.Contains("x86") || p.Contains("arm")) ?? "unknown";

            // Disk
            var diskParts = raw.DiskUsage.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            long totalDisk = ParseSize(diskParts.ElementAtOrDefault(1));
            long usedDisk = ParseSize(diskParts.ElementAtOrDefault(2));
            long freeDisk = ParseSize(diskParts.ElementAtOrDefault(3));
            int diskPercent = int.TryParse(diskParts.ElementAtOrDefault(4)?.Trim('%'), out var dp) ? dp : 0;

            // RAM
            // RAM (zakładamy wynik z `free -m`)
            var ramParts = raw.MemoryUsage.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Indeksy: total, used, free, shared, buff/cache, available
            int totalRam = int.Parse(ramParts.ElementAtOrDefault(1) ?? "0");
            int usedRam = int.Parse(ramParts.ElementAtOrDefault(2) ?? "0");
            int freeRam = int.Parse(ramParts.ElementAtOrDefault(3) ?? "0");
            int buffCacheRam = int.Parse(ramParts.ElementAtOrDefault(5) ?? "0");
            freeRam += buffCacheRam;
            int ramPercent = totalRam > 0 ? ((totalRam - freeRam) * 100 / totalRam) : 0;

            // Uptime
            var uptime = ParseUptime(raw.Uptime);

            return new VpsSystemInfoViewModel
            {
                IpAddress = raw.IpAddress,
                OsVersion = raw.OsVersion,
                FriendlyName = raw.FriendlyName,

                Hostname = hostname,
                Kernel = kernel,
                Architecture = arch,

                Uptime = uptime,

                TotalDiskGb = totalDisk,
                UsedDiskGb = usedDisk,
                FreeDiskGb = freeDisk,
                DiskUsagePercent = diskPercent,

                TotalRamMb = totalRam,
                UsedRamMb = usedRam,
                FreeRamMb = freeRam,
                RamUsagePercent = ramPercent,

                CpuCount = raw.CpuCount,
                DockerInstalled = raw.DockerInstalled
            };
        }

        private static long ParseSize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            return long.TryParse(value.Replace("G", ""), out var gbs) ? gbs : 0;
        }

        private static TimeSpan ParseUptime(string raw)
        {
            // Przykład: "up 27 weeks, 2 days, 22 hours, 36 minutes"
            var time = new TimeSpan();
            var matches = Regex.Matches(raw, @"(\d+)\s+(weeks|days|hours|minutes)");

            foreach (Match match in matches)
            {
                int val = int.Parse(match.Groups[1].Value);
                switch (match.Groups[2].Value)
                {
                    case "weeks": time = time.Add(TimeSpan.FromDays(val * 7)); break;
                    case "days": time = time.Add(TimeSpan.FromDays(val)); break;
                    case "hours": time = time.Add(TimeSpan.FromHours(val)); break;
                    case "minutes": time = time.Add(TimeSpan.FromMinutes(val)); break;
                }
            }

            return time;
        }
    }

}
