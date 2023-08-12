namespace LightTimetable.ScheduleSources.Abstractions
{
    public interface IScheduleFactory
    {
        public string Name { get; }
        public IScheduleSettings CreateSettingsSource();
        public IScheduleSource CreateScheduleSource();
    }
}
