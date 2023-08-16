using System;


namespace LightTimetable.Handlers.Abstractions
{
    public interface IDateTimeHandler
    {
        public DateTime[] AvailableDates { get; set; }
        public DateTime Date { get; set; }

        public event Action OnDateChanged;

        public void ChangeDate(int amount);
        public void SetCorrectDate();

    }
}
