using Newtonsoft.Json;
using Microsoft.Graph.Models;

using System;
using System.Linq;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Properties;


namespace LightTimetable.Models.Utilities
{
    public class DataItem
    {
        public uint Id { get; init; }
        public DateTime Date { get; init; }
        public TimeInterval StudyTime { get; init; }
        public DisciplineName Discipline { get; init; }
        public string StudyType { get; init; }
        public string Cabinet { get; init; } 
        public string Employee { get; init; }
        public string Subgroup { get; init; }
        public string Note { get; set; }
        public ElectricityStatus? Electricity { get; init; }
        [JsonIgnore]
        public List<Event>? OutlookEvents { get; }

        public DataItem() { }

        public DataItem(DateTime date, TimeInterval studyTime, string discipline,
               string studyType, string employee, string cabinet, string subgroup = "",
               Func<TimeInterval, NormalDayOfWeek, ElectricityStatus?>? electricityLoader = null,
               Func<DateTime, TimeInterval, List<Event>?>? teamsEventsLoader = null)
        {
            Date = date;
            StudyTime = studyTime;
            Discipline = new DisciplineName(discipline);
            StudyType = studyType;
            Subgroup = subgroup;
            Cabinet = cabinet;
            Employee = ShortenFullName(employee);
            Id = CreateIdentifier();
            Note = GetNote();
            
            if (electricityLoader != null)
            {
                Electricity = electricityLoader(StudyTime, date.GetNormalDayOfWeek());
            }
            if (teamsEventsLoader != null)
            {
                OutlookEvents = teamsEventsLoader(date, StudyTime);
            }
        }

        public DataItem(DataItem clone, DateTime date)
        {
            Date = date;
            StudyTime = clone.StudyTime;
            Electricity = clone.Electricity;
            Discipline = clone.Discipline;
            StudyType = clone.StudyType;
            Cabinet = clone.Cabinet;
            Subgroup = clone.Subgroup;
            Employee = clone.Employee;
            
            Id = CreateIdentifier();
            Note = GetNote();
        }

        private uint CreateIdentifier()
        {
            var stringDate = Date.ToShortDateString();
            var hash1 = int.Parse(stringDate[8..10] + stringDate[3..5] + stringDate[0..2]);
            var hash2 = Discipline.Original.Aggregate(0, (current, letter) => current + letter);
            var hash3 = StudyType.Length * Discipline.Original.Length;
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

        private string GetNote()
        {
            return Settings.Default.Notes.TryGetValue(Id, out string note) ? note : string.Empty;
        }
    }
}
