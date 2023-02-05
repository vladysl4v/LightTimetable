using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;

namespace Timetable.Main.DatePicker
{
    /// <summary>
    /// Central logic for DatePickerWindow
    /// </summary>
    public partial class DatePickerWindow : Window
    {
        private readonly MainWindow _parentWindow;
        public bool isVisible { get; private set; } = false;
        public DatePickerWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            FillDatePicker();
            _parentWindow = mainWindow;
            this.Owner = mainWindow;
            ClarifyPosition();
        }
        public new void Show()
        {
            isVisible = true;
            base.Show();
        }
        public new void Hide()
        {
            isVisible = false;
            base.Hide();
        }
        public void ClarifyPosition()
        {
            this.Top = _parentWindow.Top - this.Height;
            this.Left = _parentWindow.Left;
        }
        public void RefreshDates()
        {
            gridDatePicker.Children.Clear();
            FillDatePicker();
            ClarifyPosition();
        }
        private void btnDate_Click(object sender, RoutedEventArgs e)
        {
            Button clicked_btn = (Button)sender;
            UserData.Date = Convert.ToDateTime(AutomationProperties.GetHelpText(clicked_btn));
            Hide();
            _parentWindow.RenderWidgets();
        }
        private void FillDatePicker()
        {
            int GridNum = 0;
            int MarginTop = 0;
            foreach (var date in UserData.ContentDates)
            {
                if (GridNum == 5)
                {
                    GridNum = 0;
                    MarginTop += 20;
                }
                Button DateButton = new Button
                {
                    Content = date.Day,
                    Height = 20,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness { Top = MarginTop },
                };
                // Using SetHelpText to bind a full date to button (Crutch?)
                AutomationProperties.SetHelpText(DateButton, date.ToShortDateString());

                DateButton.Click += btnDate_Click;

                Grid.SetColumn(DateButton, GridNum);

                gridDatePicker.Children.Add(DateButton);

                GridNum++;
            }
            this.Height = 20 + MarginTop;
        }
    }
}
