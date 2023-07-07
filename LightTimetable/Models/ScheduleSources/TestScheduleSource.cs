using LightTimetable.Tools;
using LightTimetable.Models.Services;


namespace LightTimetable.Models.ScheduleSources
{
    [ScheduleSource("Тестове джерело", typeof(TestScheduleSettings))]
    public sealed class TestSource : VnzOsvitaSource
    {
        public TestSource(ElectricityService? electricity, TeamsEventsService teams)
            : base(electricity, teams) => UniversityId = 99999;
    }
}