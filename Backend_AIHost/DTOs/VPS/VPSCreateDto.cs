namespace Backend_AIHost.DTOs.VPS
{
    public class VPSCreateDto
    {
        public string IP { get; set; }
        public int Port { get; set; } = 22;
        public string Username { get; set; }
        public string Password { get; set; }
        public string FriendlyName { get; set; }
    }
}
