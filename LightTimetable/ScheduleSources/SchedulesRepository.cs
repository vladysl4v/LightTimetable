using System.Linq;
using System.Collections.Generic;

using LightTimetable.Properties;
using LightTimetable.ScheduleSources.Abstractions;


namespace LightTimetable.ScheduleSources
{
    public class SchedulesRepository
    {
        private readonly Dictionary<string, IScheduleFactory> _scheduleFactories;
        private readonly IUserSettings _settings;
        
        public SchedulesRepository(IEnumerable<IScheduleFactory> scheduleFactories, IUserSettings settings)
        {
            _settings = settings;
            _scheduleFactories = scheduleFactories.ToDictionary(key => key.Name, value => value);
        }

        public List<string> GetScheduleNames()
        {
            return _scheduleFactories.Keys.ToList();
        }

        public IScheduleSource? GetScheduleSource(string name = null)
        {
            name ??= _settings.ScheduleSource;
            return FindScheduleSourcePair(name)?.CreateScheduleSource();
        }

        public IScheduleSettings? GetScheduleSettings(string name = null)
        {
            name ??= _settings.ScheduleSource;
            return FindScheduleSourcePair(name)?.CreateSettingsSource();
        }

        private IScheduleFactory? FindScheduleSourcePair(string name)
        {
            return _scheduleFactories.TryGetValue(name, out var pair) ? pair : null;
        }


    }
}
