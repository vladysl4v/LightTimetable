using System;
using System.Net.Http;

using LightTimetable.Properties;
using LightTimetable.ScheduleSources.Abstractions;


namespace LightTimetable.ScheduleSources.KrokSource
{
    public class UniversityKrokFactory : IScheduleFactory
    {
        private readonly Func<DataItemBuilder> _builderCreator;
        private readonly IHttpClientFactory _httpFactory;
        private readonly IUserSettings _settings;
        public string Name => "Унiверситет \"КРОК\"";

        public UniversityKrokFactory(IHttpClientFactory httpClientFactory, IUserSettings settings, Func<DataItemBuilder> builderCreator)
        {
            _builderCreator = builderCreator;
            _httpFactory = httpClientFactory;
            _settings = settings;
        }

        public IScheduleSettings CreateSettingsSource()
        {
            return new UniversityKrokSettings(_httpFactory);
        }

        public IScheduleSource CreateScheduleSource()
        {
            return new UniversityKrokSource(_settings, _httpFactory, _builderCreator.Invoke());
        }
    }
}
