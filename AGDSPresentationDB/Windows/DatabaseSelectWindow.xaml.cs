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
using System.Windows.Shapes;
using AGDSPresentationDB.Services;

namespace AGDSPresentationDB.Windows
{
    /// <summary>
    /// Interaction logic for DatabaseSelectWindow.xaml
    /// </summary>
    public partial class DatabaseSelectWindow : Window , IDialogWindow
    {
        public DatabaseSelectWindow()
        {
            InitializeComponent();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
