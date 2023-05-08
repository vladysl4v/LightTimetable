using System;
using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using LightTimetable.Models.Utilities;


namespace LightTimetable.Tools.UtilityWindows
{
    public partial class EventPicker 
    {
        private bool _isOpenButtonClicked;
        private EventPicker(string title, List<OutlookEvent> events)
        {
            InitializeComponent();
            Title = title;
            foreach (var teamsEvent in events)
            {
                EventPickerGrid.Items.Add(teamsEvent);
            }
        }

        public static OutlookEvent? Show(string title, List<OutlookEvent> events)
        {
            var inputWindow = new EventPicker(title, events);
            inputWindow.ShowDialog();

            var selectedEvent = inputWindow.EventPickerGrid.SelectedItem as OutlookEvent;
            return selectedEvent;
        }

        private void OpenInTeams_Click(object sender, RoutedEventArgs e)
        {
            _isOpenButtonClicked = true;
            Close();
        }

        private void OnWindowClosing(object? s, EventArgs e)
        {
            if (!_isOpenButtonClicked)
                EventPickerGrid.SelectedItem = null;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
