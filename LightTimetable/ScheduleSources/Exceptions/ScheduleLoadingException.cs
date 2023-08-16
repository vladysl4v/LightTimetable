using System;


namespace LightTimetable.ScheduleSources.Exceptions
{
    public class ScheduleLoadingException : Exception
    {
        private readonly string _studyGroup;
        public ScheduleLoadingException(string studyGroup, string message = "Error occurred during schedule loading.")
            : base(message)
        {
            _studyGroup = studyGroup;
        }

        public ScheduleLoadingException(Exception innerException, string studyGroup,
            string message = "Error occurred during schedule loading.") : base(message, innerException)
        {
            _studyGroup = studyGroup;
        }

        public override string ToString()
        {
            return base.ToString() + $" StudentGroupId = {_studyGroup}";
        }
    }
}
