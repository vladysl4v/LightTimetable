using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;


namespace Main
{
    /// <summary>
    /// Timetable logic in MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Lesson> ListLessons { get; set; } = new ObservableCollection<Lesson>();

        private void FillTimetable()
        {
            ListLessons.Clear();
            // Window height depending on the number of lessons / their miss
            Height = UserData.SubjectCount > 0 ? (50 + 20 * UserData.SubjectCount) : 90;
            foreach (var lesson in UserData.Content.Where(lesson => lesson["full_date"] == UserData.Date.ToShortDateString()))
            {
                ListLessons.Add(
                    new Lesson()
                    {
                        Study_time = lesson["study_time"],
                        Discipline = lesson["discipline"],
                        Study_type = lesson["study_type"],
                        Cabinet = lesson["cabinet"]
                    });
            }
            DataContext = this;
        }
    }

    /// <summary>
    /// Provides data source for timetable
    /// </summary>
    public record class Lesson
    {
        public string Study_time { get; init; } = string.Empty;
        public string Discipline { get; init; } = string.Empty;
        public string Study_type { get; init; } = string.Empty;
        public string Cabinet { get; init; } = string.Empty;
        public string Employee { get; init; } = string.Empty;
    }
}