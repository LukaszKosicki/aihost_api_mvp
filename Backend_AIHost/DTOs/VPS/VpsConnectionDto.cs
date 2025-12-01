namespace Backend_AIHost.DTOs.VPS
{
    public class VpsConnectionDto
    {
        public string Host { get; set; }
        public int Port { get; set; } = 22;
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
