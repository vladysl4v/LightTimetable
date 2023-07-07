using LightTimetable.Tools;
using LightTimetable.Models.Services;


namespace LightTimetable.Models.ScheduleSources
{
    [ScheduleSource("Унiверситет \"КРОК\"", typeof(UniversityKrokSettings))]
    public sealed class UniversityKrokSource : VnzOsvitaSource
    {
        public UniversityKrokSource(ElectricityService? electricity, TeamsEventsService teams)
            : base(electricity, teams) => UniversityId = 11784;
    }
}