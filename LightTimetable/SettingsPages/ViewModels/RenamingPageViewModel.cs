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
            ChangeRenameCommand = new RelayCommand(_ => ChangeRename());
            RemoveRenameCommand = new RelayCommand(_ => RemoveRename());
        }
        public override void Save()
        {
            Default.Renames = _renamesList.ToDictionary(x => x.Key, x => x.Value);
            Default.Save();
            IsAnythingChanged = false;
        }

        #region Properties

        private List<KeyValuePair<string, string>> _renamesList = Default.Renames.ToList();
        private KeyValuePair<string, string>? _selectedItem;

        public ObservableCollection<KeyValuePair<string, string>> RenamesList
        {
            get => new (_renamesList.Select(x => x));
            set => SetProperty(ref _renamesList, value.ToList());
        }

        public KeyValuePair<string, string>? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        #endregion

        #region Commands

        public RelayCommand ChangeRenameCommand { get; }
        public RelayCommand RemoveRenameCommand { get; }

        private void ChangeRename()
        {
            if (SelectedItem == null)
                return;
            string newItemName = new InputBox("Перейменування", $"Введіть нову назву для \"{SelectedItem?.Key}\":", SelectedItem?.Value).GetText();
            if (string.IsNullOrWhiteSpace(newItemName))
                return;
            var item = _renamesList.First(x => x.Key == SelectedItem?.Key);
            _renamesList.Remove(item);
            _renamesList.Add(new KeyValuePair<string, string>(item.Key, newItemName));
            OnPropertyChanged(nameof(RenamesList));

        }

        private void RemoveRename()
        {
            if (SelectedItem == null)
                return;

            var item = _renamesList.First(x => x.Key == SelectedItem?.Key);
            _renamesList.Remove(item);
            OnPropertyChanged(nameof(RenamesList));
        }

        #endregion
    }
}
