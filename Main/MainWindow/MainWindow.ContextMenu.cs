using System;
using System.Linq;
using System.Windows.Controls;


namespace Timetable.Main
{
    public partial class MainWindow
    {
        private void InitializeContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();
            tableSchedule.ContextMenu = _contextMenu;

            MenuItem ItemRename = new MenuItem { Header = "Перейменувати пару" };
            ItemRename.Click += RenameLesson;

            _contextMenu.Items.Add(ItemRename);

            tableSchedule.BeginningEdit += CellBeginningEdit;
            tableSchedule.CellEditEnding += CellEditEnding;
        }

        private void RenameLesson(object s, EventArgs e)
        {
            tableSchedule.CurrentCell = tableSchedule.SelectedCells[1];
            tableSchedule.BeginEdit();
        }

        private void CellBeginningEdit(object? s, EventArgs e)
        {
            _cellBeforeEdit = ((Lesson)tableSchedule.SelectedItem).Discipline;
        }

        private void CellEditEnding(object? s, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                FillTimetable();
                return;
            }
                
            var renameList = Properties.Settings.Default.RenameList;
            string newCellName = ((TextBox)e.EditingElement).Text;

            if (newCellName == _cellBeforeEdit || newCellName == string.Empty)
            {
                return;
            }
            if (renameList.ContainsValue(_cellBeforeEdit))
            {
                _cellBeforeEdit = renameList.FirstOrDefault(x => x.Value == _cellBeforeEdit).Key;
            }

            Properties.Settings.Default.AppendToRenameList(_cellBeforeEdit, newCellName);

            FillTimetable();
        }
    }
}
