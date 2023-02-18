using System;
using System.Linq;
using Timetable.Utilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Timetable.Main
{
    public class Lesson
    {
        public ElectricityStatus? Electricity { get; init; }
        public string StudyTime { get; init; } = string.Empty;
        public string Discipline { get; init; } = string.Empty;
        public string StudyType { get; init; } = string.Empty;
        public string Cabinet { get; init; } = string.Empty;
        public string Employee { get; init; } = string.Empty;

        public Lesson(Dictionary<string, string> lesson)
        {
            StudyTime = lesson["study_time"];
            StudyType = lesson["study_type"];
            Cabinet = lesson["cabinet"];
            Electricity = CalculateLightInfo(lesson["study_time"]);
            Discipline = RenameDiscipline(lesson["discipline"]);
            Employee = ShortenFullName(lesson["employee"]);
        }
        private ElectricityStatus? CalculateLightInfo(string studyTime)
        {
            if (!Properties.Settings.Default.ShowBlackouts)
                return null;
            string dayOfWeek = (UserData.Date.DayOfWeek == 0) ? "7" : Convert.ToString((int)UserData.Date.DayOfWeek);
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
