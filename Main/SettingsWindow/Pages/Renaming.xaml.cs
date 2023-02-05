using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;


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
            RenameList = new(Properties.Settings.Default.RenameList.Select(pair => new MutablePair<string, string>(pair)));
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



    /// <summary>
    /// Mutable version of KeyValuePair
    /// </summary>
    public class MutablePair<KeyType, ValueType>
    {
        public KeyType Key { get; set; }
        public ValueType Value { get; set; }
        public MutablePair(KeyValuePair<KeyType, ValueType> keyValuePair)
        {
            this.Key = keyValuePair.Key;
            this.Value = keyValuePair.Value;
        }
    }

}
