using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Timetable.Main
{
    /// Timetable logic in MainWindow
    public partial class MainWindow : Window
    {
        public ObservableCollection<Lesson> ListLessons { get; set; } = new ObservableCollection<Lesson>();
        private List<Label> ListSubgroupTips { get; set; } = new List<Label>();

        public void FillTimetable()
        {
            SubGroupTipsClear();
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
                if (lesson["study_subgroup"] != null)
                    AddSubgroupTip(lesson["study_subgroup"], ListLessons.Count);
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

        private void AddSubgroupTip(string text, int margin)
        {
            Label SubgroupTip = new Label
            {
                Content = FindResource("SubgroupIcon"),
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness { Top = 5 + (20 * margin)},
                ToolTip = text,
            };
            Grid.SetColumn(SubgroupTip, 3);
            Grid.SetRow(SubgroupTip, 1);
            ToolTipService.SetInitialShowDelay(SubgroupTip, 200);
            ListSubgroupTips.Add(SubgroupTip);
            gridMain.Children.Add(SubgroupTip);
        }

        private void SubGroupTipsClear()
        {
            foreach (var tip in ListSubgroupTips)
            {
                gridMain.Children.Remove(tip);
            }
            ListSubgroupTips.Clear();
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