﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication1
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
                        List<IClassificable> classificable = new List<IClassificable>();
                        List<IClassificable> trainingSet = new List<IClassificable>();
                        foreach (Iris iris in irises)
                        {
                            classificable.Add(iris);
                            Iris trainingIris = new Iris
                            {
                                LeafLength = iris.LeafLength,
                                LeafWidth = iris.LeafWidth,
                                PetalLength = iris.PetalLength,
                                PetalWidth = iris.PetalWidth,
                                Class = iris.Class
                            };
                            trainingSet.Add(trainingIris);
                        }
                        
                        CrossValidationKnn valdiation = new CrossValidationKnn(classificable, 10);
                        var t = valdiation.GetBestCoefficient();
                        Console.WriteLine(string.Format("Best found k is: {0}, effectivness: {1}", t.Item1, t.Item2));
                        NearestNeighboursAlgorithm knn = new NearestNeighboursAlgorithm(trainingSet, t.Item1);
                        Iris searchIris = new Iris();
                        Console.WriteLine("LeafLength:");
                        double leafLength = double.Parse(Console.ReadLine());
                        searchIris.LeafLength = leafLength;
                        Console.WriteLine("LeafWidth:");
                        searchIris.LeafWidth = double.Parse(Console.ReadLine());
                        Console.WriteLine("PetalLength:");
                        searchIris.PetalLength = double.Parse(Console.ReadLine());
                        Console.WriteLine("PetalWidth:");
                        searchIris.PetalWidth = double.Parse(Console.ReadLine());
                        string cls = knn.Classify(searchIris);
                        Console.WriteLine("Iris class is {0}", cls);
                        Console.Read();

                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            //string filePath = "IrisDataAll.txt";
            //using (Stream stream = new FileStream(filePath,FileMode.Open))
            //{
            //    CSVDeserializer.CSVSerializer<Iris> serializer = new CSVDeserializer.CSVSerializer<Iris>()
            //    {
            //        Separator = '\t'
            //    };
            //    IList<Iris> irises = serializer.Deserialize(stream);
            //    List<IClassificable> irisClassificable = new List<IClassificable>();
            //    foreach(IClassificable iris in irises)
            //    {
            //        irisClassificable.Add(iris);
            //    }
            //    NearestNeighboursAlgorithm alg = new NearestNeighboursAlgorithm(irisClassificable, 20);                
            //    Iris test = new Iris()
            //    {
            //        LeafLength = 2.4,
            //        LeafWidth = 1,
            //        PetalLength = 5,
            //        PetalWidth = 4
            //    };
            //    alg.Classify(test);
            //    Console.WriteLine(test);
            //    Console.ReadLine();
            //}
        }
    }
}
