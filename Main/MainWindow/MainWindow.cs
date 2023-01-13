using System.Windows;

namespace Main
{
    /// <summary>
    /// Main logic for MainWindow
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FillTimetable();
            SetLblCurrDate();
            // Temporary positioning
            this.Left = SystemParameters.FullPrimaryScreenWidth - Width;
            this.Top = SystemParameters.FullPrimaryScreenHeight - 90;
            
        }

        private void SetLblCurrDate()
        {
            lblCurrDate.Content = UserData.Date.ToShortDateString();
        }

        private void RenderWidgets()
        {
            UserData.SetSubjectCount();
            SetLblCurrDate();
            FillTimetable();
        }
    }
}
