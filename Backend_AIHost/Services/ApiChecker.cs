namespace Backend_AIHost.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ApiChecker
    {
        private readonly HttpClient _httpClient;

        public ApiChecker(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckHealthAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);

                return response.IsSuccessStatusCode
                       || response.StatusCode == System.Net.HttpStatusCode.BadRequest
                       || response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed
                       || response.StatusCode == System.Net.HttpStatusCode.Unauthorized; // np. też 401

            }
            catch
            {
                return false; // np. brak połączenia
            }
        }
    }

}
