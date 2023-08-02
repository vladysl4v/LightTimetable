using Microsoft.Graph;
using Microsoft.Graph.Models;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.Services.Models;
using LightTimetable.Handlers.Abstractions;
using LightTimetable.Services.Abstractions;



namespace LightTimetable.Services
{
    public sealed class TeamsEventsService : IEventsService
    {
        private readonly int _utcOffset;
        private Dictionary<DateOnly, List<SpecificEvent>> _eventsData = null!;

        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        public TeamsEventsService(IUserSettings settings, IDateTimeHandler dates)
        {
            _utcOffset = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time").
                GetUtcOffset(DateTime.UtcNow).Hours;

            _startDate = dates.AvailableDates.First();
            _endDate = dates.AvailableDates.Last();

            if (!settings.ShowOldEvents)
            {
                _startDate = DateTime.Today < _endDate ? DateTime.Today : _endDate;
            }
        }

        public List<SpecificEvent> GetMeetingsInformation(DateTime date, TimeInterval timeInterval)
        {
            if (!_eventsData.TryGetValue(DateOnly.FromDateTime(date), out var dateEvents))
            {
                return new List<SpecificEvent>();
            }

            return dateEvents.Where((currEvent) => 
                currEvent.Time.Start >= timeInterval.Start &&
                currEvent.Time.Start < timeInterval.End &&
                currEvent.Link != null).ToList();
        }

        public async Task InitializeAsync()
        {
            var graphClient = new GraphServiceClient(TeamsAuthManager.GetAuthenticationProvider());

            EventCollectionResponse? calendarData;

            try
            {
                calendarData = await graphClient.Me.Calendar.CalendarView.GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.StartDateTime = _startDate.ToString("yyyy-MM-ddTHH:mm:ss");
                    requestConfiguration.QueryParameters.EndDateTime = _endDate.ToString("yyyy-MM-ddT23:59:59");
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
                    TimeOnly.FromDateTime(anEvent.Start.ToDateTime()).AddHours(_utcOffset),
                    TimeOnly.FromDateTime(anEvent.End.ToDateTime()).AddHours(_utcOffset));

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
