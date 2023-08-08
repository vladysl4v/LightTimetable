using System.Linq;
using System.Windows;
using System.Diagnostics;

using LightTimetable.Models;
using LightTimetable.Properties;
using LightTimetable.Views.Utilities;


namespace LightTimetable.ViewModels.Commands
{
    public class ContextMenuCommandHandler
    {
        private readonly IUserSettings _settings;
        public ContextMenuCommandHandler(IUserSettings settings)
        {
            _settings = settings;
        }
        public void OpenInTeams(DataItem? selectedItem)
        {
            if (selectedItem?.Events == null)
                return;

            if (selectedItem.Events.Count == 1)
            {
                var outlookEvent = selectedItem.Events.Single();
                var message =
                    $"Ви впевнені, що хочете зайти на нараду \"{outlookEvent.Name.Trim()}\"?\nВона буде відкрита в Microsoft Teams.";
                var mbResult = MessageBox.Show(message, selectedItem.Discipline.Modified, MessageBoxButton.YesNo);
                if (mbResult == MessageBoxResult.Yes)
                {
                    OpenLinkInBrowser(outlookEvent.Link);
                }
            }

            if (selectedItem.Events.Count > 1)
            {
                var selectedEvent = EventPicker.Show(selectedItem.Discipline.Modified, selectedItem.Events);
                if (selectedEvent != null)
                {
                    OpenLinkInBrowser(selectedEvent.Link);
                }
            }
        }

        public void AddNote(DataItem? selectedItem)
        {
            if (selectedItem == null)
                return;
            string noteText = InputBox.Show("Нотатка", "Введіть текст нотатки:");
            if (string.IsNullOrWhiteSpace(noteText))
                return;

            _settings.Notes[selectedItem.Id] = noteText;
            _settings.Save();

            selectedItem.Note = noteText;
        }

        public void ChangeNote(DataItem? selectedItem)
        {
            if (selectedItem == null)
                return;

            string noteText = InputBox.Show("Нотатка", "Введіть новий текст нотатки:", selectedItem.Note);
            if (string.IsNullOrWhiteSpace(noteText) || noteText == selectedItem.Note)
                return;

            _settings.Notes[selectedItem.Id] = noteText;
            _settings.Save();

            selectedItem.Note = noteText;
        }

        public void DeleteNote(DataItem? selectedItem)
        {
            if (selectedItem == null)
                return;
            var result = MessageBox.Show("Ви впевнені що хочете видалити цю нотатку?", "Нотатка", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
                return;

            _settings.Notes.Remove(selectedItem.Id);
            _settings.Save();

            selectedItem.Note = string.Empty;
        }

        public void RenameItem(DataItem? selectedItem)
        {
            if (selectedItem == null)
                return;
            string newItemName = InputBox.Show("Перейменування", $"Введіть нову назву для \"{selectedItem.Discipline.Original}\":", selectedItem.Discipline.Modified);
            if (string.IsNullOrWhiteSpace(newItemName) || newItemName == selectedItem.Discipline.Modified)
                return;

            _settings.Renames[selectedItem.Discipline.Original] = newItemName;
            _settings.Save();
        }

        private void OpenLinkInBrowser(string link)
        {
            link = link.Replace("&", "^&");
            Process.Start(new ProcessStartInfo(link)
            {
                UseShellExecute = true
            });
        }
    }
}
