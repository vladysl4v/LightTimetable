using System;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using LightTimetable.Tools;
using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.Models.Utilities;
using LightTimetable.Tools.UtilityWindows;


namespace LightTimetable.ViewModels
{
    public class TimetableViewModel : ViewModelBase
    {
        private readonly DataProvider _dataProvider = new();
        private DateControl? _dateControl;

        public TimetableViewModel()
        {
            Task.Run(ReloadDataAsync).ConfigureAwait(false);

            // Commands
            HideWindowCommand = new RelayCommand(HideWindow);
            ChangeDateCommand = new RelayCommand(ChangeDate);
            ResetDateCommand = new RelayCommand(_ => ResetDate());

            OpenInTeamsCommand = new RelayCommand(_ => OpenInTeams());
            AddNoteCommand = new RelayCommand(_ => AddNote());
            ChangeNoteCommand = new RelayCommand(_ => ChangeNote());
            DeleteNoteCommand = new RelayCommand(_ => DeleteNote());
            RenameItemCommand = new RelayCommand(_ => RenameItem());
        }

        #region Properties

        public TimetableStatusControl TtControl { get; } = new(TimetableStatus.Default);

        private double _width = 400;

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public List<DataItem> CurrentSchedule
        {
            get
            {
                if (Date == DateTime.MinValue)
                    return new List<DataItem>();

                var correctSchedule = _dataProvider.GetCurrentSchedule(Date, out bool isRigged);

                if (isRigged)
                {
                    TtStatus = TimetableStatus.RiggedScheduleShown;
                }
                else
                {
                    if (TtStatus != TimetableStatus.DataLoadingError)
                        TtStatus = TimetableStatus.Default;
                }

                return correctSchedule;
            }
        }

        public DateTime Date
        {
            get => _dateControl?.Date ?? DateTime.MinValue;
            set
            {
                if (_dateControl == null)
                    return;
                _dateControl.Date = value;
                OnDateChanged();
            }
        }

        public TimetableStatus TtStatus
        {
            get => TtControl.Type;
            set
            {
                TtControl.Type = value;
                OnPropertyChanged(nameof(TtControl));
            }
        }

        private DataItem? _selectedDataItem;

        public DataItem? SelectedDataItem
        {
            get => _selectedDataItem;
            set => SetProperty(ref _selectedDataItem, value);
        }

        private bool _isDataGridExpanded;

        public bool IsDataGridExpanded
        {
            get => _isDataGridExpanded;
            set
            {
                Width = value ? 500 : 400;
                SetProperty(ref _isDataGridExpanded, value);
            }
        }

        public DateTime[] AvailableDates
        {
            get => _dataProvider.AvailableDates;
        }
        #endregion

        #region Methods

        public async Task ReloadDataAsync()
        {
            TtStatus = TimetableStatus.LoadingData;

            if (!await _dataProvider.RefreshDataAsync())
            {
                TtStatus = TimetableStatus.DataLoadingError;
            }
            else
            {
                TtStatus = TimetableStatus.Default;
            }

            _dateControl = new DateControl(_dataProvider.AvailableDates);

            OnPropertyChanged(nameof(AvailableDates));
            UpdateDataGrid();
        }

        public void UpdateDataGrid(DisciplinePair? renamePair = null)
        {
            // temporary condition to update data grid cells
            if (renamePair != null)
                _dataProvider.UpdateRenames(renamePair);
            DateTime temp = Date;
            Date = DateTime.MinValue;
            Date = temp;
        }

        private void OpenLinkInBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }

            MessageBox.Show("LightTimetable", "ваша операційна система не підтримується", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnDateChanged()
        {
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(CurrentSchedule));
        }

        #endregion

        #region Commands

        public RelayCommand HideWindowCommand { get; }
        public RelayCommand ChangeDateCommand { get; }
        public RelayCommand ResetDateCommand { get; }

        private void ResetDate()
        {
            _dateControl?.SetCorrectDate();
            OnDateChanged();
        }

        private void ChangeDate(object amount)
        {
            _dateControl?.ChangeDate(int.Parse((string)amount));
            OnDateChanged();
        }

        private void HideWindow(object win)
        {
            if (win is Window thisWindow)
            {
                thisWindow.Hide();
            }
        }

        #endregion

        #region Context menu
        public RelayCommand OpenInTeamsCommand { get; }
        public RelayCommand AddNoteCommand { get; }
        public RelayCommand ChangeNoteCommand { get; }
        public RelayCommand DeleteNoteCommand { get; }
        public RelayCommand RenameItemCommand { get; }

        private void OpenInTeams()
        {
            if (SelectedDataItem == null || SelectedDataItem.OutlookEvents == null)
                return;

            if (SelectedDataItem.OutlookEvents.Count == 1)
            {
                var outlookEvent = SelectedDataItem.OutlookEvents.First();
                var message =
                    $"Ви впевнені, що хочете зайти на нараду \"{outlookEvent.Title.Trim()}\"?\nВона буде відкрита в Microsoft Teams.";
                var mbResult = MessageBox.Show(message, SelectedDataItem.Discipline.Modified, MessageBoxButton.YesNo);
                if (mbResult == MessageBoxResult.Yes)
                {
                    OpenLinkInBrowser(outlookEvent.Url);
                }
            }
            else
            {
                var eventPickerWindow =
                    new EventPicker(SelectedDataItem.Discipline.Modified, SelectedDataItem.OutlookEvents);
                eventPickerWindow.Show();
            }
        }

        private void AddNote()
        {
            if (SelectedDataItem == null)
                return;
            string noteText = InputBox.Show("Нотатка", "Введіть текст нотатки:");
            if (string.IsNullOrWhiteSpace(noteText))
                return;

            Settings.Default.Notes[SelectedDataItem.Id] = noteText;
            Settings.Default.Save();

            SelectedDataItem.Note = noteText;
            UpdateDataGrid();
        }

        private void ChangeNote()
        {
            if (SelectedDataItem == null)
                return;
            string noteText = InputBox.Show("Нотатка", "Введіть новий текст нотатки:", SelectedDataItem.Note);
            if (string.IsNullOrWhiteSpace(noteText) || noteText == SelectedDataItem.Note) 
                return;

            Settings.Default.Notes[SelectedDataItem.Id] = noteText;
            Settings.Default.Save();

            SelectedDataItem.Note = noteText;
            UpdateDataGrid();
        }
        private void DeleteNote()
        {
            if (SelectedDataItem == null)
                return;
            var result = MessageBox.Show("Ви впевнені що хочете видалити цю нотатку?", "Нотатка", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) 
                return;

            Settings.Default.Notes.Remove(SelectedDataItem.Id);
            Settings.Default.Save();

            SelectedDataItem.Note = string.Empty;
            UpdateDataGrid();
        }

        private void RenameItem()
        {
            if (SelectedDataItem == null)
                return;
            string newItemName = InputBox.Show("Перейменування", $"Введіть нову назву для \"{SelectedDataItem.Discipline.Original}\":", SelectedDataItem.Discipline.Modified);
            if (string.IsNullOrWhiteSpace(newItemName) || newItemName == SelectedDataItem.Discipline.Modified) 
                return;

            Settings.Default.Renames[SelectedDataItem.Discipline.Original] = newItemName;
            Settings.Default.Save();

            SelectedDataItem.Discipline.Modified = newItemName;
            UpdateDataGrid(SelectedDataItem.Discipline);
        }

        #endregion
    }
}
