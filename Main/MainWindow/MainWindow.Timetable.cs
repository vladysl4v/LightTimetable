using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace Main
{
    /// Timetable logic in MainWindow
    public partial class MainWindow : Window
    {
        public ObservableCollection<Lesson> ListLessons { get; set; } = new ObservableCollection<Lesson>();

        private void FillTimetable()
        {
            ListLessons.Clear();       
            foreach (var lesson in UserData.Content.Where(lesson => lesson["full_date"] == UserData.Date.ToShortDateString()))
            {
                ListLessons.Add(
                    new Lesson()
                    {
                        Study_time = lesson["study_time"],
                        Discipline = lesson["discipline"],
                        Study_type = lesson["study_type"],
                        Cabinet = lesson["cabinet"],
                        Employee = EmployeeShortener(lesson["employee"])
                    });
            }
            DataContext = this;
            UserData.LessonsCount = ListLessons.Count;
            // Window height depending on the number of lessons / their miss     
            Height = UserData.LessonsCount > 0 ? (50 + 20 * UserData.LessonsCount) : 90;
        }

        private string EmployeeShortener(string employee)
        {
            if (employee != default)
            {
                string[] EmplSplitted = employee.Split();
                return $"{EmplSplitted[0]} {EmplSplitted[1][0]}.{EmplSplitted[2][0]}.";
            }
            return "";
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