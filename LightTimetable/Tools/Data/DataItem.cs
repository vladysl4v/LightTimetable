using System;
using System.Collections.Generic;

using LightTimetable.Tools.Electricity;

using static LightTimetable.Properties.Settings;


namespace LightTimetable.Tools.Data
{
    public class DataItem
    {
        public uint ID { get; }
        public ElecticityStatus? Electricity { get; set; }
        public TimeInterval StudyTime { get; set; }
        public MutablePair<string, string> Discipline { get; set; }
        public string StudyType { get; set; }
        public string Cabinet { get; set; }
        public string Employee { get; set; }
        public string Subgroup { get; set; }
        public string Note { get; set; }

        public DataItem(Dictionary<string, string> item, ElectricityProvider elecProvider)
        {
            StudyTime = new TimeInterval(item["study_time"]);
            Electricity = elecProvider.GetLightInformation(StudyTime, item["full_date"]);
            Discipline = RenameDiscipline(item["discipline"]);
            StudyType = item["study_type"];
            Cabinet = item["cabinet"];
            Subgroup = item["study_subgroup"];
            Employee = ShortenFullName(item["employee"]);
            ID = CreateIdentifier(item["study_time"], item["discipline"], item["study_type"], item["full_date"]);
            Note = GetNote(ID);
        }
        private string GetNote(uint identifier)
        {
            return Default.Notes.TryGetValue(identifier, out string note) ? note : string.Empty;
        }

        private uint CreateIdentifier(string studyTime, string discipline, string studyType, string date)
        {
            string encodeDate = date[0..2];
            string encodeStudyTime = studyTime[0..5].Replace(":", "");
            string encodeDiscipline = Convert.ToString(discipline.Length);
            string encodeStudyType = Convert.ToString(studyType.Length);
            return uint.Parse(encodeDate + encodeStudyTime + encodeDiscipline + encodeStudyType);
        }

        private MutablePair<string, string> RenameDiscipline(string discipline)
        {
            if (Default.Renames.TryGetValue(discipline, out string rename))
            {
                return new MutablePair<string, string>(rename, discipline);
            }
            else
            {
                return new MutablePair<string, string>(discipline, discipline);
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

    }
}
