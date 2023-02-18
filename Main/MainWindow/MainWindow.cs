using System;
using System.Windows;
using System.Windows.Media;


namespace Timetable.Main
{
    /// <summary>
    /// Central logic for MainWindow
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeDatePicker();
            InitializeContextMenu();
            RenderWidgets();

            this.SizeChanged += OnWindowSizeChanged;
        }

        public void RenderWidgets()
        {
            ChangeCurrentDate();
            FillSchedule();
        }

        private void ChangeCurrentDate()
        {

            pickerCurrentDate.Text = UserData.Date.ToShortDateString();

            if (UserData.Date.Date == DateTime.Now.Date)
            {
                pickerCurrentDate.Foreground = Brushes.SeaGreen;
            }
            else
            {
                pickerCurrentDate.Foreground = Brushes.Black;
            }
            ChangeDayOfWeek();
        }
        private void ChangeDayOfWeek()
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

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Top = SystemParameters.WorkArea.Height - e.NewSize.Height;
            this.Left = SystemParameters.FullPrimaryScreenWidth - e.NewSize.Width;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
            base.OnStateChanged(e);
        }
    }

}
