using System.Windows;


namespace Main
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
        private void btnShowTeachersColumn_Click(object sender, RoutedEventArgs e)
        {
            this.Width = (Width == 400) ? 500 : 400;
        }
    }
}