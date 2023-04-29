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
        public ElectricityStatus? Electricity { get; }
        public TimeInterval StudyTime { get; }
        public DisciplinePair Discipline { get; set; }
        public string StudyType { get; }
        public string Cabinet { get; }
        public string Employee { get; }
        public string Subgroup { get; }
        public string Note { get; set; }
        public List<OutlookEvent>? OutlookEvents { get; set; }

        public DataItem(Dictionary<string, string>? item)
        {
            var date = Convert.ToDateTime(item["full_date"]);

            StudyTime = new TimeInterval(TimeOnly.Parse(item["study_time_begin"]), TimeOnly.Parse(item["study_time_end"]));
            Discipline = RenameDiscipline(item["discipline"]);
            StudyType = item["study_type"];
            Cabinet = item["cabinet"];
            Subgroup = item["study_subgroup"];
            Employee = ShortenFullName(item["employee"]);
            Id = CreateIdentifier(item["full_date"]);
            Note = GetNote();

            if (Settings.Default.ShowBlackouts)
            {
                Electricity = ElectricityPlugin.GetLightInformation(StudyTime, date.GetNormalDayOfWeek());
            }

            if (Settings.Default.ShowTeamsEvents)
            {
                OutlookEvents = TeamsEventsPlugin.GetSuitableEvents(date, StudyTime);
            }
        }

        public DataItem(DataItem clone, DateTime date)
        {
            StudyTime = clone.StudyTime;
            Electricity = clone.Electricity;
            Discipline = clone.Discipline;
            StudyType = clone.StudyType;
            Cabinet = clone.Cabinet;
            Subgroup = clone.Subgroup;
            Employee = clone.Employee;

            Id = CreateIdentifier(date.ToShortDateString());
            Note = GetNote();
        }

        private uint CreateIdentifier(string date)
        {
            var hash1 = int.Parse(date[8..10] + date[3..5] + date[0..2]);
            var hash2 = Discipline.Original.Aggregate(0, (current, letter) => current + letter);
            var hash3 = StudyType.Length * Discipline.Original.Length;
            return (uint)(hash1 + hash2 + hash3);
        }

        private DisciplinePair RenameDiscipline(string discipline)
        {
            if (Settings.Default.Renames.TryGetValue(discipline, out string rename))
            {
                return new DisciplinePair(rename, discipline);
            }
            else
            {
                return new DisciplinePair(discipline, discipline);
            }
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
