using Newtonsoft.Json;

using System;
using System.Globalization;
using System.Collections.Generic;

using LightTimetable.Tools;


namespace LightTimetable.Models.Utilities
{
    public class OutlookEvent
    {
        private static readonly int UtcOffset = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time").GetUtcOffset(DateTime.UtcNow).Hours;
        public string Title { get; set; }
        public DateOnly Date { get; set; }
        public TimeInterval Time { get; set; }
        public string? Url { get; set; }

        [JsonConstructor]
        public OutlookEvent(string subject, Dictionary<string, string> start, Dictionary<string, string> end, Dictionary<string, string> onlineMeeting)
        {
            const string datetimeFormat = "MM'/'dd'/'yyyy' 'HH':'mm':'ss";
            
            var startTime = TimeOnly.ParseExact(start["dateTime"], datetimeFormat, CultureInfo.CurrentCulture).AddHours(UtcOffset);
            var endTime = TimeOnly.ParseExact(end["dateTime"], datetimeFormat, CultureInfo.CurrentCulture).AddHours(UtcOffset);

            Date = DateOnly.ParseExact(end["dateTime"], datetimeFormat, CultureInfo.CurrentCulture);
            Time = new TimeInterval(startTime, endTime);
            Url = onlineMeeting.TryGetValue("joinUrl", out var url) ? url : null;
            Title = subject;
        }
    }
}
