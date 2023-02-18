using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Timetable.Main
{
    /// Timetable logic in MainWindow
    public partial class MainWindow
    {
        public ObservableCollection<Lesson> ListLessons { get; set; } = new ObservableCollection<Lesson>();
        private readonly List<Label> _listSubgroupTips = new List<Label>();
        private string _cellBeforeEdit;

        public void FillSchedule()
        {
            SubGroupTipsClear();
            ListLessons.Clear();
            // Loop around the lessons whose date is today
            foreach (var lesson in UserData.Content.Where(lesson => lesson["full_date"] == UserData.Date.ToShortDateString()))
            {
                ListLessons.Add(new Lesson(lesson));
                if (lesson["study_subgroup"] != null)
                    AddSubgroupTip(lesson["study_subgroup"], ListLessons.Count);
            }
            DataContext = this;
            // Window height depending on the number of lessons / their miss     
            Height = ListLessons.Count > 0 ? (50 + 20 * ListLessons.Count) : 90;
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
            foreach (Label tip in _listSubgroupTips)
            {
                gridMain.Children.Remove(tip);
            }
            _listSubgroupTips.Clear();
        }
    }
}