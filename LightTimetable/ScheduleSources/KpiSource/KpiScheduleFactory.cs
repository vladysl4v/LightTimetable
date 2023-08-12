using System;
using System.Net.Http;

using LightTimetable.Properties;
using LightTimetable.ScheduleSources.Abstractions;


namespace LightTimetable.ScheduleSources.KpiSource
{
    public class KpiScheduleFactory : IScheduleFactory
    {
        private readonly Func<DataItemBuilder> _builderCreator;
        private readonly IHttpClientFactory _httpFactory;
        private readonly IUserSettings _settings;
        public string Name => "КПI iм. Iгоря Сiкорського";

        public KpiScheduleFactory(IHttpClientFactory httpClientFactory, IUserSettings settings, Func<DataItemBuilder> builderCreator)
        {
            _builderCreator = builderCreator;
            _httpFactory = httpClientFactory;
            _settings = settings;
        }

        public IScheduleSettings CreateSettingsSource()
        {
            return new KpiScheduleSettings(_httpFactory);
        }

        public IScheduleSource CreateScheduleSource()
        {
            return new KpiScheduleSource(_settings, _httpFactory, _builderCreator.Invoke());
        }
    }
}
