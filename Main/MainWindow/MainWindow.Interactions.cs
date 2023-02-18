using System.Windows;


namespace Timetable.Main
{
    /// Interactions with widgets in MainWindow 
    public partial class MainWindow
    {
        private void btnNavClose_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnNavNext_Click(object sender, RoutedEventArgs e)
        {
            UserData.ChangeDate(+1);
            RenderWidgets();
        }

        private void btnNavPrev_Click(object sender, RoutedEventArgs e)
        {
            UserData.ChangeDate(-1);
            RenderWidgets();
        }

        private void btnNavHome_Click(object sender, RoutedEventArgs e)
        {
            UserData.InitializeDate();
            RenderWidgets();
        }
        
        private void btnShowTeachersColumn_Click(object sender, RoutedEventArgs e)
        {
            if (employeeColumn.Visibility == Visibility.Hidden)
            {
                employeeColumn.Visibility = Visibility.Visible;
                this.Width = 500;
            }
            else
            {
                employeeColumn.Visibility = Visibility.Hidden;
                this.Width = 400;
            }
        }
    }
}