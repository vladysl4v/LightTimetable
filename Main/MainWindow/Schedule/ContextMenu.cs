using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using Timetable.Utilities;

namespace Timetable.Main
{
    public partial class MainWindow
    {
        private ContextMenu _contextMenu;
        private Lesson _selectedItem;
        private string _cellBeforeEdit;
        private void InitializeContextMenu()
        {
            _contextMenu = new ContextMenu();
            Schedule.ContextMenu = _contextMenu;
            Schedule.ContextMenuOpening += ChangeContextMenuItems;

            Schedule.BeginningEdit += CellBeginningEdit;
            Schedule.CellEditEnding += CellEditEnding;
        }
        private void ChangeContextMenuItems(object s, EventArgs e)
        {
            _selectedItem = ((Lesson)Schedule.SelectedItem);
            _contextMenu.Items.Clear();
            if (_selectedItem.Note.Any())
            {
                MenuItem ItemChangeNote = new MenuItem { Header = "Змiнити нотатку" };
                ItemChangeNote.Click += ChangeNote;
                _contextMenu.Items.Add(ItemChangeNote);

                MenuItem ItemDeleteNote = new MenuItem { Header = "Видалити нотатку" };
                ItemDeleteNote.Click += DeleteNote;
                _contextMenu.Items.Add(ItemDeleteNote);
            }
            else
            {
                MenuItem ItemAddNote = new MenuItem { Header = "Додати нотатку" };
                ItemAddNote.Click += AddNote;
                _contextMenu.Items.Add(ItemAddNote);
            }

            MenuItem ItemRename = new MenuItem { Header = "Перейменувати пару" };
            ItemRename.Click += RenameLesson;
            _contextMenu.Items.Add(ItemRename);
        }

        private void AddNote(object s, EventArgs e)
        {
            string noteText = new InputBox("Нотатка", "Введiть текст нотатки:").GetText();
            if (noteText.Any())
            {
                Properties.Settings.Default.AppendToNotes(_selectedItem.ID, noteText);
                UserData.RestructurizeContent();
                FillSchedule();
            }
        }
        private void DeleteNote(object s, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Ви впевненi що хочете видалити цю нотатку?", "Нотатка", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Properties.Settings.Default.RemoveFromNotes(_selectedItem.ID);
                UserData.RestructurizeContent();
                FillSchedule();
            }
        }
        private void ChangeNote(object s, EventArgs e)
        {
            string noteText = new InputBox("Нотатка", "Введiть новий текст нотатки:", _selectedItem.Note).GetText();
            if (noteText.Any() || noteText != _selectedItem.Note)
            {
                Properties.Settings.Default.AppendToNotes(_selectedItem.ID, noteText);
                UserData.RestructurizeContent();
                FillSchedule();
            }
        }

        private void RenameLesson(object s, EventArgs e)
        {
            Schedule.CurrentCell = Schedule.SelectedCells[2];
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
            UserData.RestructurizeContent();
            FillSchedule();
        }
    }
}
