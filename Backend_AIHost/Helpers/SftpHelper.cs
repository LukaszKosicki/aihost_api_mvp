using Renci.SshNet;

namespace Backend_AIHost.Helpers
{
    public static class SftpHelper
    {
        public static void UploadFile(string host, int port, string username, string password, string content, string remotePath)
        {
            using var sftp = new SftpClient(host, port, username, password);
            sftp.Connect();

            using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            sftp.UploadFile(ms, remotePath, true);

            sftp.Disconnect();
        }
    }

}
