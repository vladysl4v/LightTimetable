using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;


namespace LightTimetable.ViewModels.Pages
{
    public abstract partial class PageViewModelBase : ObservableObject 
    {   
        [ObservableProperty]
        private bool _isAnythingChanged;

        public abstract void Save();

        protected PageViewModelBase()
        {
            PropertyChanged += AnythingChangedEvent;
        }

        private void AnythingChangedEvent(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(IsAnythingChanged))
                return;
            _isAnythingChanged = true;
            OnPropertyChanged(nameof(IsAnythingChanged));
        }


        #region Button commands

        [RelayCommand]
        private void SaveSettings()
        {
            Save();
            
            IsAnythingChanged = false;
        }

        [RelayCommand]
        private void SaveAndClose(object control)
        {
            if (control is not UserControl and not Window) return;

            Save();
            
            IsAnythingChanged = false;

            switch (control)
            {
                case Window window:
                    window.Close(); 
                    break;

                case UserControl userControl:
                    CloseSettings(userControl); 
                    break;
            }
        }

        [RelayCommand]
        private void CloseSettings(UserControl? page)
        {
            if (!IsAnythingChanged)
            {
                Window.GetWindow(page).Close();
                return;
            }
            var msgResult = MessageBox.Show("Ви внесли не збережені зміни. Все одно закрити налаштування?", "Налаштування",
                MessageBoxButton.YesNo);
            if (msgResult == MessageBoxResult.Yes)
            {
                Window.GetWindow(page).Close();
            }
        }

        #endregion
    }
}