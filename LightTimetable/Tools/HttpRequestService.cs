using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace LightTimetable.Tools
{
    public static class HttpRequestService
    {
        public static async Task<string> LoadStringAsync(string url, int attemps = 5)
        {
            var outputStringResult = string.Empty;

            using var httpClient = new HttpClient();

            for (var retries = 0; retries < attemps; retries++)
            {
                try
                {
                    outputStringResult = await httpClient.GetStringAsync(url);
                }
                catch 
                {
                    continue;
                }

                break;
            }

            return outputStringResult;
        }

        public static async Task<string> GetUsingAuthenticationAsync(string url, string authHeader)
        {
            using var httpClient = new HttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", authHeader);
            try
            {
                var httpResponse = await httpClient.SendAsync(requestMessage);
                var outputStringResult = await httpResponse.Content.ReadAsStringAsync();
                return outputStringResult;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
