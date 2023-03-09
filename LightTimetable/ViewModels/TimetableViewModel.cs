using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

using LightTimetable.Model;
using LightTimetable.Tools;
using LightTimetable.Tools.Data;

using static LightTimetable.Properties.Settings;


namespace LightTimetable.ViewModels
{
    public class TimetableViewModel : ViewModelBase
    {
        private readonly UserData _userData;

        public TimetableViewModel()
        {
            _userData = new UserData();
        }

        #region Properties

        private bool _isDataGridExpanded;
        private DataItem _selectedDataItem = null!;

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

        public Dictionary<DateTime, List<DataItem>> ScheduleData
        {
            get => _userData.ScheduleData;
        }

        public List<DateTime> AvailableDates
        {
            get => _userData.ScheduleData.Keys.ToList();
        }

        public DateTime Date
        {
            get => _userData.Date;
            set
            {
                _userData.Date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        #endregion

        #region Commands
        private RelayCommand _hideWindowCommand = null!;
        private RelayCommand _changeDateCommand = null!;
        private RelayCommand _resetDateCommand = null!;

        public RelayCommand HideWindowCommand
        {
            get
            {
                return _hideWindowCommand ??= new RelayCommand(objectWindow =>
                {
                    if (objectWindow is Window thisWindow)
                    {
                        thisWindow.Hide();
                    }
                });
            }
        }

        public RelayCommand ChangeDateCommand
        {
            get
            {
                return _changeDateCommand ??= new RelayCommand(amount =>
                  {
                    _userData.ChangeDate(int.Parse((string)amount));
                    OnPropertyChanged(nameof(Date));
                  });
            }
        }
        public RelayCommand ResetDateCommand
        {
            get
            {
                return _resetDateCommand ??= new RelayCommand(_ =>
                {
                    _userData.ResetDate();
                    OnPropertyChanged(nameof(Date));
                });
            }
        }

        #endregion

        #region Context menu

        private RelayCommand _changeNoteCommand = null!;
        private RelayCommand _deleteNoteCommand = null!;
        private RelayCommand _addNoteCommand = null!;
        private RelayCommand _renameItemCommand = null!;

        public RelayCommand ChangeNoteCommand
        {
            get
            {
                return _changeNoteCommand ??= new RelayCommand(obj =>
                {
                    string noteText = new InputBox("Нотатка", "Введіть новий текст нотатки:", SelectedDataItem.Note).GetText();
                    if (noteText.Any() && noteText != SelectedDataItem.Note)
                    {
                        Default.AppendToNotes(SelectedDataItem.ID, noteText);
                        SelectedDataItem.Note = noteText;
                        UpdateDataGrid();
                    }
                });
            }
        }

        public RelayCommand DeleteNoteCommand
        {
            get
            {
                return _deleteNoteCommand ??= new RelayCommand(obj =>
                {
                    MessageBoxResult result = MessageBox.Show("Ви впевнені що хочете видалити цю нотатку?", "Нотатка", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        Default.RemoveFromNotes(SelectedDataItem.ID);
                        SelectedDataItem.Note = string.Empty;
                        UpdateDataGrid();
                    }
                });
            }
        }

        public RelayCommand AddNoteCommand
        {
            get
            {
                return _addNoteCommand ??= new RelayCommand(obj =>
                {
                    string noteText = new InputBox("Нотатка", "Введіть текст нотатки:").GetText();
                    if (noteText.Any())
                    {
                        Default.AppendToNotes(SelectedDataItem.ID, noteText);
                        SelectedDataItem.Note = noteText;
                        UpdateDataGrid();
                    }
                });
            }
        }

        public RelayCommand RenameItemCommand
        {
            get
            {
                return _renameItemCommand ??= new RelayCommand(obj =>
                {
                    string newItemName= new InputBox("Перейменування", $"Введіть нову назву для \"{SelectedDataItem.Discipline.Value}\":", SelectedDataItem.Discipline.Key).GetText();
                    if (newItemName.Any() && newItemName != SelectedDataItem.Discipline.Key)
                    {
                        Default.AppendToRenames(SelectedDataItem.Discipline.Value, newItemName);
                        SelectedDataItem.Discipline.Key = newItemName;
                        UpdateDataGrid(SelectedDataItem.Discipline);
                    }
                });
            }
        }

        #endregion

        public void UpdateDataGrid(MutablePair<string, string> renamePair = null)
        {
            // temporary condition to update data grid cells
            if (renamePair != null)
                _userData.UpdateRenames(renamePair);
            DateTime temp = Date;
            Date = DateTime.MinValue;
            Date = temp;
        }

        public void ReloadData()
        {
            _userData.UpdateData();
        }
    }
}
