using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;


namespace Timetable.Main
{
    public partial class MainWindow
    {
        private void OpenCalendar_Click(object s, EventArgs e)
        {
            pickerCurrentDate.IsDropDownOpen = true;
        }
        private void SelectedDate_Changed(object s, SelectionChangedEventArgs e)
        {
            UserData.Date = pickerCurrentDate.SelectedDate ?? UserData.Date;
            RenderWidgets();
        }

        private void InitializeDatePicker()
        {
            pickerCurrentDate.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, UserData.ContentDates[0].AddDays(-1)));
            pickerCurrentDate.BlackoutDates.Add(new CalendarDateRange(UserData.ContentDates.Last().AddDays(1), DateTime.MaxValue));
            DateTime dateCounter = UserData.ContentDates[0];
            foreach (DateTime loopDate in UserData.ContentDates.Skip(1))
            {
                if (loopDate.AddDays(-1).Date != dateCounter.Date)
                {
                    pickerCurrentDate.BlackoutDates.Add(
                        new CalendarDateRange(dateCounter.AddDays(1), loopDate.AddDays(-1)));
                }

                dateCounter = loopDate;
            }
        }
    }

    public class CustomDatePicker : DatePicker
    {
        public List<DateTime> HighlightedDates { get; } = new();
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template.FindName("PART_Button", this) is Button button)
            {
                button.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}
