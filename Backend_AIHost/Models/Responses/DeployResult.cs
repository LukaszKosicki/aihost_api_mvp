namespace Backend_AIHost.Models.Responses
{
    public class DeployResult
    {
        public bool Success { get; }
        public string Message { get; }

        public DeployResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
