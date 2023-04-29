using Newtonsoft.Json.Linq;
using Microsoft.Identity.Client;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Models.Utilities;


namespace LightTimetable.Models.Services
{
    public static class TeamsEventsPlugin
    {
        private static Dictionary<DateOnly, List<OutlookEvent>>? _eventsData;
        private static readonly IPublicClientApplication ClientApp;

        private const string AppId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

        static TeamsEventsPlugin()
        {
            ClientApp = PublicClientApplicationBuilder.Create(AppId).Build();
            TokenCacheHelper.EnableSerialization(ClientApp.UserTokenCache);
        }

        public static List<OutlookEvent>? GetSuitableEvents(DateTime date, TimeInterval timeInterval)
        {
            var outputList = new List<OutlookEvent>();

            var dateOnly = DateOnly.FromDateTime(date);

            if (!_eventsData.TryGetValue(dateOnly, out var dateEvents))
            {
                return null;
            }
            foreach (var anEvent in dateEvents)
            {
                if (anEvent.Time.Start < timeInterval.Start ||
                    anEvent.Time.Start > timeInterval.End ||
                    anEvent.Url == null)
                {
                    continue;
                }

                outputList.Add(anEvent);
            }

            return outputList.Any() ? outputList : null;
        }

        public static async Task InitializeTeamsCalendarAsync(DateTime start, DateTime end)
        {
            var authResult = await RequestAuthenticationToken();

            var outputList = new List<OutlookEvent>();

            var startString = start.ToString("yyyy-MM-ddTHH:mm:ss");
            var endString = end.ToString("yyyy-MM-ddTHH:mm:ss");

            var url = $"https://graph.microsoft.com/v1.0/users/{authResult.Account.Username}" +
                      $"/calendar/calendarView?" +
                      $"startDateTime={startString}&" +
                      $"endDateTime={endString}&" +
                      $"$filter=isCancelled eq false";

            for (var i = 0; i < 50; i++)
            {
                var request = await HttpRequestService.GetUsingAuthenticationAsync(url, authResult.AccessToken);

                var currGroup = JObject.Parse(request.Response);

                if (currGroup["value"] == null || !currGroup["value"].Any())
                {
                    break;
                }

                outputList.AddRange(currGroup["value"].ToObject<IEnumerable<OutlookEvent>>());

                if (!currGroup.TryGetValue("@odata.nextLink", out var nextLink))
                {
                    break;
                }

                url = nextLink.ToString();
            }
            var outputDictionary = outputList.GroupBy(x => x.Date).ToDictionary(g => g.Key, g => g.ToList());

            _eventsData = outputDictionary;
        }

        private static async Task<AuthenticationResult> RequestAuthenticationToken()
        {
            string[] scopes = { "Calendars.Read.Shared" };

            var accounts = await ClientApp.GetAccountsAsync();

            AuthenticationResult result;
            try
            {
                result = await ClientApp.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                result = await ClientApp.AcquireTokenInteractive(scopes).ExecuteAsync();
            }

            return result;
        }

    }
}
