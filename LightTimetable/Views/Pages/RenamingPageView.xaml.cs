using System;
using System.Collections.Generic;
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

namespace LightTimetable.Views.Pages
{
    /// <summary>
    /// Interaction logic for RenamingPageView.xaml
    /// </summary>
    public partial class RenamingPageView : UserControl
    {
        public RenamingPageView()
        {
            InitializeComponent();
        }

        private void ItemsSourceUpdated(object sender, EventArgs eventArgs)
        {
            CollectionViewSource.GetDefaultView(((DataGrid)sender).ItemsSource).Refresh();
        }
    }
}
