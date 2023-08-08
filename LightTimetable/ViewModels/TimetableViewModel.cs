using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.ViewModels.Commands;
using LightTimetable.Handlers.Abstractions;


namespace LightTimetable.ViewModels
{
    public partial class TimetableViewModel : ObservableObject, IDisposable
    {
        private readonly ContextMenuCommandHandler _commands;
        private readonly UpdatesMediator _mediator;
        private readonly IDataManager _dataManager;

        public TimetableViewModel(IDataManager dataManager, UpdatesMediator mediator, IUserSettings settings)
        {
            _commands = new ContextMenuCommandHandler(settings);
            _dataManager = dataManager;
            _mediator = mediator;

            _dataManager.Dates.OnDateChanged += OnDateChanged;
            _dataManager.OnStatusChanged += OnStatusChanged;
            _mediator.OnUpdateRequired += UpdateDataGrid;

            ScheduleStatus = TimetableStatus.LoadingData;
            AvailableDates = Array.Empty<DateTime>();

            Task.Run(LoadDataAsync).ConfigureAwait(false);
        }

        #region Properties

        public DateTime Date
        {
            get => _dataManager.Dates.Date;
            set => SetProperty(_dataManager.Dates.Date, value,
                _dataManager.Dates, (model, date) => model.Date = date);
        }

        public List<DataItem> CurrentSchedule => _dataManager.GetSchedule(Date);

        [ObservableProperty]
        public TimetableStatus _scheduleStatus;

        [ObservableProperty]
        private DateTime[] _availableDates;

        [ObservableProperty]    
        private DataItem? _selectedDataItem;

        #endregion

        #region Methods

        public async Task LoadDataAsync()
        {
            await _dataManager.LoadScheduleAsync();
            AvailableDates = _dataManager.Dates.AvailableDates;
            UpdateDataGrid();
        }

        public void UpdateDataGrid()
        {
            // Easiest way to update datagrid (imo)
            DateTime temp = Date;
            Date = DateTime.MinValue;
            Date = temp;
        }

        private void OnStatusChanged()
        {
            ScheduleStatus = _dataManager.Status;
        }

        private void OnDateChanged()
        {
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(CurrentSchedule));
        }

        public void Dispose()
        {
            _mediator.OnUpdateRequired -= UpdateDataGrid;
            _dataManager.Dates.OnDateChanged -= OnDateChanged;
            _dataManager.OnStatusChanged -= OnStatusChanged;
        }
        
        #endregion

        #region Commands

        [RelayCommand]
        private void ResetDate()
        {
            _dataManager.Dates.SetCorrectDate();
        }

        [RelayCommand]
        private void ChangeDate(string amount)
        {
            _dataManager.Dates.ChangeDate(int.Parse(amount));
        }

        [RelayCommand]
        private void OpenInTeams()
        {
            _commands.OpenInTeams(SelectedDataItem);
        }

        [RelayCommand]
        private void AddNote()
        {
            _commands.AddNote(SelectedDataItem);
            UpdateDataGrid();
        }

        [RelayCommand]
        private void ChangeNote()
        {
            _commands.ChangeNote(SelectedDataItem);
            UpdateDataGrid();
        }

        [RelayCommand]
        private void DeleteNote()
        {
            _commands.DeleteNote(SelectedDataItem);
            UpdateDataGrid();
        }

        [RelayCommand]
        private void RenameItem()
        {
            _commands.RenameItem(SelectedDataItem);
            UpdateDataGrid();
        }

        #endregion
    }
}
