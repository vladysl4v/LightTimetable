using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Timetable.Utilities
{
    public class Lesson
    {
        public uint ID { get; private set; }
        public ElectricityStatus? Electricity { get; set; }
        public string StudyTime { get; set; }
        public string Discipline { get; set; }
        public string StudyType { get; set; }
        public string Cabinet { get; set; }
        public string Employee { get; set; }
        public string Subgroup { get; set; } 
        public string Note { get; set; }

        public Lesson(Dictionary<string, string> lesson)
        {
            StudyTime   = lesson["study_time"];
            StudyType   = lesson["study_type"];
            Cabinet     = lesson["cabinet"];
            Subgroup    = lesson["study_subgroup"] ?? string.Empty;
            Electricity = CalculateLightInfo(lesson["study_time"], lesson["week_day"]);
            Discipline  = RenameDiscipline(lesson["discipline"]);
            Employee    = ShortenFullName(lesson["employee"]);
            ID          = CreateIdentifier(lesson["study_time"], lesson["discipline"], lesson["study_type"], lesson["full_date"]);
            Note        = CheckForNote();
        }

        private string CheckForNote()
        {
            if (Properties.Settings.Default.Notes.ContainsKey(ID))
            {
                return Properties.Settings.Default.Notes[ID];
            }
            else
            {
                return string.Empty;
            }
        }

        private uint CreateIdentifier(string studyTime, string discipline, string studyType, string date)
        {
            string encodeDate = date[0..2];
            string encodeStudyTime = studyTime[0..5].Replace(":", "");
            string encodeDiscipline = Convert.ToString(discipline.Length);
            string encodeStudyType = Convert.ToString(studyType.Length);
            return uint.Parse(encodeDate + encodeStudyTime + encodeDiscipline + encodeStudyType);
        }

        private ElectricityStatus? CalculateLightInfo(string studyTime, string weekDay)
        {
            if (!Properties.Settings.Default.ShowBlackouts)
                return null;
            string dayOfWeek = GetWeekNumber(weekDay);
            string[] liststring = FindIntersections(studyTime, dayOfWeek);
            if (liststring.Length == 0)
            {
                return null;
            }
            ElectricityStatus elecInfo = new ElectricityStatus();
            foreach (string intersectedTime in liststring)
            {
                if (UserData.GroupedLights[dayOfWeek]["no"].Contains(intersectedTime))
                {
                    elecInfo.Add(intersectedTime, LIGHT_TYPE.ABSENT);
                }
                else if (UserData.GroupedLights[dayOfWeek]["maybe"].Contains(intersectedTime))
                {
                    elecInfo.Add(intersectedTime, LIGHT_TYPE.MAYBE);
                }
            }
            elecInfo.Finish();
            return elecInfo;
        }

        private string GetWeekNumber(string weekDay)
        {
            switch (weekDay)
            {
                case "Понеділок":   return "1";
                case "Вівторок":    return "2";
                case "Середа":      return "3";
                case "Четвер":      return "4";
                case "П'ятниця":    return "5";
                case "Субота":      return "6";
                case "Неділя":      return "7";

                default:            return "1";
            }

        }

        private string[] FindIntersections(string studyTime, string dayOfWeek)
        {
            var currBlackouts = UserData.GroupedLights[dayOfWeek];
            string[] CanBeNoLight = currBlackouts["no"].Concat(currBlackouts["maybe"]).ToArray();

            string[] timeInterval = new Regex("-").Split(studyTime);
            string startHour = new Regex(":").Split(timeInterval[0])[0];
            string[] tempEndHour = new Regex(":").Split(timeInterval[1]);

            if (tempEndHour[1] == "00")
                tempEndHour[0] = Convert.ToString(int.Parse(tempEndHour[0]) - 1);
            string endHour = tempEndHour[0];

            List<string> lessonPeriod = new List<string>();
            for (int i = int.Parse(startHour); i <= int.Parse(endHour); i++)
            {
                lessonPeriod.Add(Convert.ToString(i + 1));
            }

            return CanBeNoLight.Intersect(lessonPeriod).ToArray();
        }
        private string RenameDiscipline(string discipline)
        {
            if (Properties.Settings.Default.Renames.ContainsKey(discipline))
            {
                return Properties.Settings.Default.Renames[discipline];
            }
            else
            {
                return discipline;
            }
        }
        private string ShortenFullName(string employee)
        {
            if (employee == string.Empty)
                return employee;

            string[] EmplSplitted = employee.Split();
            // if the string is not of format "Surname Name Patronymic"
            if (EmplSplitted.Length != 3)
            {
                return employee;
            }
            else
            {
                return $"{EmplSplitted[0]} {EmplSplitted[1][0]}.{EmplSplitted[2][0]}.";
            }
        }
    }
}
