using CSVDeserializer;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class Iris : IClassificable
    {
        private double _leafLength;
        private double _leafWidth;
        private double _petalLength;
        private double _petalWidth;
        private IrisType _type;


        public string Class
        {
            get { return _type.ToString(); }
            set
            {
                _type = (IrisType)Enum.Parse(typeof(IrisType), value, true);
            }
        }

        public double LeafLength
        {
            get { return _leafLength; }
            set { _leafLength = value; }
        }

        public double LeafWidth
        {
            get { return _leafWidth; }
            set { _leafWidth = value; }
        }

        public double PetalLength
        {
            get { return _petalLength; }
            set { _petalLength = value; }
        }

        public double PetalWidth
        {
            get { return _petalWidth; }
            set { _petalWidth = value; }
        }

        [CsvIgnore]
        [SkipPropertyAttribute]
        public double[] Dimensions
        {
            get { return new double[] { _leafLength, _leafWidth, _petalLength, _petalWidth }; }
            set
            {
                _leafLength = value[0];
                _leafWidth = value[1];
                _petalLength = value[2];
                _petalWidth = value[3];
            }
        }

        public Iris()
        {
        }

        public override string ToString()
        {
            return string.Format("LeafLength: {0}; LeafWidth: {1}; PetalLength: {2}; PetalWidth {3}; Type: {4}", _leafLength, _leafWidth, _petalLength, _petalWidth, _type.ToString());
        }
    }
}
