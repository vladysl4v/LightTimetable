using System.Net.Http;
using System.Threading.Tasks;


namespace LightTimetable.Tools
{
    public static class HttpRequestService
    {
        /// <summary>
        /// Tries a certain number of attempts to get a response to the request
        /// </summary>
        /// <returns>A tuple consisting of the boolean value of the success and the string response</returns>
        public static async Task<(bool IsSuccessful, string Response)> LoadStringAsync(string url, int attemps = 5)
        {
            var outputStringResult = string.Empty;

            var outputRequestSuccessness = true;

            using var httpClient = new HttpClient();

            for (var retries = 0; retries < attemps; retries++)
            {
                try
                {
                    outputStringResult = await httpClient.GetStringAsync(url);
                }
                catch 
                {
                    if (retries == attemps - 1)
                    {
                        outputRequestSuccessness = false;
                    }
                    continue;
                }

                break;
            }

            return (IsSuccessful:outputRequestSuccessness, Response:outputStringResult);
        }

    }
}
