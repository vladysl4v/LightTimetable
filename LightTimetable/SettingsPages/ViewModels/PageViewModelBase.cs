using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Runtime.CompilerServices;

using LightTimetable.Tools;


namespace LightTimetable.SettingsPages.ViewModels
{
    public abstract class PageViewModelBase : INotifyPropertyChanged
    {
        protected PageViewModelBase()
        {
            SaveSettingsCommand = new RelayCommand(_ => SaveSettings());
            SaveAndCloseCommand = new RelayCommand(SaveAndCloseSettings);
            CloseSettingsCommand = new RelayCommand(CloseSettings);
        }

        public abstract void Save();

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            IsAnythingChanged = true;
            return true;
        }

        #region Properties

        protected bool _isAnythingChanged;
        public bool IsAnythingChanged
        {
            get => _isAnythingChanged;
            protected set
            {
                _isAnythingChanged = value;
                OnPropertyChanged(nameof(IsAnythingChanged));
            }
        }

        #endregion

        #region Button commands

        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand SaveAndCloseCommand { get; }
        public RelayCommand CloseSettingsCommand { get; }

        private void SaveSettings()
        {
            Save();
        }

        private void SaveAndCloseSettings(object control)
        {
            if (control is not UserControl thisPage) return;
            Save();
            Window.GetWindow(thisPage).Close();
        }

        private void CloseSettings(object control)
        {
            if (control is not UserControl thisPage) return;

            if (!IsAnythingChanged)
            {
                Window.GetWindow(thisPage).Close();
            }
            var msgResult = MessageBox.Show("Ви внесли не збережені зміни. Все одно закрити налаштування?", "Налаштування",
                MessageBoxButton.YesNo);
            if (msgResult == MessageBoxResult.Yes)
            {
                Window.GetWindow(thisPage).Close();
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}