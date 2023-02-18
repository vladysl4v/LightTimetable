using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Timetable.Utilities;

namespace Timetable.Settings.Pages
{
    /// <summary>
    /// Interaction logic for renaming page
    /// </summary>
    public partial class RenamePage : Page
    {
        public ObservableCollection<MutablePair<string, string>> RenameList { get; set; }
        public RenamePage()
        {
            RenameList = new(Properties.Settings.Default.Renames.Select(pair => new MutablePair<string, string>(pair)));
            InitializeComponent();
            DataContext = this;
        }

        
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (tableRenaming.SelectedItems.Count == 0)
                return;
            tableRenaming.CurrentCell = tableRenaming.SelectedCells[1];
            tableRenaming.BeginEdit();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (tableRenaming.SelectedItems.Count == 0)
                return;
            object selectedItem = tableRenaming.SelectedItem;
            var selectedItemName = ((MutablePair<string, string>)selectedItem);
            RenameList.Remove(selectedItemName);
        }
    }
}
