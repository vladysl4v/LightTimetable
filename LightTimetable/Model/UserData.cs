using System;
using System.Linq;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Tools.Data;


namespace LightTimetable.Model
{
    public class UserData
    {
        private readonly DataProvider _dataProvider;
        public Dictionary<DateTime, List<DataItem>> ScheduleData { get; private set; } = null!;
        public DateTime Date { get; set; }

        public UserData()
        {
            _dataProvider = new DataProvider();
            UpdateData();
        }

        public void UpdateData()
        {
            ScheduleData = _dataProvider.GetData();
            Date = GetCorrectDate();
        }

        public void UpdateRenames(MutablePair<string, string> renamePair)
        {
            foreach (var item in from dateItems in ScheduleData.Values from item in dateItems where item.Discipline.Value == renamePair.Value select item)
            {
                item.Discipline.Key = renamePair.Key;
            }
        }

        public void ResetDate()
        {
            Date = GetCorrectDate();
        }
        public void ChangeDate(int amount)
        {
            List<DateTime> dataDays = ScheduleData.Keys.ToList();
            // If we keep going through the empty content list
            if (!dataDays.Contains(Date))
            {
                Date = Date.AddDays(amount);
                return;
            }
            int nextIndex = (dataDays.IndexOf(Date)) + amount;
            // If we hit the borders, just keep moving into the void
            if (nextIndex >= dataDays.Count || nextIndex < 0)
                Date = Date.AddDays(amount);
            else // Best result, we got the next date
                Date = dataDays[nextIndex];
        }

        private DateTime GetCorrectDate()
        {
            if (!ScheduleData.Any()) 
            {
                return DateTime.Today;
            }
            var currentDate = DateTime.Today;
            if (DateTime.Now.Hour > 18)
                currentDate = currentDate.AddDays(1);

            var dataDays = ScheduleData.Keys;

            var nextDates = (from date in dataDays where date >= currentDate select date).ToArray();

            return nextDates.Any() ? nextDates.First() : dataDays.Last();
        }

    }
}