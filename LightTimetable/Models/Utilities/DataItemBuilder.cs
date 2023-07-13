using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;

using LightTimetable.Tools;
using LightTimetable.Common;
using LightTimetable.Properties;


namespace LightTimetable.Models.Utilities
{
    public class DataItemBuilder
    {
        private IServiceProvider? _serviceProvider;
        private DataItem _item = null!;
        
        public DataItemBuilder()
        {
            this.Reset();
        }
        public void AddGlobalServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
            _item.Discipline = new DisciplineName(discipline);
            _item.StudyType = studyType;
            _item.Cabinet = cabinet;
            _item.Employee = ShortenFullName(employee); 
        }
        
        public void AddPromt(string promt)
        {
            _item.Promt = promt;
        }

        public void AddServices()
        {
            var electricityService = _serviceProvider?.GetService<IElectricityService>();
            var eventsService = _serviceProvider?.GetService<IEventsService>();

            if (eventsService != null)
            {
                var eventsData = eventsService.GetMeetingsInformation(_item.Date, _item.StudyTime);
                _item.Events = eventsData.Any() ? new EventsContainer(eventsData) : null;
            }

            if (electricityService != null)
            {
                var outagesData = electricityService.GetOutagesInformation(_item.StudyTime, _item.Date.GetNormalDayOfWeek());
                _item.Outages = outagesData.Any() ? new OutagesContainer(outagesData) : null;
            }
        }
 
        public DataItem Build()
        {
            _item.Id = CreateIdentifier();
            _item.Note = GetNote();
            if (_serviceProvider != null)
            {
                AddServices();
            }
            DataItem finalItem = _item; 
            this.Reset();
            
            return finalItem;
        }

        private void Reset()
        {
            _item = new DataItem();
        }

        private string GetNote()
        {
            return Settings.Default.Notes.TryGetValue(_item.Id, out string note) ? note : string.Empty;
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