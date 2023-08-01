using System;
using System.Linq;

using LightTimetable.Models.Extensions;
using LightTimetable.Handlers.Abstractions;


namespace LightTimetable.Handlers
{
    public class DateTimeHandler : IDateTimeHandler
    {
        public event Action? OnDateChanged;
        private DateTime _date;

        public DateTimeHandler()
        {
            var startOfTheWeek = DateTime.Today.AddDays(-(int)DateTime.Today.GetNormalDayOfWeek());

            var startDate = startOfTheWeek.AddDays(-14);
            var endDate = startOfTheWeek.AddDays(+13);

            startDate = new DateTime(2023, 01, 23);
            endDate = new DateTime(2023, 02, 10);

            AvailableDates = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                .Select(offset => startDate.AddDays(offset))
                .ToArray();

            SetCorrectDate();
        }

        public DateTime[] AvailableDates { get; set; }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnDateChanged?.Invoke();
            }
        }

        public void SetCorrectDate()
        {
            if (!AvailableDates.Any())
            {
                Date = DateTime.MinValue;
                return;
            }
            var currentDate = DateTime.Today;
            if (DateTime.Now.Hour > 18)
                currentDate = currentDate.AddDays(1);

            var nextDates = (from date in AvailableDates where date >= currentDate select date).ToArray();

            Date = nextDates.Any() ? nextDates.First() : AvailableDates.Last();
        }

        public void ChangeDate(int amount)
        {
            // If we keep going through the empty content list
            if (!AvailableDates.Contains(Date))
            {
                if (!(Date == DateTime.MinValue && amount == -1))
                {
                    Date = Date.AddDays(amount);
                }
                return;
            }
            var nextIndex = Array.IndexOf(AvailableDates, Date) + amount;
            // If we hit the borders, just keep moving into the void
            if (nextIndex >= AvailableDates.Length || nextIndex < 0)
                Date = Date.AddDays(amount);
            else // Best result, we got the next date
                Date = AvailableDates[nextIndex];
        }
    }
}
