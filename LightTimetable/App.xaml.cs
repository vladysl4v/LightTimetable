using Microsoft.Extensions.Hosting;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;

using System.Linq;
using System.Windows;

using LightTimetable.Views;
using LightTimetable.Properties;
using LightTimetable.ViewModels;
using LightTimetable.Models.Extensions;

namespace LightTimetable
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon = null!;
        private readonly IHost _host;

        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .AddServices()
                .AddScheduleSources()
                .AddSettingsPages()
                .AddViewModels()
                .AddViews();
        }

        protected override void OnStartup(StartupEventArgs startupArgs)
        {
            _host.Start();

            ShowNotifyIcon();

            if (startupArgs.Args.Contains("-initial"))
            {
                ShowInitialWindow();
            }

            Application.Current.MainWindow = _host.Services.GetRequiredService<TimetableView>();

            ConfigureNotifyIconCommands();

            base.OnStartup(startupArgs);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();
            base.OnExit(e);
        }

        private void ShowNotifyIcon()
        {
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            var viewModel = _host.Services.GetRequiredService<NotifyIconViewModel>();
            _notifyIcon.DataContext = viewModel;
        }

        private void ShowInitialWindow()
        {
            var settings = _host.Services.GetRequiredService<IUserSettings>();
            settings.Upgrade();
            if (settings.StudyGroup.Any())
            {
                return;
            }

            var initSetupWindow = _host.Services.GetRequiredService<InitialSetupView>();

            initSetupWindow.ShowDialog();
        }

        private void ConfigureNotifyIconCommands()
        {
            var viewModel = _notifyIcon.DataContext as NotifyIconViewModel;

            viewModel.ShowTimetableWindow = Application.Current.MainWindow.Show;
            viewModel.CloseTimetableWindow = Application.Current.Shutdown;

            viewModel.RefreshTimetableWindow = () =>
                Application.Current.MainWindow.DataContext =
                    _host.Services.GetRequiredService<TimetableViewModel>();

            viewModel.CreateSettingsWindow = () =>
            {
                var settsWindow = _host.Services.GetRequiredService<SettingsView>();
                settsWindow.Show();
            };

        }
    }
}
