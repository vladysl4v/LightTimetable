using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using LightTimetable.Models.Utilities;


namespace LightTimetable.Tools.UtilityWindows
{
    public partial class EventPicker 
    {
        public EventPicker(string title, List<OutlookEvent> events)
        {
            InitializeComponent();
            Title = title;
            foreach (var teamsEvent in events)
            {
                EventPickerGrid.Items.Add(teamsEvent);
            }
        }

        private void OpenInTeams_Click(object sender, RoutedEventArgs e)
        {
            var url = ((OutlookEvent)EventPickerGrid.SelectedItem).Url;
            OpenLinkInBrowser(url);

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenLinkInBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }

            MessageBox.Show("LightTimetable", "ваша операційна система не підтримується", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
