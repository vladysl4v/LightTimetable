using System;

using CommunityToolkit.Mvvm.Input;
using LightTimetable.Models;
using LightTimetable.Properties;


namespace LightTimetable.ViewModels
{
    public partial class NotifyIconViewModel : IDisposable
    {
        public Action? CreateSettingsWindow { get; set; }
        public Action? ShowTimetableWindow { get; set; }
        public Action? CloseTimetableWindow { get; set; }
        public Action? RefreshTimetableWindow { get; set; }

        private readonly UpdatesMediator _mediator;
        private readonly IUserSettings _settings;

        public NotifyIconViewModel(IUserSettings settings, UpdatesMediator mediator)
        {
            _settings = settings;
            _mediator = mediator;

            _mediator.OnReloadRequired += RefreshData;
        }

        public void Dispose()
        {
            _mediator.OnReloadRequired -= RefreshData;
        }

        [RelayCommand]
        private void SingleClick()
        {
            if (_settings.OpenWindowMode == 0)
            {
                ShowTimetable();
            }
        }

        [RelayCommand]
        private void DoubleClick()
        {
            if (_settings.OpenWindowMode == 1)
            {
                ShowTimetable();
            }
        }

        [RelayCommand]
        private void MiddleClick()
        {
            switch (_settings.MiddleMouseClick)
            {
                case 1: RefreshData(); break;
                case 2: OpenSettings(); break;
            }
        }

        [RelayCommand]
        private void ShowTimetable()
        {
            ShowTimetableWindow?.Invoke();
        }

        [RelayCommand]
        private void RefreshData()
        {
            RefreshTimetableWindow?.Invoke();
        }

        [RelayCommand]
        private void OpenSettings()
        {
            CreateSettingsWindow?.Invoke();
        }

        [RelayCommand]
        private void CloseApplication()
        {
            CloseTimetableWindow?.Invoke();
        }
    }
}
