using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class NearestNeighboursAlgorithm
    {
        private int _kNearests;
        private int _dimensionsCount;
        private readonly IList<IClassificable> _classificableList;

        public int KNearest
        {
            set { _kNearests = value; }
        }

        public NearestNeighboursAlgorithm(IList<IClassificable> classificableList, int kNearests)
        {
            _dimensionsCount = classificableList[0].Dimensions.Length;
            foreach (IClassificable classificable in classificableList)
            {
                if (_dimensionsCount != classificable.Dimensions.Length)
                {
                    throw new InvalidOperationException();
                }
            }
            _classificableList = classificableList;
            _kNearests = kNearests;
        }

        public string Classify(IClassificable item)
        {
            if (item.Dimensions.Length != _dimensionsCount)
            {
                throw new InvalidOperationException();
            }
            List<ClassificableWrapper> lenghts = new List<ClassificableWrapper>();
            foreach (IClassificable classificable in _classificableList)
            {
                double lenght = 0;
                for (int i = 0; i < _dimensionsCount; i++)
                {
                    lenght += item.Dimensions[i] - classificable.Dimensions[i];
                }
                lenght = Math.Pow(lenght, 2);
                ClassificableWrapper wrapper = new ClassificableWrapper(lenght, classificable);
                lenghts.Add(wrapper);
            }
            lenghts.Sort();
            Dictionary<string, int> nearestsTypes = new Dictionary<string, int>();
            int loopLimit = _kNearests > lenghts.Count ? _kNearests : lenghts.Count;
            for (int i = 0; i < _kNearests; i++)
            {
                string currentKey = lenghts[i].Classificable.Class;
                if (nearestsTypes.ContainsKey(currentKey))
                {
                    nearestsTypes[currentKey]++;
                }
                else
                {
                    nearestsTypes.Add(currentKey, 1);
                }
            }
            string currentType = nearestsTypes.OrderByDescending(value => value.Value).First().Key;            
            return currentType;

        }

        
    }
    public class ClassificableWrapper : IComparable<ClassificableWrapper>
    {
        private double _lenght;
        private IClassificable _classificable;

        public IClassificable Classificable
        {
            get { return _classificable; }
        }

        public ClassificableWrapper(double lenght, IClassificable IClassificable)
        {
            _lenght = lenght;
            _classificable = IClassificable;
        }

        public int CompareTo(ClassificableWrapper other)
        {
            return (int)(_lenght - other._lenght);
        }
    }
}
