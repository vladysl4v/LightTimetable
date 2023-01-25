using System.Windows;

namespace Timetable
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await UserData.Initialize();
            var trayMenu = new TrayMenu();
        }
    }
}
