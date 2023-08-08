using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Windows;
using System.Security;

using Microsoft.Win32;
using LightTimetable.Models;
using LightTimetable.Properties;


namespace LightTimetable.ViewModels.Pages
{
    public partial class ApplicationPageViewModel : PageViewModelBase
    {
        private readonly UpdatesMediator _mediator;
        private readonly IUserSettings _settings;
        public ApplicationPageViewModel(IUserSettings settings, UpdatesMediator mediator)
        {
            _settings = settings;
            _mediator = mediator;

            _startAutomatically = _settings.Autostart;
            _openWindowMode = _settings.OpenWindowMode;
            _middleMouseClick = _settings.MiddleMouseClick;
            _windowPosition = _settings.WindowPosition;
        }

        #region Properties

        [ObservableProperty]
        private int _startAutomatically;
        
        [ObservableProperty]
        private int _openWindowMode;
        
        [ObservableProperty]
        private int _middleMouseClick;
        
        [ObservableProperty]
        private int _windowPosition;

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
            bool isSettingsChanged = _settings.WindowPosition != WindowPosition;

            if (_settings.Autostart != StartAutomatically)
            {
                switch (StartAutomatically)
                {
                    case 0: RemoveFromAutostart(); break;
                    case 1: AddAppToAutostart(); break;
                }
            }

            _settings.OpenWindowMode = OpenWindowMode;
            _settings.MiddleMouseClick = MiddleMouseClick;
            _settings.WindowPosition = WindowPosition;
            _settings.Autostart = StartAutomatically;
            _settings.Save();
            
            if (isSettingsChanged)
            {
                _mediator.RepositionRequired();
            }
        }

        #endregion
    }
}
