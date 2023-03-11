using System.Windows.Controls;

using LightTimetable.Tools;
using LightTimetable.SettingsPages;

using static LightTimetable.Properties.Settings;


namespace LightTimetable.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            // Default page
            ChangePage("TimetablePage");

            // Commands
            ChangePageCommand    = new RelayCommand(ChangePage);
            SaveSettingsCommand  = new RelayCommand(_ => SaveSettings());
            ResetSettingsCommand = new RelayCommand(_ => ResetSettings());
        }

        #region Properties

        private Page _currentPage = null!;
        private string _lvl1Header = null!;
        private string _lvl2Header = null!;
        public Page CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public string Lvl1Header
        {
            get => _lvl1Header;
            set => SetProperty(ref _lvl1Header, value.ToUpper());
        }

        public string Lvl2Header
        {
            get => _lvl2Header;
            set => SetProperty(ref _lvl2Header, value);
        }
        #endregion

        #region Commands

        public RelayCommand ChangePageCommand { get; }
        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand ResetSettingsCommand { get; }

        private void ChangePage(object pageName)
        {
            if ((string)pageName == "TimetablePage")
            {
                CurrentPage = new TimetablePage();
                Lvl1Header = "Розклад";
                Lvl2Header = "Виберіть свою навчальну групу";
            }
        }

        private void SaveSettings()
        {
            Default.Save();
        }

        private void ResetSettings()
        {
            Default.Reload();
        }
        
        #endregion

    }
}
