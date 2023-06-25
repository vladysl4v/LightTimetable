using Microsoft.Graph;
using Microsoft.Graph.Models;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Tools;


namespace LightTimetable.Models.Services
{
    public class TeamsEventsService
    {
        public static readonly int UtcOffset;
        private Dictionary<DateOnly, List<Event>>? _eventsData;

        static TeamsEventsService()
        {
            UtcOffset = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time").
                        GetUtcOffset(DateTime.UtcNow).Hours;
        }

        public List<Event>? GetSuitableEvents(DateTime date, TimeInterval timeInterval)
        {
            if (_eventsData == null ||
               !_eventsData.TryGetValue(DateOnly.FromDateTime(date), out var dateEvents))
            {
                return null;
            }

            var startDate = date.Date.Add(timeInterval.Start.AddHours(-UtcOffset).ToTimeSpan()); 
            var endDate = date.Date.Add(timeInterval.End.AddHours(-UtcOffset).ToTimeSpan()); 

            var outputList = dateEvents.Where((teamsEvent) => (
                teamsEvent.Start.ToDateTime() >= startDate &&
                teamsEvent.Start.ToDateTime() < endDate &&
                teamsEvent.OnlineMeeting?.JoinUrl != null)).ToList();

            return outputList.Any() ? outputList : null;
        }

        public async Task InitializeTeamsCalendarAsync(DateTime start, DateTime end)
        {
            var graphClient = new GraphServiceClient(TeamsAuthManager.GetAuthenticationProvider());

            if (graphClient == null)
            {
                _eventsData = null;
                return;
            }

            var result = await graphClient.Me.Calendar.CalendarView.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.StartDateTime = start.ToString("yyyy-MM-ddTHH:mm:ss");
                requestConfiguration.QueryParameters.EndDateTime = end.ToString("yyyy-MM-ddT23:59:59");
                requestConfiguration.QueryParameters.Filter = "isCancelled eq false";
                requestConfiguration.QueryParameters.Top = 150;
            });
            
            if (result == null)
            {
                _eventsData = null;
                return;   
            }

            _eventsData = result.Value?.GroupBy(x => DateOnly.ParseExact(x.Start.DateTime, "yyyy-MM-ddTHH:mm:ss.0000000"))
                          .ToDictionary(k => k.Key, v => v.ToList());
        }

    }
}
