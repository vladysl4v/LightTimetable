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
            ChangePageCommand.Execute("TimetablePage");
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
        private RelayCommand _changePageCommand = null!;
        private RelayCommand _saveSettingsCommand = null!;
        private RelayCommand _resetSettingsCommand = null!;
        public RelayCommand ChangePageCommand
        {
            get
            {
                return _changePageCommand ??= new RelayCommand(obj =>
                {
                    if ((string)obj != "TimetablePage") 
                        return;
                    CurrentPage = new TimetablePage();
                    Lvl1Header = "Розклад";
                    Lvl2Header = "Виберіть свою навчальну групу";
                });
            }
        }
        
        public RelayCommand SaveSettingsCommand
        {
            get
            {
                return _saveSettingsCommand ??= new RelayCommand(obj =>
                {
                    Default.Save();
                });
            }
        }
        
        public RelayCommand ResetSettingsCommand
        {
            get
            {
                return _resetSettingsCommand ??= new RelayCommand(obj =>
                {
                    Default.Reload();
                });
            }
        }
        #endregion

    }
}
