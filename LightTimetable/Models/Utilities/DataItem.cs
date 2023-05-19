using System;
using System.Linq;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Properties;
using LightTimetable.Models.Services;

namespace LightTimetable.Models.Utilities
{
    public class DataItem
    {
        public uint Id { get; }
        public DateTime Date { get; }
        public TimeInterval StudyTime { get; }
        public DisciplineName Discipline { get; set; }
        public string StudyType { get; }
        public string Cabinet { get; }
        public string Employee { get; }
        public string Subgroup { get; }
        public string Note { get; set; }
        public ElectricityStatus? Electricity { get; }
        public List<OutlookEvent>? OutlookEvents { get; set; }

        public DataItem(DateTime date, TimeInterval studyTime, string discipline, string studyType, string employee,
            string cabinet, string subgroup = "")
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

            if (Settings.Default.ShowOutages)
            {
                Electricity = ElectricityPlugin.GetElectricityInformation(StudyTime, date.GetNormalDayOfWeek());
            }

            if (Settings.Default.ShowTeamsEvents)
            {
                OutlookEvents = TeamsEventsPlugin.GetSuitableEvents(date, StudyTime);
            }
        }

        public DataItem(DataItem clone)
        {
            Date = clone.Date;
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

        protected string GetNote()
        {
            return Settings.Default.Notes.TryGetValue(Id, out string note) ? note : string.Empty;
        }
    }
}
