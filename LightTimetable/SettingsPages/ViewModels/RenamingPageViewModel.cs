using CommunityToolkit.Mvvm.Input;

using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using LightTimetable.Views;
using LightTimetable.Properties;
using LightTimetable.Tools.UtilityWindows;


namespace LightTimetable.SettingsPages.ViewModels
{
    public partial class RenamingPageViewModel : PageViewModelBase
    {
        #region Properties

        private List<KeyValuePair<string, string>> _renamesList = Settings.Default.Renames.ToList();

        public ObservableCollection<KeyValuePair<string, string>> RenamesList
        {
            get => new (_renamesList.Select(x => x));
            set => SetProperty(ref _renamesList, value.ToList());
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void ChangeRename(object selectedItem)
        {
            if (selectedItem == null)
                return;

            var thisItem = (KeyValuePair<string, string>)selectedItem;

            string newItemName = InputBox.Show("Перейменування", $"Введіть нову назву для \"{thisItem.Key}\":", thisItem.Value);
            if (string.IsNullOrWhiteSpace(newItemName))
                return;
            var item = _renamesList.First(x => x.Key == thisItem.Key);
            _renamesList.Remove(item);
            _renamesList.Add(new KeyValuePair<string, string>(item.Key, newItemName));
            IsAnythingChanged = true;
            OnPropertyChanged(nameof(RenamesList));

        }

        [RelayCommand]
        private void RemoveRename(object selectedItem)
        {
            if (selectedItem == null)
                return;

            var thisItem = (KeyValuePair<string, string>)selectedItem;
            var item = _renamesList.First(x => x.Key == thisItem.Key);
            _renamesList.Remove(item);
            IsAnythingChanged = true;
            OnPropertyChanged(nameof(RenamesList));
        }

        #endregion

        #region Methods

        public override void Save()
        {
            if (IsAnythingChanged)
            {
                SettingsView.IsRequiredReload = true;
            }
                
            Settings.Default.Renames = _renamesList.ToDictionary(x => x.Key, x => x.Value);
            Settings.Default.Save();

            IsAnythingChanged = false;
        }

        #endregion
    }
}
