using Microsoft.Graph;
using Microsoft.Graph.Models;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Common;


namespace LightTimetable.Models.Services
{
    public sealed class TeamsEventsService : IEventsService
    {
        public static readonly int UtcOffset;
        private Dictionary<DateOnly, List<SpecificEvent>> _eventsData = null!;

        static TeamsEventsService()
        {
            UtcOffset = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time").
                        GetUtcOffset(DateTime.UtcNow).Hours;
        }

        public List<SpecificEvent> GetMeetingsInformation(DateTime date, TimeInterval timeInterval)
        {
            if (!_eventsData.TryGetValue(DateOnly.FromDateTime(date), out var dateEvents))
            {
                return new List<SpecificEvent>();
            }

            return dateEvents.Where((currEvent) => (
                currEvent.Time.Start >= timeInterval.Start &&
                currEvent.Time.Start < timeInterval.End &&
                currEvent.Link != null)).ToList();
        }

        public async Task InitializeAsync(DateTime start, DateTime end)
        {
            var graphClient = new GraphServiceClient(TeamsAuthManager.GetAuthenticationProvider());

            EventCollectionResponse? calendarData;

            try
            {
                calendarData = await graphClient.Me.Calendar.CalendarView.GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.StartDateTime = start.ToString("yyyy-MM-ddTHH:mm:ss");
                    requestConfiguration.QueryParameters.EndDateTime = end.ToString("yyyy-MM-ddT23:59:59");
                    requestConfiguration.QueryParameters.Filter = "isCancelled eq false";
                    requestConfiguration.QueryParameters.Top = 150;
                });

                if (calendarData == null)
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                _eventsData = new Dictionary<DateOnly, List<SpecificEvent>>();
                return;
            }
            
            var ungroupedData = new List<SpecificEvent>();

            foreach (var anEvent in calendarData.Value ?? new List<Event>())
            {
                var time = new TimeInterval(
                    TimeOnly.FromDateTime(anEvent.Start.ToDateTime()).AddHours(UtcOffset),
                    TimeOnly.FromDateTime(anEvent.End.ToDateTime()).AddHours(UtcOffset));
            
                ungroupedData.Add(new SpecificEvent(
                    time,
                    anEvent.Subject,
                    anEvent.OnlineMeeting?.JoinUrl,
                    DateOnly.FromDateTime(anEvent.Start.ToDateTime())
                ));
            }
            
            _eventsData = ungroupedData.GroupBy(x => x.Date)
                .ToDictionary(k => k.Key, v => v.ToList());
        }

    }
}
