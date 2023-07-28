using Hardcodet.Wpf.TaskbarNotification;

using System.Linq;
using System.Windows;

using LightTimetable.Views;


namespace LightTimetable
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;

        protected override void OnStartup(StartupEventArgs startupArgs)
        {
            base.OnStartup(startupArgs);

            // Create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon")!;

            if (startupArgs.Args.Contains("-initial"))
            {
                var initSetupWindow = new InitialSetupView();
                initSetupWindow.ShowDialog();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // The icon would clean up automatically, but this is cleaner
            _notifyIcon.Dispose(); 
            base.OnExit(e);
        }
    }
}
