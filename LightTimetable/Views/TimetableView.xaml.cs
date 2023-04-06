using System;
using System.Threading.Tasks;
using System.Windows;

using LightTimetable.ViewModels;


namespace LightTimetable.Views
{
    /// <summary>
    /// Interaction logic for TimetableView.xaml
    /// </summary>
    public partial class TimetableView : Window
    {
        public TimetableView()
        {
            InitializeComponent();
        }

        public async Task ReloadViewModelData()
        {
            if (DataContext is TimetableViewModel viewModel)
            {
                await viewModel.ReloadData();
            }
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var isWidthChanged = Math.Abs(e.PreviousSize.Width - e.NewSize.Width) > 10;
            var isHeightChanged = Math.Abs(e.PreviousSize.Height - e.NewSize.Height) > 10;

            if (isHeightChanged)
                this.Top = SystemParameters.WorkArea.Height - e.NewSize.Height;

            if (isWidthChanged)
                this.Left = SystemParameters.FullPrimaryScreenWidth - e.NewSize.Width;
        }
    }
}
