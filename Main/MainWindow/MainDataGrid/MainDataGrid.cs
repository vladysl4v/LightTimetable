using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Timetable.Utilities;


namespace Timetable.Main
{
    /// Timetable logic in MainWindow
    public partial class MainWindow
    {
        public ObservableCollection<Lesson> ListLessons { get; set; } = new ObservableCollection<Lesson>();
        private readonly List<Label> _listSubgroupTips = new List<Label>();
        private string _cellBeforeEdit;

        public void FillTimetable()
        {
            SubGroupTipsClear();
            ListLessons.Clear();
            foreach (var lesson in UserData.Content.Where(lesson => lesson["full_date"] == UserData.Date.ToShortDateString()))
            {
                //UserData.Date.DayOfWeek;
                ElectricityStatus? elecInfo = (Properties.Settings.Default.ShowDTEK) ? CalculateLightInfo(lesson["study_time"]) : null;
                ListLessons.Add(
                    new Lesson()
                    {
                        Electricity = elecInfo,
                        Study_time = lesson["study_time"],
                        Discipline = DisciplineRenamer(lesson["discipline"]),
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

        private string DisciplineRenamer(string discipline)
        {
            if (Properties.Settings.Default.RenameList.ContainsKey(discipline))
            {
                return Properties.Settings.Default.RenameList[discipline];
            }
            else
            {
                return discipline;
            }
        }
        private string EmployeeShortener(string employee)
        {
            if (employee != default)
            {
                string[] EmplSplitted = employee.Split();
                return $"{EmplSplitted[0]} {EmplSplitted[1][0]}.{EmplSplitted[2][0]}.";
            }
            else
            {
                return string.Empty;
            }
        }

        private void AddSubgroupTip(string text, int margin)
        {
            Label SubgroupTip = new Label
            {
                Content = FindResource("SubgroupIcon"),
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness { Top = 5 + (20 * margin) },
                ToolTip = text,
            };
            Grid.SetColumn(SubgroupTip, 3);
            Grid.SetRow(SubgroupTip, 1);
            ToolTipService.SetInitialShowDelay(SubgroupTip, 200);
            _listSubgroupTips.Add(SubgroupTip);
            gridMain.Children.Add(SubgroupTip);
        }

        private void SubGroupTipsClear()
        {
            foreach (var tip in _listSubgroupTips)
            {
                gridMain.Children.Remove(tip);
            }
            _listSubgroupTips.Clear();
        }
    }
}