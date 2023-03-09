using System;
using System.Windows;

using LightTimetable.ViewModels;


namespace LightTimetable
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

        public void ReloadData()
        {
            ((TimetableViewModel)this.DataContext).ReloadData();
        }

        public void RefreshTimetable()
        {
            ((TimetableViewModel)this.DataContext).UpdateDataGrid();
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool isWidthChanged = Math.Abs(e.PreviousSize.Width - e.NewSize.Width) > 10;
             bool isHeightChanged = Math.Abs(e.PreviousSize.Height - e.NewSize.Height) > 10;

             if (!isHeightChanged && !isWidthChanged)
                 return;

             if (isHeightChanged && isWidthChanged)
             {
                 this.Left = SystemParameters.FullPrimaryScreenWidth - e.NewSize.Width;
                 this.Top = SystemParameters.WorkArea.Height - e.NewSize.Height;
             }
             if (isHeightChanged && !isWidthChanged)
                 this.Top = SystemParameters.WorkArea.Height - e.NewSize.Height;

             if (!isHeightChanged && isWidthChanged)
                 this.Left = SystemParameters.FullPrimaryScreenWidth - e.NewSize.Width;
        }
    }
}
