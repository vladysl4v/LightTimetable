using System.Diagnostics;
using System.Windows;
using System.Threading.Tasks;

using LightTimetable.Tools;
using LightTimetable.ViewModels;
using LightTimetable.Properties;

namespace LightTimetable.Views
{
    public partial class TimetableView : Window
    {
        private SizeChangedEventHandler _sizeChanged;
        public TimetableView()
        {
            InitializeComponent();
            _sizeChanged = PositionCalculator.Calculate((WindowPosition)Settings.Default.WindowPosition);
        }

        private TimetableViewModel? _viewModel => DataContext as TimetableViewModel;

        public async Task ReloadViewModelData()
        {
            await _viewModel?.ReloadDataAsync();
        }

        public void InvokeWindowResize()
        {
            Width = Settings.Default.ShowBlackouts ? 425 : 400;
            if (_viewModel != null)
            {
                Width += _viewModel.IsDataGridExpanded ? 100 : 0;
            }
            _sizeChanged = PositionCalculator.Calculate((WindowPosition)Settings.Default.WindowPosition);
            _sizeChanged.Invoke(this, null);
        }

        private void OnWindowSizeChanged(object s, SizeChangedEventArgs e)
        {
            _sizeChanged.Invoke(s, e);
        }
    }
}
