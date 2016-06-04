using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using AGDSPresentationDB.AGDS;

namespace AGDSPresentationDB.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumericTextBoxPrevievInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string input = textBox.Text + e.Text;
                int value;
                if (int.TryParse(input, out value))
                {
                    if (value <= AGDSGraph.MaxSearchDepth && value >= AGDSGraph.MinSearchDepth)
                    {
                        e.Handled = false;
                        return;
                    }
                }
            }

            e.Handled = true;

        }
    }
}
