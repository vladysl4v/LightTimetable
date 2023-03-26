using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Models.Electricity;

using static LightTimetable.Properties.Settings;


namespace LightTimetable.Models
{
    public class DataItem
    {
        public uint Id { get; }
        public ElecticityStatus? Electricity { get; }
        public TimeInterval StudyTime { get; }
        public DisciplinePair Discipline { get; set; }
        public string StudyType { get; }
        public string Cabinet { get; }
        public string Employee { get; }
        public string Subgroup { get; }
        public string Note { get; set; }

        public DataItem(Dictionary<string, string> item, string[][] electricity)
        {
            StudyTime = new TimeInterval(item["study_time"]);
            Electricity = GetLightInformation(electricity);
            Discipline = RenameDiscipline(item["discipline"]);
            StudyType = item["study_type"];
            Cabinet = item["cabinet"];
            Subgroup = item["study_subgroup"];
            Employee = ShortenFullName(item["employee"]);
            Id = CreateIdentifier(item["full_date"]);
            Note = GetNote();
        }

        private ElecticityStatus? GetLightInformation(string[][] intersections)
        {
            if (!Default.ShowBlackouts)
                return null;

            var isDefinitelyBlackout = false;

            StringBuilder result = new StringBuilder();
            result.Append("Ймовірні відключення:");
            if (intersections[1].Any())
            {
                int startHour = int.Parse(intersections[1].First()) - 1;
                result.Append($"\n{startHour}:00-{intersections[1].Last()}:00 - електроенергії не буде");
                isDefinitelyBlackout = true;
            }
            else if (intersections[0].Any() && Default.ShowPossibleBlackouts)
            {
                int startHour = int.Parse(intersections[0].First()) - 1;
                result.Append($"\n{startHour}:00-{intersections[0].Last()}:00 - можливе відключення");
            }

            if (!isDefinitelyBlackout && !Default.ShowPossibleBlackouts)
                return null;

            return new ElecticityStatus(result.ToString(), isDefinitelyBlackout);
        }

        private uint CreateIdentifier(string date)
        {
            string encodeDate = date[0..2];
            string encodeStudyTime = StudyTime.ToString()[0..5].Replace(":", "");
            string encodeDiscipline = Convert.ToString(Discipline.Modified.Length);
            string encodeStudyType = Convert.ToString(StudyType.Length);
            return uint.Parse(encodeDate + encodeStudyTime + encodeDiscipline + encodeStudyType);
        }

        private DisciplinePair RenameDiscipline(string discipline)
        {
            if (Default.Renames.TryGetValue(discipline, out string rename))
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
            if (employee == string.Empty || employee == null)
                return employee;

            string[] EmplSplitted = employee.Split();
            // if the string is not of format "Surname Name Patronymic"
            return EmplSplitted.Length != 3 ? employee : $"{EmplSplitted[0]} {EmplSplitted[1][0]}.{EmplSplitted[2][0]}.";
        }

        protected string GetNote()
        {
            return Default.Notes.TryGetValue(Id, out string note) ? note : string.Empty;
        }
    }
}
