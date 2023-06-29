using System;
using System.Linq;


namespace LightTimetable.Models
{
    public class DateControl
    {
        private readonly DateTime[] _scheduleDates;
        private readonly Action? OnDateChanged;
        private DateTime _date;

        public DateControl()
        {
            _scheduleDates = Array.Empty<DateTime>();
            _date = DateTime.MinValue;
        }

        public DateControl(DateTime[] dates, Action? onDateChanged)
        {
            _scheduleDates = dates;
            OnDateChanged = onDateChanged;
            SetCorrectDate();
        }

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
            if (!_scheduleDates.Any())
            {
                Date = DateTime.MinValue;
                return;
            }
            var currentDate = DateTime.Today;
            if (DateTime.Now.Hour > 18)
                currentDate = currentDate.AddDays(1);


            var nextDates = (from date in _scheduleDates where date >= currentDate select date).ToArray();

            Date = nextDates.Any() ? nextDates.First() : _scheduleDates.Last();
        }

        public void ChangeDate(int amount)
        {
            // If we keep going through the empty content list
            if (!_scheduleDates.Contains(Date))
            {
                if (!(Date == DateTime.MinValue && amount == -1))
                {
                    Date = Date.AddDays(amount);
                }
                return;
            }
            var nextIndex = Array.IndexOf(_scheduleDates, Date) + amount;
            // If we hit the borders, just keep moving into the void
            if (nextIndex >= _scheduleDates.Length || nextIndex < 0)
                Date = Date.AddDays(amount);
            else // Best result, we got the next date
                Date = _scheduleDates[nextIndex];
        }
    }
}
