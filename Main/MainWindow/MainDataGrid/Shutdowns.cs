using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Timetable.Utilities;

namespace Timetable.Main
{
    public partial class MainWindow
    {
        private ElectricityStatus? CalculateLightInfo(string studyTime)
        {
            string dayOfWeek = (UserData.Date.DayOfWeek == 0) ? "7" : Convert.ToString((int)UserData.Date.DayOfWeek);
            var liststring = FindIntersections(studyTime, dayOfWeek);
            if (liststring.Count == 0)
            {
                return null;
            }
            ElectricityStatus elecInfo = new ElectricityStatus();
            foreach (var intersectedTime in liststring)
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

        private List<string> FindIntersections(string studyTime, string dayOfWeek)
        {
            Dictionary<string, List<string>> CanBeNoLight = new Dictionary<string, List<string>>();
            foreach (var group1 in UserData.GroupedLights)
            {
                CanBeNoLight.Add(group1.Key, group1.Value["no"].Concat(group1.Value["maybe"]).ToList());
            }

            string[] timeInterval = new Regex("-").Split(studyTime);
            Regex doubleDot = new Regex(":");
            string startHour = doubleDot.Split(timeInterval[0])[0];
            string[] tempEndHour = doubleDot.Split(timeInterval[1]);
            if (tempEndHour[1] == "00")
                tempEndHour[0] = Convert.ToString(int.Parse(tempEndHour[0]) - 1);
            string endHour = tempEndHour[0];
            List<string> lessonPeriod = new List<string>();
            for (int i = int.Parse(startHour); i <= int.Parse(endHour); i++)
            {
                lessonPeriod.Add(Convert.ToString(i + 1));
            }
            return CanBeNoLight[dayOfWeek].Intersect(lessonPeriod).ToList();
        }
    }


}
