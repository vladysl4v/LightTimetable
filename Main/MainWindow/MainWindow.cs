using System;
using System.Windows;
using System.Windows.Media;

namespace Main
{
    /// <summary>
    /// Central logic for MainWindow
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RenderWidgets();
            InitializeTray();
            // Positioning
            this.SizeChanged += OnWindowSizeChanged;
        }

        private void SetLblCurrDate()
        {
            lblCurrDate.Content = UserData.Date.ToShortDateString();

            if (UserData.Date.Date == DateTime.Now.Date)
            {
                lblCurrDate.Foreground = Brushes.SeaGreen;
            }
            else
            {
                lblCurrDate.Foreground = Brushes.Black;
            }
            SetDayOfWeek();
        }

        private void SetDayOfWeek()
        {
            string localized = string.Empty;
            switch (UserData.Date.DayOfWeek)
            {
                case DayOfWeek.Monday:     localized = "понедiлок"; break;
                case DayOfWeek.Tuesday:    localized = "вiвторок"; break;
                case DayOfWeek.Wednesday:  localized = "середу"; break;
                case DayOfWeek.Thursday:   localized = "четвер"; break;
                case DayOfWeek.Friday:     localized = "п'ятницю"; break;
                case DayOfWeek.Saturday:   localized = "суботу"; break;
                case DayOfWeek.Sunday:     localized = "недiлю"; break;
            }
            lblDayOfWeek.Content = "Розклад на " + localized;
        }

        private void RenderWidgets()
        {
            SetLblCurrDate();
            FillTimetable();
        }

        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Top = SystemParameters.WorkArea.Height - e.NewSize.Height;
            this.Left = SystemParameters.FullPrimaryScreenWidth - e.NewSize.Width;
        }
    }
}
