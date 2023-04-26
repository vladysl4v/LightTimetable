using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LightTimetable.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class ItemData
    {
        public string Name { get; set; }
        public string Time { get; set; }

        public ItemData(string name, string time)
        {
            Name = name;
            Time = time;
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var abc = new List<ItemData>
            {
                new ItemData("AHAHAHAHAYH", "17:30-22:14"),
                new ItemData("Proebali", "12:00-14:00"),
                new ItemData("Proehalo pro mozhet igrat chem", "12:35-12:55")
            };
            Title = "Дискретна математика";
            foreach (var a in abc)
            {
                EventPickerGrid.Items.Add(a);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((ItemData)EventPickerGrid.SelectedItem).Name, "TITLE");
        }
    }
}
