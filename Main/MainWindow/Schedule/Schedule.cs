using System;
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
        private readonly List<Label> _informationLabels = new List<Label>();

        public void FillSchedule()
        {
            SubGroupTipsClear();
            ListLessons.Clear();

            DateTime currentDate = UserData.Date;

            if (UserData.Content.TryGetValue(currentDate, out List<Lesson> currentDateLessons))
            {
                // Window height depending on the number of lessons / their miss   
                Height = currentDateLessons.Count > 0 ? (50 + 20 * currentDateLessons.Count) : 90;
                foreach (var lesson in currentDateLessons)
                    ListLessons.Add(lesson);

                foreach (var lesson in UserData.Content[currentDate].Where(lesson => lesson.Subgroup.Any()))
                {
                   AddSubgroupTip(lesson);
                }

                foreach (var lesson in UserData.Content[currentDate].Where(lesson => lesson.Note.Any()))
                {
                    AddNote(lesson);
                }
            }    
            DataContext = this;  
            
        }

        private void AddSubgroupTip(Lesson lesson)
        {
            int margin = UserData.Content[UserData.Date].IndexOf(lesson);
            Label SubgroupTip = new Label
            {
                Content = FindResource("SubgroupIcon"),
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness { Top = 5 + (20 * (margin + 1)), Left = -10 },
                ToolTip = lesson.Subgroup,
                Tag = lesson.ID
            };
            Grid.SetColumn(SubgroupTip, 4);
            Grid.SetRow(SubgroupTip, 1);
            ToolTipService.SetInitialShowDelay(SubgroupTip, 200);
            _informationLabels.Add(SubgroupTip);
            gridMain.Children.Add(SubgroupTip);
        }

        private void AddNote(Lesson lesson)
        {
            int margin = UserData.Content[UserData.Date].IndexOf(lesson);
            Label Note = new Label
            {
                Content = FindResource("NotesIcon"),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness { Top = 5 + (20 * (margin + 1)), Left = -10 },
                ToolTip = lesson.Note,
                Tag = lesson.ID
            };
            // if this lesson already has an infolabel, insert a note on another row
            if ((from infolabel in _informationLabels where (uint)infolabel.Tag == lesson.ID select infolabel).Any())
            {
                Grid.SetColumn(Note, 3);
            }
            else
            {
                Grid.SetColumn(Note, 4);
            }

            Grid.SetRow(Note, 1);
            ToolTipService.SetInitialShowDelay(Note, 200);
            _informationLabels.Add(Note);
            gridMain.Children.Add(Note);
        }


        private void SubGroupTipsClear()
        {
            foreach (Label tip in _informationLabels)
            {
                gridMain.Children.Remove(tip);
            }
            _informationLabels.Clear();
        }
    }
}