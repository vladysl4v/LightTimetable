using System.Windows;
using System.Collections.Generic;

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
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
