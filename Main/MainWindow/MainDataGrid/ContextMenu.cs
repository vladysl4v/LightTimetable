using System;
using System.Linq;
using System.Windows.Controls;

using Timetable.Utilities;

namespace Timetable.Main
{
    public partial class MainWindow
    {
        private void InitializeContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();
            MainDataGrid.ContextMenu = _contextMenu;

            MenuItem ItemRename = new MenuItem { Header = "Перейменувати пару" };
            ItemRename.Click += RenameLesson;

            _contextMenu.Items.Add(ItemRename);

            MainDataGrid.BeginningEdit += CellBeginningEdit;
            MainDataGrid.CellEditEnding += CellEditEnding;
        }

        private void RenameLesson(object s, EventArgs e)
        {
            MainDataGrid.CurrentCell = MainDataGrid.SelectedCells[1];
            MainDataGrid.BeginEdit();
        }

        private void CellBeginningEdit(object? s, EventArgs e)
        {
            _cellBeforeEdit = ((Lesson)MainDataGrid.SelectedItem).Discipline;
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
