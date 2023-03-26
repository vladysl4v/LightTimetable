using System;
using Microsoft.Win32;

using static LightTimetable.Properties.Settings;


namespace LightTimetable.SettingsPages.ViewModels
{
    public class ApplicationPageViewModel : PageViewModelBase
    {
        #region Properties

        private int _startAutomatically = Default.Autostart;
        private int _openWindowMode = Default.OpenWindowMode;
        private int _middleMouseClick = Default.MiddleMouseClick;
        public int StartAutomatically
        {
            get => _startAutomatically;
            set => SetProperty(ref _startAutomatically, value);
        }

        public int OpenWindowMode
        {
            get => _openWindowMode;
            set => SetProperty(ref _openWindowMode, value);
        }

        public int MiddleMouseClick
        {
            get => _middleMouseClick;
            set => SetProperty(ref _middleMouseClick, value);
        }

        #endregion

        #region Methods

        private void AddAppToAutostart()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue("LightTimetable", AppDomain.CurrentDomain.BaseDirectory + "LightTimetable.exe");
        }

        private void RemoveFromAutostart()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.DeleteValue("LightTimetable", false);
        }

        public override void Save()
        {
            Default.OpenWindowMode = OpenWindowMode;
            Default.MiddleMouseClick = MiddleMouseClick;

            if (Default.Autostart != StartAutomatically)
            {
                switch (StartAutomatically)
                {
                    case 0: RemoveFromAutostart(); break;
                    case 1: AddAppToAutostart(); break;
                }
            }
            Default.Autostart = StartAutomatically;
            Default.Save();
            IsAnythingChanged = false;
        }

        #endregion
    }
}
