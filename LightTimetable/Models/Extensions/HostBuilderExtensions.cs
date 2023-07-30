using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using System;

using LightTimetable.Views;
using LightTimetable.Handlers;
using LightTimetable.ViewModels;
using LightTimetable.Properties;
using LightTimetable.ScheduleSources;
using LightTimetable.ViewModels.Pages;
using LightTimetable.Handlers.Abstractions;
using LightTimetable.ScheduleSources.Abstractions;
using LightTimetable.ScheduleSources.KpiSource;
using LightTimetable.ScheduleSources.KrokSource;


namespace LightTimetable.Models.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddViewModels(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddTransient<NotifyIconViewModel>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<TimetableViewModel>();
            });

            return host;
        }

        public static IHostBuilder AddSettingsPages(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddTransient<ApplicationPageViewModel>();
                services.AddTransient<IntegrationsPageViewModel>();
                services.AddTransient<RenamingPageViewModel>();
                services.AddTransient<SchedulePageViewModel>();

                services.AddSingleton<CreateViewModel<ApplicationPageViewModel>>(s =>
                    s.GetRequiredService<ApplicationPageViewModel>);

                services.AddSingleton<CreateViewModel<IntegrationsPageViewModel>>(s =>
                    s.GetRequiredService<IntegrationsPageViewModel>);

                services.AddSingleton<CreateViewModel<RenamingPageViewModel>>(s =>
                    s.GetRequiredService<RenamingPageViewModel>);

                services.AddSingleton<CreateViewModel<SchedulePageViewModel>>(s =>
                    s.GetRequiredService<SchedulePageViewModel>);

            });

            return host;
        }

        public static IHostBuilder AddViews(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddSingleton(s => new TimetableView(
                        s.GetRequiredService<TimetableViewModel>(),
                        s.GetRequiredService<UpdatesMediator>(),
                        s.GetRequiredService<IUserSettings>()));

                services.AddSingleton(s =>
                    new InitialSetupView(s.GetRequiredService<SchedulePageViewModel>()));

                services.AddTransient(s =>
                    new SettingsView(s.GetRequiredService<SettingsViewModel>()));
            });

            return host;
        }

        public static IHostBuilder AddScheduleSources(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddTransient<IScheduleFactory, KpiScheduleFactory>();
                services.AddTransient<IScheduleFactory, UniversityKrokFactory>();
            });

            return host;
        }

        public static IHostBuilder AddServices(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddHttpClient();
                services.AddSingleton<IUserSettings, Settings>(_ => Settings.Default);
                services.AddTransient<DataItemBuilder>();
                services.AddSingleton<Func<DataItemBuilder>>(s => s.GetRequiredService<DataItemBuilder>);
                services.AddSingleton<SchedulesRepository>();
                services.AddSingleton<UpdatesMediator>();
                services.AddTransient<IDataManager, DataManager>();
            });

            return host;
        }
    }
}
