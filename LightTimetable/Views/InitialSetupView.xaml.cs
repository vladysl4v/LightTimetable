using System.Windows;


namespace LightTimetable.Views
{
    /// <summary>
    /// Interaction logic for InitialSetupView.xaml
    /// </summary>
    public partial class InitialSetupView : Window
    {
        public InitialSetupView(object dataContext)
        {
            InitializeComponent();
            
            DataContext = dataContext;
        }
    }
}
