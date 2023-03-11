using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

using LightTimetable.Model;
using LightTimetable.Tools;

using static LightTimetable.Properties.Settings;


namespace LightTimetable.ViewModels
{
    public class TimetableViewModel : ViewModelBase
    {
        private readonly DataProvider _dataProvider;
        private readonly DateControl _dateControl;

        public TimetableViewModel()
        {
            _dataProvider = new DataProvider();
            _dateControl = new DateControl(_dataProvider.AvailableDates);

            // Commands
            HideWindowCommand = new RelayCommand(HideWindow);
            ChangeDateCommand = new RelayCommand(ChangeDate);
            ResetDateCommand  = new RelayCommand(_ => ResetDate());

            AddNoteCommand    = new RelayCommand(_ => AddNote());
            ChangeNoteCommand = new RelayCommand(_ => ChangeNote());
            DeleteNoteCommand = new RelayCommand(_ => DeleteNote());
            RenameItemCommand = new RelayCommand(_ => RenameItem());
        }

        #region Properties

        private bool _isDataGridExpanded;
        private DataItem _selectedDataItem;

        public List<DataItem> CurrentSchedule
        {
            get
            {
                if (Date == DateTime.MinValue)
                    return new List<DataItem>();

                return _dataProvider.ScheduleDictionary.TryGetValue(Date, out List<DataItem> correctDataItems) ? correctDataItems : new List<DataItem>();
            }
        }

        public DataItem SelectedDataItem
        {
            get => _selectedDataItem;
            set => SetProperty(ref _selectedDataItem, value);

        }

        public bool IsDataGridExpanded
        {
            get => _isDataGridExpanded;
            set => SetProperty(ref _isDataGridExpanded, value);
        }

        public DateTime Date
        {
            get => _dateControl.Date;
            set
            {
                _dateControl.Date = value;
                OnDateChanged();
            }
        }

        public DateTime[] AvailableDates
        {
            get => _dataProvider.AvailableDates;
        }
        #endregion

        #region Commands

        public RelayCommand HideWindowCommand { get; }
        public RelayCommand ChangeDateCommand { get; }
        public RelayCommand ResetDateCommand { get; }

        private void ResetDate()
        {
            _dateControl.SetCorrectDate();
            OnDateChanged();
        }

        private void ChangeDate(object amount)
        {
            _dateControl.ChangeDate(int.Parse((string)amount));
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

        public RelayCommand AddNoteCommand { get; }
        public RelayCommand ChangeNoteCommand { get; }
        public RelayCommand DeleteNoteCommand { get; }
        public RelayCommand RenameItemCommand { get; }

        private void AddNote()
        {
            string noteText = new InputBox("Нотатка", "Введіть текст нотатки:").GetText();
            if (noteText.Any())
            {
                Default.AppendToNotes(SelectedDataItem.Id, noteText);
                SelectedDataItem.Note = noteText;
                UpdateDataGrid();
            }
        }

        private void ChangeNote()
        {
            string noteText = new InputBox("Нотатка", "Введіть новий текст нотатки:", SelectedDataItem.Note).GetText();
            if (noteText.Any() && noteText != SelectedDataItem.Note)
            {
                Default.AppendToNotes(SelectedDataItem.Id, noteText);
                SelectedDataItem.Note = noteText;
                UpdateDataGrid();
            }
        }
        private void DeleteNote()
        {
            MessageBoxResult result = MessageBox.Show("Ви впевнені що хочете видалити цю нотатку?", "Нотатка", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Default.RemoveFromNotes(SelectedDataItem.Id);
                SelectedDataItem.Note = string.Empty;
                UpdateDataGrid();
            }
        }

        private void RenameItem()
        {
            string newItemName = new InputBox("Перейменування", $"Введіть нову назву для \"{SelectedDataItem.Discipline.Value}\":", SelectedDataItem.Discipline.Key).GetText();
            if (newItemName.Any() && newItemName != SelectedDataItem.Discipline.Key)
            {
                Default.AppendToRenames(SelectedDataItem.Discipline.Value, newItemName);
                SelectedDataItem.Discipline.Key = newItemName;
                UpdateDataGrid(SelectedDataItem.Discipline);
            }
        }

        #endregion

        #region Methods

        private void OnDateChanged()
        {
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(CurrentSchedule));
        }

        public void UpdateDataGrid(MutablePair<string, string> renamePair = null)
        {
            // temporary condition to update data grid cells
            if (renamePair != null)
                UpdateRenames(renamePair);
            DateTime temp = Date;
            Date = DateTime.MinValue;
            Date = temp;
        }

        private void UpdateRenames(MutablePair<string, string> renamePair)
        {
            foreach (var item in from dateItems in _dataProvider.ScheduleDictionary.Values from item in dateItems where item.Discipline.Value == renamePair.Value select item)
            {
                item.Discipline.Key = renamePair.Key;
            }
        }

        public void ReloadData()
        {
            _dataProvider.ReloadData();
            OnPropertyChanged(nameof(AvailableDates));
            _dateControl.UpdateDates(_dataProvider.AvailableDates);
        }

        #endregion
    }
}
