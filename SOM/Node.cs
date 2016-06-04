using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM
{
    public class Node
    {
        private List<double> _weights;
        private static Random WeightGeneratoRandom = new Random();

        public static double Scale = 1;

        public List<double> Weights
        {
            get { return _weights; }
        }

        private int _xPos;

        public int XPos
        {
            get { return _xPos; }
        }

        private int _yPos;

        public int Ypos
        {
            get { return _yPos; }
        }



        public Node(int size, int xpos, int ypos)
        {
            _yPos = ypos;
            _xPos = xpos;
            
            _weights = new List<double>(size);
            for (int i = 0; i < size; i++)
            {
                _weights.Add(WeightGeneratoRandom.NextDouble() * Scale);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (double weight in _weights)
            {
                sb.Append(weight);
                sb.Append('\t');
            }
            return sb.ToString();
        }
    }
}
