using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using Timetable.Settings.Pages;


namespace Timetable.Settings
{
    /// <summary>
    /// Central logic for SettingsWindow
    /// </summary>

    public partial class SettingsWindow : Window
    {
        private readonly Action<object?, EventArgs?> _refreshTimetable;
        private object _currentPage;
        public SettingsWindow(Action<object?, EventArgs?> refreshTimetable)
        {
            _refreshTimetable = refreshTimetable;
            InitializeComponent();
            Timetable_Selected(this, null);
        }

        private void Timetable_Selected(object sender, RoutedEventArgs? e)
        {
            SetHeaders("Розклад", "Виберіть свою навчальну групу");
            _currentPage = new TimetablePage();
            MainFrame.Content = _currentPage;
        }
        private void Renames_Selected(object sender, RoutedEventArgs? e)
        {
            SetHeaders("Перейменування", "Список усiх перейменувань пар");
            _currentPage = new RenamePage();
            MainFrame.Content = _currentPage;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Button button_sender = (Button)sender;
            if (_currentPage is TimetablePage thisPage1)
            {
                Properties.Settings.Default.Course = Convert.ToString(thisPage1.comboCourses.SelectedValue);
                Properties.Settings.Default.EducForm = Convert.ToString(thisPage1.comboEducForms.SelectedValue);
                Properties.Settings.Default.Faculty = Convert.ToString(thisPage1.comboFaculties.SelectedValue);
                Properties.Settings.Default.StudyGroup = Convert.ToString(thisPage1.comboStudyGroups.SelectedValue);
                Properties.Settings.Default.Save();
            }

            if (_currentPage is RenamePage thisPage2)
            {
               Properties.Settings.Default.RenameList = new(thisPage2.RenameList.Select(pair => new KeyValuePair<string, string>(pair.Key, pair.Value)));
                Properties.Settings.Default.Save();
            }

            if ((string)button_sender.Content == "ОК")
                this.Close();

            _refreshTimetable(this, null);
        }
        private void SetHeaders(string h1, string h2)
        {
            lvl1Header.Text = h1.ToUpper();
            lvl2Header.Text = h2;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
