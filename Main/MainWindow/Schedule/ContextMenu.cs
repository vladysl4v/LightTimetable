using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;


namespace Timetable.Main
{
    public partial class MainWindow
    {
        private void InitializeContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();
            Schedule.ContextMenu = _contextMenu;

            MenuItem ItemRename = new MenuItem { Header = "Перейменувати пару" };
            ItemRename.Click += RenameLesson;

            _contextMenu.Items.Add(ItemRename);

            Schedule.BeginningEdit += CellBeginningEdit;
            Schedule.CellEditEnding += CellEditEnding;
        }

        private void RenameLesson(object s, EventArgs e)
        {
            Schedule.CurrentCell = (Properties.Settings.Default.ShowBlackouts) ? Schedule.SelectedCells[2] : Schedule.SelectedCells[1];
            Schedule.BeginEdit();
        }

        private void CellBeginningEdit(object? s, EventArgs e)
        {
            _cellBeforeEdit = ((Lesson)Schedule.SelectedItem).Discipline;
        }

        private void CellEditEnding(object? s, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                FillSchedule();
                return;
            }
                
            Dictionary<string, string> renames = Properties.Settings.Default.Renames;
            string newCellName = ((TextBox)e.EditingElement).Text;

            if (newCellName == _cellBeforeEdit || newCellName == string.Empty)
            {
                return;
            }
            if (renames.ContainsValue(_cellBeforeEdit))
            {
                _cellBeforeEdit = renames.FirstOrDefault(x => x.Value == _cellBeforeEdit).Key;
            }

            Properties.Settings.Default.AppendToRenames(_cellBeforeEdit, newCellName);

            FillSchedule();
        }
    }
}
