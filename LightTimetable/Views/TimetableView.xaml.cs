using System;
using System.Windows;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.Models.Enums;
using LightTimetable.Views.Utilities;


namespace LightTimetable.Views
{
    // intentional violation of MVVM
    public partial class TimetableView : Window
    {
        private readonly UpdatesMediator _mediator;
        private readonly IUserSettings _settings;

        private SizeChangedEventHandler _sizeChanged;

        public TimetableView(object dataContext, UpdatesMediator mediator, IUserSettings settings)
        {
            _settings = settings;
            _mediator = mediator;

            this.Closed += OnClosed;
            _mediator.OnRepositionRequired += InvokeWindowResize;
            _sizeChanged = PositionCalculator.Calculate((WindowPosition)_settings.WindowPosition);
            Width = _settings.ShowOutages ? 425 : 400;

            InitializeComponent();
            DataContext = dataContext;
        }

        public void InvokeWindowResize()
        {
            Width = _settings.ShowOutages ? 425 : 400;
            Width += TimetableExpander.IsExpanded ? 100 : 0;
            _sizeChanged = PositionCalculator.Calculate((WindowPosition)_settings.WindowPosition);
            _sizeChanged.Invoke(this, null);
        }

        public void OnCloseButtonClick(object? s, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void OnTimetableExpanded(object? s, RoutedEventArgs e)
        {
            this.Width = (TimetableExpander.IsExpanded ? 500 : 400) + (_settings.ShowOutages ? 25 : 0);
        }

        public void WindowSizeChanged(object? s, SizeChangedEventArgs e)
        {
            _sizeChanged.Invoke(s, e);
        }

        public void OnClosed(object? s, EventArgs e)
        {
            _mediator.OnRepositionRequired -= InvokeWindowResize;
            Application.Current.Shutdown();
        }
    }
}
