using LightTimetable.Properties;


namespace LightTimetable.Tools
{
    public struct DisciplineName
    {
        public string Original { get; }
        public string Modified => Settings.Default.Renames.TryGetValue(Original, out var modified) ? modified : Original; 

        public DisciplineName(string originalName)
        {
            Original = originalName;
        }

        public override string ToString() => Modified;
    }
}
