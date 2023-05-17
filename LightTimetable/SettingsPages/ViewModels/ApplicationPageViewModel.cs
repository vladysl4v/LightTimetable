using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Windows;
using System.Security;
using Microsoft.Win32;

using LightTimetable.Views;
using LightTimetable.Properties;


namespace LightTimetable.SettingsPages.ViewModels
{
    public partial class ApplicationPageViewModel : PageViewModelBase
    {
        #region Properties

        [ObservableProperty]
        private int _startAutomatically = Settings.Default.Autostart;
        
        [ObservableProperty]
        private int _openWindowMode = Settings.Default.OpenWindowMode;
        
        [ObservableProperty]
        private int _middleMouseClick = Settings.Default.MiddleMouseClick;
        
        [ObservableProperty]
        private int _windowPosition = Settings.Default.WindowPosition;

        #endregion

        #region Methods

        private void AddAppToAutostart()
        {
            try
            {
                var registry = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                registry?.SetValue("LightTimetable", AppDomain.CurrentDomain.BaseDirectory + "LightTimetable.exe");
            }
            catch (Exception e)
            {
                if (e is UnauthorizedAccessException or SecurityException)
                {
                    MessageBox.Show("Виникла помилка: Користувач не має дозволів, необхідних для зміни розділів реєстру.",
                        "LightTimetable", MessageBoxButton.OK, MessageBoxImage.Error);
                    StartAutomatically = 0;
                }
            }
        }

        private void RemoveFromAutostart()
        {
            try
            {
                var registry = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                registry?.DeleteValue("LightTimetable", false);
            }
            catch (Exception e)
            {
                if (e is UnauthorizedAccessException or SecurityException)
                {
                    MessageBox.Show("Виникла помилка: Користувач не має дозволів, необхідних для зміни розділів реєстру.",
                        "LightTimetable", MessageBoxButton.OK, MessageBoxImage.Error);
                    StartAutomatically = 1;
                }
            }
        }

        public override void Save()
        {
            if (Settings.Default.WindowPosition != WindowPosition)
            {
                SettingsView.IsRequiredResize = true;
            }

            if (Settings.Default.Autostart != StartAutomatically)
            {
                switch (StartAutomatically)
                {
                    case 0: RemoveFromAutostart(); break;
                    case 1: AddAppToAutostart(); break;
                }
            }

            Settings.Default.OpenWindowMode = OpenWindowMode;
            Settings.Default.MiddleMouseClick = MiddleMouseClick;
            Settings.Default.WindowPosition = WindowPosition;
            Settings.Default.Autostart = StartAutomatically;
            Settings.Default.Save();
            
            IsAnythingChanged = false;
        }

        #endregion
    }
}
