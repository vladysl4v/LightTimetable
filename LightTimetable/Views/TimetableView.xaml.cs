using System.Windows;
using System.Threading.Tasks;

using LightTimetable.Tools;
using LightTimetable.ViewModels;


namespace LightTimetable.Views
{
    /// <summary>
    /// Interaction logic for TimetableView.xaml
    /// </summary>
    public partial class TimetableView : Window
    {
        private WindowPositionControl _positionController;
        public TimetableView()
        {
            InitializeComponent();

            _positionController = new WindowPositionControl();
        }

        public async Task ReloadViewModelData()
        {
            if (DataContext is TimetableViewModel viewModel)
            {
                await viewModel.ReloadData();
            }
        }

        public void InvokeWindowResize()
        {
            _positionController = new WindowPositionControl();
            _positionController.OnWindowSizeChanged.Invoke(this, null);
        }

        private void OnWindowSizeChanged(object s, SizeChangedEventArgs e)
        {
            _positionController.OnWindowSizeChanged.Invoke(s, e);
        }
    }
}
