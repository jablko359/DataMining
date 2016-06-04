using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class CrossValidationKnn
    {
        private readonly List<IClassificable> _allData;
        private readonly List<List<IClassificable>> _splittedSet;
        private readonly int _n;
        private readonly Dictionary<int, double> _scores = new Dictionary<int, double>();

        public CrossValidationKnn(List<IClassificable> inputData, int n)
        {
            _allData = inputData;
            _n = n;
            _splittedSet = new List<List<IClassificable>>();
            Normalize();
            Split(GetClasses());
            FillScores();        
        }
        private List<double> GetMaxValues()
        {
            List<double> maxValues = new List<double>(_allData[0].Dimensions);
            foreach (IClassificable classificable in _allData)
            {
                for (int i = 0; i < classificable.Dimensions.Length; i++)
                {
                    if (maxValues[i] < classificable.Dimensions[i])
                    {
                        maxValues[i] = classificable.Dimensions[i];
                    }
                }
            }
            return maxValues;
        }

        private List<double> GetMinValues()
        {
            List<double> minValues = new List<double>(_allData[0].Dimensions);
            foreach (IClassificable classificable in _allData)
            {
                for (int i = 0; i < classificable.Dimensions.Length; i++)
                {
                    if (minValues[i] > classificable.Dimensions[i])
                    {
                        minValues[i] = classificable.Dimensions[i];
                    }
                }
            }
            return minValues;
        }

        private void Normalize()
        {
            List<double> maxValues = GetMaxValues();
            List<double> minValues = GetMinValues();
            foreach (IClassificable classificable in _allData)
            {
                double[] tempArray = new double[classificable.Dimensions.Length];
                for (int i = 0; i < classificable.Dimensions.Length; i++)
                {
                    double tempValue = (classificable.Dimensions[i] - minValues[i]) / (maxValues[i] - minValues[i]);
                    tempArray[i] = tempValue;
                }
                classificable.Dimensions = tempArray;
            }
        }
        private Dictionary<string, List<IClassificable>> GetClasses()
        {
            Dictionary<string, List<IClassificable>> splittedClasses = new Dictionary<string, List<IClassificable>>();
            foreach (IClassificable classificable in _allData)
            {
                if (!splittedClasses.ContainsKey(classificable.Class))
                {
                    splittedClasses.Add(classificable.Class, new List<IClassificable>());
                }
                splittedClasses[classificable.Class].Add(classificable);
            }
            return splittedClasses;
        }

        private void Split(Dictionary<string, List<IClassificable>> classes)
        {
            //TODO: what to do if not divisible by _n?
            for (int i = 0; i < _n; i++)
            {
                _splittedSet.Add(new List<IClassificable>());
                foreach (List<IClassificable> classificableList in classes.Values)
                {
                    int elementCount = classificableList.Count;
                    int lowerBoundary = elementCount / _n * i;
                    int upperBoundary = elementCount / _n;
                    _splittedSet[i].AddRange(classificableList.GetRange(lowerBoundary, upperBoundary));
                }
            }
        }

        private double Validate(int k, List<IClassificable> learningSet, List<IClassificable> testSet)
        {
            int correctlyClassified = 0;
            NearestNeighboursAlgorithm algorithm = new NearestNeighboursAlgorithm(learningSet, k);
            foreach (IClassificable classificable in testSet)
            {
                string foundClass = algorithm.Classify(classificable);
                if (foundClass == classificable.Class)
                {
                    correctlyClassified++;
                }
            }
            return (double)correctlyClassified / (double)testSet.Count;
        }

        private double ValidateK(int k)
        {
            double correctness = 0;
            for (int i = 0; i < _splittedSet.Count; i++)
            {
                List<IClassificable> testSet = _splittedSet[i];
                var rest = _splittedSet.Where(item => item != testSet);
                List<IClassificable> learningSet = new List<IClassificable>();
                foreach(List<IClassificable> classificables in rest)
                {
                    learningSet.AddRange(classificables);
                }
                correctness += Validate(k: k, testSet: testSet, learningSet: learningSet);
            }
            return correctness/_n;
        }

        private void FillScores()
        {
            int maxK = _allData.Count - (_allData.Count / _n);
            for(int k = 1; k < maxK; k++)
            {
                _scores.Add(k, ValidateK(k));
            }
        }

        public Tuple<int, double> GetBestCoefficient()
        {            
            Tuple<int, double> coefficientValue = new Tuple<int, double>(0,0);
            foreach(KeyValuePair<int,double> pair in _scores)
            {
                if(coefficientValue.Item2 < pair.Value)
                {
                    coefficientValue = new Tuple<int, double>(pair.Key, pair.Value);                    
                }
            }
            return coefficientValue;
        }

        //public NearestNeighboursAlgorithm FindBest()
        //{
            
        //}

    }
}
