using LightTimetable.Properties;


namespace LightTimetable.Models
{
    public readonly struct DisciplineName
    {
        private readonly IUserSettings _settings;
        public string Original { get; }
        public string Modified => _settings.Renames.TryGetValue(Original, out var modified) ? modified : Original; 

        public DisciplineName(string original, IUserSettings settingsSource)
        {
            _settings = settingsSource;
            Original = original;
        }
        
        public override string ToString() => Modified;
    }
}
