using System;
using System.Linq;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.Services.Models;
using LightTimetable.Models.Extensions;
using LightTimetable.Services.Abstractions;


namespace LightTimetable.ScheduleSources
{
    public class DataItemBuilder
    {
        private readonly IElectricityService? _electricityService;
        private readonly IEventsService? _eventsService;
        private readonly IUserSettings _settings;

        private DataItem _item;

        public DataItemBuilder(IUserSettings settings, IElectricityService? electricityService = null, IEventsService? eventsService = null)
        {
            _electricityService = electricityService;
            _eventsService = eventsService;
            _settings = settings;

            _item = new DataItem();
        }

        public void AddTimePeriod(DateTime date, TimeInterval time)
        {
            _item.Date = date;
            _item.StudyTime = time;
        }

        public void AddTimePeriod(DateTime date, TimeOnly startTime, TimeSpan lessonTime)
        {
            _item.Date = date;
            _item.StudyTime = new TimeInterval(startTime, startTime.Add(lessonTime));
        }

        public void AddBasicInformation(string discipline, string studyType, string cabinet, string employee)
        {
            _item.Discipline = new DisciplineName(discipline, _settings);
            _item.StudyType = studyType;
            _item.Cabinet = cabinet;
            _item.Employee = ShortenFullName(employee);
        }

        public void AddPromt(string promt)
        {
            _item.Promt = promt;
        }

        private void AddServices()
        {
            if (_eventsService != null)
            {
                if (_item.Date == default || _item.StudyTime.Start == default)
                {
                    throw new NullReferenceException("The time period is not initialized.");
                }
                var eventsData = _eventsService.GetMeetingsInformation(_item.Date, _item.StudyTime);
                _item.Events = eventsData.Any() ? new EventsContainer(eventsData) : null;
            }

            if (_electricityService != null)
            {
                if (_item.Date == default || _item.StudyTime.Start == default)
                {
                    throw new NullReferenceException("The time period is not initialized.");
                }
                var outagesData = _electricityService.GetOutagesInformation(_item.StudyTime, _item.Date.GetNormalDayOfWeek());
                _item.Outages = outagesData.Any() ? new OutagesContainer(outagesData) : null;
            }
        }

        public DataItem Build()
        {
            _item.Id = CreateIdentifier();
            _item.Note = GetNote();
            AddServices();
            DataItem finalItem = _item;
            Reset();
            return finalItem;
        }

        private void Reset()
        {
            _item = new DataItem();
        }

        private string GetNote()
        {
            return _settings.Notes.TryGetValue(_item.Id, out string note) ? note : string.Empty;
        }

        private uint CreateIdentifier()
        {
            var stringDate = _item.Date.ToShortDateString();
            var hash1 = int.Parse(stringDate[8..10] + stringDate[3..5] + stringDate[0..2]);
            var hash2 = _item.Discipline.Original.Aggregate(0, (current, letter) => current + letter);
            var hash3 = _item.StudyType.Length * _item.Discipline.Original.Length;
            return (uint)(hash1 + hash2 + hash3);
        }

        private string ShortenFullName(string employee)
        {
            if (string.IsNullOrEmpty(employee))
                return employee;

            string[] EmplSplitted = employee.Split();
            // if the string is not of format "Surname Name Patronymic"
            return EmplSplitted.Length != 3 ? employee : $"{EmplSplitted[0]} {EmplSplitted[1][0]}.{EmplSplitted[2][0]}.";
        }
    }
}
