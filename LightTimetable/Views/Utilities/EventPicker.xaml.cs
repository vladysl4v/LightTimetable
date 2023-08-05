using System;
using System.Windows;
using System.Collections.Generic;

using LightTimetable.Services.Models;


namespace LightTimetable.Views.Utilities
{
    public partial class EventPicker 
    {
        private bool _isOpenButtonClicked;
        private EventPicker(string title, List<SpecificEvent> events)
        {
            InitializeComponent();
            Title = title;
            events.ForEach(specEvent => EventPickerGrid.Items.Add(specEvent));
        }

        public static SpecificEvent? Show(string title, List<SpecificEvent> events)
        {
            var inputWindow = new EventPicker(title, events);
            inputWindow.ShowDialog();

            var selectedEvent = inputWindow.EventPickerGrid.SelectedItem as SpecificEvent;
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
