using Newtonsoft.Json.Linq;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Models.Utilities;


namespace LightTimetable.Models.Services
{
    public class TeamsEventsService
    {
        private Dictionary<DateOnly, List<OutlookEvent>>? _eventsData;

        public List<OutlookEvent>? GetSuitableEvents(DateTime date, TimeInterval timeInterval)
        {
            if (_eventsData == null)
                return null;

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

        public async Task InitializeTeamsCalendarAsync(DateTime start, DateTime end)
        {
            var authResult = await TeamsAuthManager.RequestAuthenticationToken();

            if (authResult == null)
            {
                _eventsData = null;
                return;
            }

            var outputList = new List<OutlookEvent>();

            var startString = start.ToString("yyyy-MM-ddTHH:mm:ss");
            var endString = end.ToString("yyyy-MM-ddTHH:mm:ss");

            var url = $"https://graph.microsoft.com/v1.0/users/{authResult.Account.Username}" +
                      $"/calendar/calendarView?" +
                      $"startDateTime={startString}&" +
                      $"endDateTime={endString}&" +
                      $"$filter=isCancelled eq false";

            // 35 - initialization limit
            for (var i = 0; i < 35; i++)
            {
                var request = await HttpRequestService.GetUsingAuthenticationAsync(url, authResult.AccessToken);

                if (request == string.Empty)
                {
                    _eventsData = null;
                    return;
                }

                var currGroup = JObject.Parse(request);

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
            var outputDictionary = outputList.GroupBy(x => x.Date).ToDictionary(k => k.Key, v => v.ToList());

            _eventsData = outputDictionary;
        }

    }
}
