using CommunityToolkit.Mvvm.Input;

using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.Views.Utilities;


namespace LightTimetable.ViewModels.Pages
{
    public partial class RenamingPageViewModel : PageViewModelBase
    {
        private readonly UpdatesMediator _mediator;
        private readonly IUserSettings _settings;
        
        public RenamingPageViewModel(IUserSettings settings, UpdatesMediator mediator)
        {
            _settings = settings;
            _mediator = mediator;

            _renamesList = _settings.Renames.ToList();
        }

        #region Properties

        private List<KeyValuePair<string, string>> _renamesList;

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
            _settings.Renames = _renamesList.ToDictionary(x => x.Key, x => x.Value);
            _settings.Save();

            if (IsAnythingChanged)
            {
                _mediator.UpdateRequired();
            }
        }

        #endregion
    }
}
