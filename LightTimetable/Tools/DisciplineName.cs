using LightTimetable.Properties;


namespace LightTimetable.Tools
{
    public readonly struct DisciplineName
    {
        public string Original { get; }
        public string Modified => Settings.Default.Renames.TryGetValue(Original, out var modified) ? modified : Original; 

        public DisciplineName(string original)
        {
            Original = original;
        }
        
        public override string ToString() => Modified;
    }
}
