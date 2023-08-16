using System.Threading.Tasks;
using System.Collections.Generic;


namespace LightTimetable.ScheduleSources.Abstractions
{
    public interface IScheduleSettings
    {
        public Task LoadStudentFiltersAsync();
        public Dictionary<string, string>? Faculties { get; protected set; }
        public Dictionary<string, string>? Courses { get; protected set; }
        public Dictionary<string, string>? EducationTypes { get; protected set; }
        public Task<Dictionary<string, string>?> GetStudyGroupsAsync(string? faculty, string? course, string? studyType);
    }
}