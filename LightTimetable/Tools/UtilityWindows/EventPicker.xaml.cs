using Microsoft.Graph.Models;

using System;
using System.Windows;
using System.Collections.Generic;

using LightTimetable.Models.Services;

namespace LightTimetable.Tools.UtilityWindows
{
    public partial class EventPicker 
    {
        private bool _isOpenButtonClicked;
        private EventPicker(string title, List<Event> events)
        {
            InitializeComponent();
            Title = title;
            foreach (var teamsEvent in events)
            {
                var teamsTimeStart = teamsEvent.Start.ToDateTime().AddHours(TeamsEventsService.UtcOffset).ToShortTimeString();
                var teamsTimeEnd = teamsEvent.End.ToDateTime().AddHours(TeamsEventsService.UtcOffset).ToShortTimeString();
                teamsEvent.BodyPreview = teamsTimeStart + "-" + teamsTimeEnd;
                EventPickerGrid.Items.Add(teamsEvent);
            }
        }

        public static Event? Show(string title, List<Event> events)
        {
            var inputWindow = new EventPicker(title, events);
            inputWindow.ShowDialog();

            var selectedEvent = inputWindow.EventPickerGrid.SelectedItem as Event;
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
