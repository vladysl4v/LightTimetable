using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Models.Utilities;

using RiggedScheduleDictionary =
    System.Collections.Generic.Dictionary<bool, System.Collections.Generic.Dictionary<int,
        System.Collections.Generic.List<LightTimetable.Models.Utilities.DataItem>>>;


namespace LightTimetable.Models.Services
{
    public class RiggedSchedule
    {
        private readonly RiggedScheduleDictionary _riggedSchedule;
        private readonly GregorianCalendar _calendar = new();

        public RiggedSchedule(Dictionary<DateTime, List<DataItem>> scheduleDictionary)
        {
            _riggedSchedule = InitializeRiggedSchedule(scheduleDictionary);
        }

        public List<DataItem> GetRiggedSchedule(DateTime date)
        {

            if (!_riggedSchedule.Any())
                return new List<DataItem>();

            var currentRiggedSchedule = _riggedSchedule[IsWeekPrimary(date)];

            if (!currentRiggedSchedule.Any() ||
                !currentRiggedSchedule.TryGetValue((int)date.GetNormalDayOfWeek(), out List<DataItem> suitableList))
                return new List<DataItem>();

            var result = new List<DataItem>();

            suitableList.ForEach(x => result.Add(new DataItem(x, date)));

            return result;
        }

        private bool IsWeekPrimary(DateTime date)
        {
            var intPrimariness = _calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday) % 2;
            return intPrimariness == 1;
        }

        private RiggedScheduleDictionary InitializeRiggedSchedule(Dictionary<DateTime, List<DataItem>> scheduleDictionary)
        {
            if (!scheduleDictionary.Any())
                return new RiggedScheduleDictionary();

            var isWeekPrimary = true;
            var previousDayOfWeek = NormalDayOfWeek.Sunday;

            var primaryWeek = new Dictionary<int, List<DataItem>>();
            var secondaryWeek = new Dictionary<int, List<DataItem>>();

            foreach (var dayItems in scheduleDictionary.Reverse())
            {
                if (dayItems.Key.GetNormalDayOfWeek() > previousDayOfWeek)
                {
                    if (!isWeekPrimary)
                        break;
                    isWeekPrimary = false;
                }

                if (isWeekPrimary)
                    primaryWeek.Add((int)dayItems.Key.GetNormalDayOfWeek(), dayItems.Value);
                else
                    secondaryWeek.Add((int)dayItems.Key.GetNormalDayOfWeek(), dayItems.Value);

                previousDayOfWeek = dayItems.Key.GetNormalDayOfWeek();
            }

            var lastWeekPrimariness = IsWeekPrimary(scheduleDictionary.Last().Key);

            return new RiggedScheduleDictionary
            {
                { !lastWeekPrimariness, secondaryWeek },
                { lastWeekPrimariness, primaryWeek }
            };
        }
    }
}
