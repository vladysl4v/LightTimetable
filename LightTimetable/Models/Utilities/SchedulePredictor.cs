using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.Common;

using PredictedSchedule =
    System.Collections.Generic.Dictionary<bool, System.Collections.Generic.Dictionary<int,
        System.Collections.Generic.List<LightTimetable.Common.DataItem>>>;


namespace LightTimetable.Models.Utilities
{
    public class SchedulePredictor
    {
        private readonly PredictedSchedule? _predictedSchedule;
        private readonly GregorianCalendar _calendar;

        public SchedulePredictor(Dictionary<DateTime, List<DataItem>> fullSchedule)
        {
            _calendar = new GregorianCalendar();
            _predictedSchedule = CreateSchedule(fullSchedule);
        }

        public List<DataItem> GetRiggedSchedule(DateTime date)
        {
            if (_predictedSchedule == null)
                return new List<DataItem>();

            var currentSchedule = _predictedSchedule[IsWeekPrimary(date)];

            if (!currentSchedule.Any() ||
                !currentSchedule.TryGetValue((int)date.GetNormalDayOfWeek(), out List<DataItem> suitableList))
                return new List<DataItem>();

            var result = new List<DataItem>();
            var builder = new DataItemBuilder();
            
            foreach (var dataItem in suitableList)
            {
                builder.AddTimePeriod(date, dataItem.StudyTime);
                builder.AddBasicInformation(dataItem.Discipline.Original,
                    dataItem.StudyType, dataItem.Cabinet, dataItem.Employee);
                builder.AddPromt(dataItem.Promt);

                result.Add(builder.Build());
            }
            
            return result;
        }

        private PredictedSchedule? CreateSchedule(Dictionary<DateTime, List<DataItem>> schedule)
        {
            if (!schedule.Any())
                return null;

            var isWeekPrimary = true;
            var previousDayOfWeek = NormalDayOfWeek.Sunday;

            var primaryWeek = new Dictionary<int, List<DataItem>>();
            var secondaryWeek = new Dictionary<int, List<DataItem>>();

            foreach (var dayItems in schedule.Reverse())
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

            var lastWeekPrimariness = IsWeekPrimary(schedule.Last().Key);

            return new PredictedSchedule
            {
                { !lastWeekPrimariness, secondaryWeek },
                { lastWeekPrimariness, primaryWeek }
            };
        }
        private bool IsWeekPrimary(DateTime date)
        {
            var intPrimariness = _calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday) % 2;
            return intPrimariness == 1;
        }
    }
}
