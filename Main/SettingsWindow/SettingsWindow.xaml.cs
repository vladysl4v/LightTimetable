using System;
using System.Windows;
using System.Windows.Controls;

using Timetable.Settings.Pages;

namespace Timetable.Settings
{
    /// <summary>
    /// Central logic for SettingsWindow
    /// </summary>

    public partial class SettingsWindow : Window
    {
        private readonly Action<object?, EventArgs?> RefreshTimetable;
        object CurrentPage;
        public SettingsWindow(Action<object?, EventArgs?> refreshTimetable)
        {
            RefreshTimetable = refreshTimetable;
            InitializeComponent();
            Timetable_Selected(this, null);
        }

        private void Timetable_Selected(object sender, RoutedEventArgs? e)
        {
            SetHeaders("Розклад", "Виберіть свою навчальну групу");
            CurrentPage = new TimetablePage();
            MainFrame.Content = CurrentPage;
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Button button_sender = (Button)sender;
            if (CurrentPage is TimetablePage thisPage)
            {
                Properties.Settings.Default.Course = Convert.ToString(thisPage.comboCourses.SelectedValue);
                Properties.Settings.Default.EducForm = Convert.ToString(thisPage.comboEducForms.SelectedValue);
                Properties.Settings.Default.Faculty = Convert.ToString(thisPage.comboFaculties.SelectedValue);
                Properties.Settings.Default.StudyGroup = Convert.ToString(thisPage.comboStudyGroups.SelectedValue);
                Properties.Settings.Default.Save();
            }

            if ((string)button_sender.Content == "ОК")
                this.Close();

            RefreshTimetable(this, null);
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
