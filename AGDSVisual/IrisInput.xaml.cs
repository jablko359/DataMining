using ConsoleApplication1;
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

namespace AGDSVisual
{
    /// <summary>
    /// Interaction logic for IrisInput.xaml
    /// </summary>
    public partial class IrisInput : UserControl
    {

        public event EventHandler IrisChanged;

        public Iris IrisValue
        {
            get { return (Iris)GetValue(IrisValueProperty); }
            set { SetValue(IrisValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IrisValueProperty =
            DependencyProperty.Register("IrisValue", typeof(Iris), typeof(IrisInput), new PropertyMetadata(new Iris()));
        

        public IrisInput()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double leafLenght;
            double.TryParse(LeafLength.Text, out leafLenght);
            IrisValue.LeafLength = leafLenght;
            LeafLength.Text = leafLenght.ToString();

            double leafWidth;
            double.TryParse(LeafWidth.Text, out leafWidth);
            IrisValue.LeafWidth = leafWidth;
            LeafWidth.Text = leafWidth.ToString();

            double petalLenght;
            double.TryParse(PetalLength.Text, out petalLenght);
            IrisValue.PetalLength = petalLenght;
            PetalLength.Text = petalLenght.ToString();

            double petalWidth;
            double.TryParse(PetalWidth.Text, out petalWidth);
            IrisValue.PetalWidth = petalWidth;
            PetalWidth.Text = petalWidth.ToString();

            if (IrisChanged != null)
            {
                IrisChanged(this, null);
            }
        }
    }    
}
