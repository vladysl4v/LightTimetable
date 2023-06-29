using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

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
    public partial class TimetableViewModel : ObservableObject
    {
        private readonly DataProvider _dataProvider;
        private DateControl _dateControl;

        public TimetableViewModel()
        {
            WindowMediator.OnUpdateRequired += UpdateDataGrid;
            WindowMediator.OnReloadRequired += () => Task.Run(ReloadDataAsync).ConfigureAwait(false);

            _dateControl = new DateControl();
            _dataProvider = new DataProvider();
            
            Task.Run(ReloadDataAsync).ConfigureAwait(false);
        }

        #region Properties

        [ObservableProperty]
        private bool _isDataGridExpanded;

        [ObservableProperty]
        public DateTime[] _availableDates;

        [ObservableProperty]    
        private DataItem? _selectedDataItem;

        [ObservableProperty]
        private double _width = Settings.Default.ShowOutages ? 425 : 400;

        [ObservableProperty]
        private TimetableStatus _scheduleStatus = TimetableStatus.Default;

        public List<DataItem> CurrentSchedule
        {
            get
            {
                var correctSchedule = _dataProvider.GetCurrentSchedule(Date, out var currentStatus);
                ScheduleStatus = currentStatus;
                return correctSchedule;
            }
        }

        public DateTime Date
        {
            get => _dateControl.Date;
            set => SetProperty(_dateControl.Date, value, _dateControl, (model, date) => model.Date = date);
        }

        #endregion

        #region Methods

        public async Task ReloadDataAsync()
        {
            ScheduleStatus = TimetableStatus.LoadingData;
    
            ScheduleStatus = await _dataProvider.RefreshDataAsync();

            _dateControl = new DateControl(_dataProvider.AvailableDates, () => {
                OnPropertyChanged(nameof(Date));
                OnPropertyChanged(nameof(CurrentSchedule));  
            });

            AvailableDates = _dataProvider.AvailableDates;
            
            UpdateDataGrid();
        }

        public void UpdateDataGrid()
        {
            // temporary condition to update data grid cells
            DateTime temp = Date;
            Date = DateTime.MinValue;
            Date = temp;
        }

        partial void OnIsDataGridExpandedChanged(bool newValue)
        {
            var tempWidth = newValue ? 500 : 400; 
            tempWidth += Settings.Default.ShowOutages ? 25 : 0;
            Width = tempWidth;    
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
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void ResetDate()
        {
            _dateControl?.SetCorrectDate();
        }

        [RelayCommand]
        private void ChangeDate(string amount)
        {
            _dateControl?.ChangeDate(int.Parse(amount));
        }

        [RelayCommand]
        private void HideWindow(Window window)
        {
            window.Hide();
        }

        #endregion

        #region Context menu

        [RelayCommand]
        private void OpenInTeams()
        {
            if (SelectedDataItem == null || SelectedDataItem.OutlookEvents == null)
                return;

            if (SelectedDataItem.OutlookEvents.Count == 1)
            {
                var outlookEvent = SelectedDataItem.OutlookEvents.First();
                var message =
                    $"Ви впевнені, що хочете зайти на нараду \"{outlookEvent.Subject?.Trim()}\"?\nВона буде відкрита в Microsoft Teams.";
                var mbResult = MessageBox.Show(message, SelectedDataItem.Discipline.Modified, MessageBoxButton.YesNo);
                if (mbResult == MessageBoxResult.Yes)
                {
                    OpenLinkInBrowser(outlookEvent.OnlineMeeting.JoinUrl);
                }
            }
            else
            { 
                var selectedEvent = EventPicker.Show(SelectedDataItem.Discipline.Modified, SelectedDataItem.OutlookEvents);
                if (selectedEvent != null)
                {
                    OpenLinkInBrowser(selectedEvent.OnlineMeeting.JoinUrl);
                }
            }
        }

        [RelayCommand]
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

        [RelayCommand]
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

        [RelayCommand]
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

        [RelayCommand]
        private void RenameItem()
        {
            if (SelectedDataItem == null)
                return;
            string newItemName = InputBox.Show("Перейменування", $"Введіть нову назву для \"{SelectedDataItem.Discipline.Original}\":", SelectedDataItem.Discipline.Modified);
            if (string.IsNullOrWhiteSpace(newItemName) || newItemName == SelectedDataItem.Discipline.Modified) 
                return;

            Settings.Default.Renames[SelectedDataItem.Discipline.Original] = newItemName;
            Settings.Default.Save();

            UpdateDataGrid();
        }

        #endregion
    }
}
