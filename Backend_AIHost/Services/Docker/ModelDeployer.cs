using Renci.SshNet;

namespace Backend_AIHost.Services.Docker
{
    public class ModelDeployer
    {
        public async Task DeployDockerAsync(string host, string username, string password, string modelInternalName, int port)
        {
            var dockerfileContent = DockerfileGenerator.GenerateDockerfileForTextGenerationModel(modelInternalName, port);
            var dockerfileName = $"Dockerfile_{modelInternalName}";
            var tempDockerfilePath = Path.Combine(Path.GetTempPath(), dockerfileName);

            await File.WriteAllTextAsync(tempDockerfilePath, dockerfileContent);

            using var client = new SshClient(host, username, password);
            using var scp = new ScpClient(host, username, password);

            client.Connect();
            scp.Connect();

            var remotePath = $"/home/{username}/{dockerfileName}";
            using var fileStream = File.OpenRead(tempDockerfilePath);
            scp.Upload(fileStream, remotePath);

            // Budujemy obraz
            var imageName = $"ai_{modelInternalName}";
            var containerName = $"container_{modelInternalName}";

            var dockerCommands = $"""
        cd /home/{username}
        docker build -t {imageName} -f {dockerfileName} .
        docker run -d -p {port}:11434 --name {containerName} {imageName}
        """;

            using var cmd = client.CreateCommand(dockerCommands);
            var result = cmd.Execute();

            Console.WriteLine("Wynik: " + result);

            // (Opcjonalnie) Usuń plik tymczasowy
            var cleanupCommand = $"rm /home/{username}/{dockerfileName}";
            using var cleanupCmd = client.CreateCommand(cleanupCommand);
            cleanupCmd.Execute();

            client.Disconnect();
            scp.Disconnect();

            File.Delete(tempDockerfilePath);
        }
    }
}
