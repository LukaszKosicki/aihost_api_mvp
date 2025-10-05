namespace Backend_AIHost.Models
{
    public class VPS
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string UserId { get; set; }
        public string FriendlyName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void Update(string ip, int port, string friendlyName, string userName, string password)
        {
            IP = ip;
            Port = port;
            FriendlyName = friendlyName;
            Username = userName;
            Password = password;
        }
    }
}
