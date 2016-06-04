using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SOM demo");
            try
            {
                string filePath;
                if (args.Length != 1)
                {
                    Console.WriteLine("Please enter input file path");
                    filePath = Console.ReadLine().Trim();
                }
                else
                {
                    filePath = args[0];
                }
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File {0} doesn'r exists", filePath);
                }
                InputDataSetDeserializator deserializator = new InputDataSetDeserializator(filePath);
                Console.WriteLine("Deserializing input");
                var t = deserializator.Deserialize();
                Console.WriteLine("Enter x size of SOM:");
                int x = Int32.Parse(Console.ReadLine());
                Console.WriteLine("Enter y size of SOM:");
                int y = Int32.Parse(Console.ReadLine());
                Console.WriteLine("Enter narrowing:");
                double narrowing = Double.Parse(Console.ReadLine());
                Console.WriteLine("Enter desired precision");
                double precision = Double.Parse(Console.ReadLine());
                Console.WriteLine("Creating map");
                SelfOrganizedMap map = new SelfOrganizedMap(x, y, narrowing, precision);
                map.BuildMap(t, deserializator.AtributesCount);
                string userInput;
                do
                {
                    Console.WriteLine(@"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                    Console.WriteLine("To show map node press 1");
                    Console.WriteLine("To enter element and find closest node press 2");
                    Console.WriteLine("To exit press q");
                    userInput = Console.ReadLine();
                    if (userInput == "1")
                    {
                        Console.WriteLine("x:");
                        int xpos = Int32.Parse(Console.ReadLine());
                        Console.WriteLine("y:");
                        int ypos = Int32.Parse(Console.ReadLine());
                        Node node = map.GetNode(xpos, ypos);
                        if (node != null)
                        {
                            Console.WriteLine(node);
                        }
                        else
                        {
                            Console.WriteLine("No such node");
                        }
                    }
                    else if (userInput == "2")
                    {
                        List<double> inputVector = new List<double>(deserializator.AtributesCount);
                        for (int indx = 0; indx < deserializator.AtributesCount; indx++)
                        {
                            Console.WriteLine("Enter attribute number {0}", indx);
                            double attribute = Double.Parse(Console.ReadLine());
                            inputVector.Add(attribute);
                        }
                        InputVector vector = new InputVector(deserializator.AttributesList,inputVector);
                        Node nd = map.FindNearestNode(map.CountWeights(vector));
                        if (nd != null)
                        {
                            Console.WriteLine(nd);
                        }
                        else
                        {
                            Console.WriteLine("Node not found");
                        }
                    }
                }
                while (userInput != "q");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }


        }
    }
}
