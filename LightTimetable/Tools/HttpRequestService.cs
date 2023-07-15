using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace LightTimetable.Tools
{
    /// <summary>
    /// Class that provides methods for http requests
    /// </summary>
    public static class HttpRequestService
    {
        private static readonly HttpClient _sharedClient;

        /// <remarks>
        /// Create HttpClient once so as not to cause port exhaust
        /// and limit PooledConnectionLifetime in case the DNS entries
        /// may change while the program is running
        /// </remarks>
        static HttpRequestService()
        {
            var httpHandler = new SocketsHttpHandler()
            {
                PooledConnectionLifetime = TimeSpan.FromHours(1)  
            };
            _sharedClient = new HttpClient(httpHandler);
        } 

        /// <summary>
        /// Asynchronously tries to download data for a given number of times
        /// </summary>
        /// <returns>Response body as a string or null if unsuccessful</returns>
        public static async Task<string?> LoadStringAsync(string url, int maxAttemps = 5, HttpClient httpClient = null)
        {
            httpClient ??= _sharedClient;
            string? outputStringResult = null;

            for (var retries = 0; retries < maxAttemps; retries++)
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
    }
}
