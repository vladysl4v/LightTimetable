using System.Windows;


namespace Timetable.Main
{
    /// Interactions with widgets in MainWindow 
    public partial class MainWindow : Window
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
            UserData.InitDate();
            RenderWidgets();
        }

        private void lblCurrDate_Click(object sender, RoutedEventArgs e)
        {
            if (datepickerWindow.Value.isVisible == true)
                datepickerWindow.Value.Hide();
            else
                datepickerWindow.Value.Show();
        }

        private void btnShowTeachersColumn_Click(object sender, RoutedEventArgs e)
        {
            this.Width = (Width == 400) ? 500 : 400;
        }
    }
}