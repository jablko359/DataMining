using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace AGDS
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CSVDeserializer.CSVSerializer<Iris> serializer = new CSVDeserializer.CSVSerializer<Iris>()
            {
                Separator = '\t'
            };
            IList<Iris> irises;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Stream openStream;
                    if ((openStream = openFileDialog.OpenFile()) != null)
                    {
                        irises = serializer.Deserialize(stream: openStream);                        
                        AGDSBuilder builder = new AGDSBuilder(typeof(Iris));
                        AGDSGraph graph = builder.BuildGraph(new List<object>(irises));
                        ListFinder<Iris> irisFinder = new ListFinder<Iris>(new List<Iris>(irises));
                        Iris test = new Iris()
                        {
                            LeafLength = 5.1,
                            LeafWidth = 3.5,
                            PetalLength = 1.4,
                            PetalWidth = 0.2
                        };
                        Stopwatch testStopWatch = Stopwatch.StartNew();
                        var resultList = irisFinder.FindLike(test,0.5);
                        testStopWatch.Stop();
                        Console.WriteLine(string.Format("List find {0}",testStopWatch.Elapsed));
                        testStopWatch = Stopwatch.StartNew();
                        var resultGraph = graph.FindLike(test, 0.5);
                        testStopWatch.Stop();
                        Console.WriteLine(string.Format("Graph find {0}", testStopWatch.Elapsed));
                        //Stopwatch watch = Stopwatch.StartNew(); 
                        //List<object> items = graph.FindLike(new Iris()
                        //{
                        //    Class = "Setosa",
                        //    LeafLength = 5.1,
                        //    LeafWidth = 3.5,
                        //    PetalLength = 1.4,
                        //    PetalWidth = 0.2                           

                        //}, 1);
                        //foreach(object item in items)
                        //{
                        //    Console.WriteLine(item);
                        //}
                        Console.ReadLine();
                    }
                }
                catch (IOException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
    }
}
