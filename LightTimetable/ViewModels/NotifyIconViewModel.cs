﻿using System.Windows;

using LightTimetable.Models;
using LightTimetable.Tools;
using LightTimetable.Models.Electricity;
using LightTimetable.Views;

using static LightTimetable.Properties.Settings;

namespace LightTimetable.ViewModels
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        private SettingsView? _settingsWindow;

        public NotifyIconViewModel()
        {
            // Commands
            SingleClickCommand = new RelayCommand(_ => SingleClick());
            DoubleClickCommand = new RelayCommand(_ => DoubleClick());
            MiddleClickCommand = new RelayCommand(_ => MiddleClick());

            ShowTimetableCommand = new RelayCommand(_ => ShowTimetable());
            RefreshDataCommand = new RelayCommand(_ => RefreshData());
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
            CloseApplicationCommand = new RelayCommand(_ => CloseApplication());

            InitializeNotifyIcon();
        }

        #region Commands
        public RelayCommand DoubleClickCommand { get; }
        public RelayCommand SingleClickCommand { get; }
        public RelayCommand MiddleClickCommand { get; }
        public RelayCommand ShowTimetableCommand { get; }
        public RelayCommand RefreshDataCommand { get; }
        public RelayCommand OpenSettingsCommand { get; }
        public RelayCommand CloseApplicationCommand { get; }

        private void SingleClick()
        {
            if (Default.OpenWindowMode == 0)
                ShowTimetable();
        }

        private void DoubleClick()
        {
            if (Default.OpenWindowMode == 1)
                ShowTimetable();
        }

        private void MiddleClick()
        {
            switch (Default.MiddleMouseClick)
            {
                case 1: RefreshData(); break;
                case 2: OpenSettings(); break;
            }
        }

        private void ShowTimetable()
        {
            Application.Current.MainWindow.Show();
        }

        private async void RefreshData()
        {
            var mainWindow = Application.Current.MainWindow as TimetableView;
            await DataProvider.InitializeDataAsync();
            mainWindow.ReloadData();
            mainWindow.RefreshTimetable();
        }

        private void OpenSettings()
        {
            if (_settingsWindow != null) 
                return;
            _settingsWindow = new SettingsView();
            _settingsWindow.Closed += (_, _) =>
            {
                _settingsWindow = null;
                RefreshData();
            };
            _settingsWindow.Show();
        }

        private void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Methods

        private void InitializeNotifyIcon()
        {
            Application.Current.MainWindow = new TimetableView();
            DataProvider.InitializeDataAsync();
            ElectricityProvider.InitializeBlackoutsAsync();
        }

        #endregion
    }
}
