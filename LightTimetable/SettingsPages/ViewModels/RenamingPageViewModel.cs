using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using LightTimetable.Tools;
using static LightTimetable.Properties.Settings;


namespace LightTimetable.SettingsPages.ViewModels
{
    class RenamingPageViewModel : PageViewModelBase
    {
        public RenamingPageViewModel()
        {
            // Commands
            ChangeRenameCommand = new RelayCommand(ChangeRename);
            RemoveRenameCommand = new RelayCommand(RemoveRename);
        }

        #region Properties

        private List<KeyValuePair<string, string>> _renamesList = Default.Renames.ToList();

        public ObservableCollection<KeyValuePair<string, string>> RenamesList
        {
            get => new (_renamesList.Select(x => x));
            set => SetProperty(ref _renamesList, value.ToList());
        }


        #endregion

        #region Commands

        public RelayCommand ChangeRenameCommand { get; }
        public RelayCommand RemoveRenameCommand { get; }

        private void ChangeRename(object selectedItem)
        {
            if (selectedItem == null)
                return;

            var thisItem = (KeyValuePair<string, string>)selectedItem;

            string newItemName = new InputBox("Перейменування", $"Введіть нову назву для \"{thisItem.Key}\":", thisItem.Value).GetText();
            if (string.IsNullOrWhiteSpace(newItemName))
                return;
            var item = _renamesList.First(x => x.Key == thisItem.Key);
            _renamesList.Remove(item);
            _renamesList.Add(new KeyValuePair<string, string>(item.Key, newItemName));

            IsAnythingChanged = true;
            OnPropertyChanged(nameof(RenamesList));

        }

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
            Default.Renames = _renamesList.ToDictionary(x => x.Key, x => x.Value);
            Default.Save();
            IsAnythingChanged = false;
        }

        #endregion
    }
}
