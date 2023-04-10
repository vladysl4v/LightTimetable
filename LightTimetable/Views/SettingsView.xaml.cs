using System;
using System.Windows;


namespace LightTimetable.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public static event Action? SettingsSaved;

        public static bool IsRequiredReload { get; set; }
        public static bool IsRequiredResize { get; set; }

        public static void OnSettingsSaved()
        {
            SettingsSaved?.Invoke();
            IsRequiredReload = false;
            IsRequiredResize = false;
        }
    }
}
