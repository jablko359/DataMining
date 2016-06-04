using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM
{
    public class SelfOrganizedMap
    {
        private readonly int _xSize;
        private readonly int _ySize;
        private readonly Node[,] _nodesMatrix;
        private readonly double _narrowingConstant;
        private readonly double _desiredPrecision;
        private int _currentDistance;
        private int _currentIteration = 1;
        private double _weightFactor = 1;
        private Node _currentNearstNode;
        private Dictionary<InputVector, Node> _vectorToNodes = new Dictionary<InputVector, Node>();

        public SelfOrganizedMap(int x, int y, double narrowingConstant, double precision, double nodeRandomScale = 1)
        {
            _xSize = x;
            _ySize = y;
            _nodesMatrix = new Node[x, y];
            _narrowingConstant = narrowingConstant;
            _desiredPrecision = precision;
            //take larger
            _currentDistance = _xSize > _ySize ? _xSize : _ySize;
            //ToDo needs to be modified
            Node.Scale = nodeRandomScale;
        }

        public void BuildMap(List<InputVector> vectors, int atributesCount)
        {
            for (int i = 0; i < _xSize; i++)
            {
                for (int j = 0; j < _ySize; j++)
                {
                    _nodesMatrix[i, j] = new Node(atributesCount, i, j);
                }
            }
            int iteration = 0;
            while (!CheckStopCondition())
            {
                Console.WriteLine("Iteration {0}", iteration);
                foreach (InputVector vector in vectors)
                {
                    var weights = CountWeights(vector);
                    _currentNearstNode = FindNearestNode(weights);
                    if (!_vectorToNodes.ContainsKey(vector))
                    {
                        _vectorToNodes.Add(vector, _currentNearstNode);
                    }
                    else
                    {
                        _vectorToNodes[vector] = _currentNearstNode;
                    }
                    UpdateNeighborhood(_currentNearstNode, vector);
                }
                UpdateCurrentDistance();
                UpdateWeightFactor();
                iteration++;
            }
        }

        public List<Tuple<double, Node>> CountWeights(InputVector vector)
        {
            List<Tuple<double, Node>> nodeDistanceList = new List<Tuple<double, Node>>();
            for (int i = 0; i < _xSize; i++)
            {
                for (int j = 0; j < _ySize; j++)
                {
                    Node mapNode = _nodesMatrix[i, j];
                    double distance = 0;
                    for (int k = 0; k < mapNode.Weights.Count; k++)
                    {
                        distance += Math.Pow(mapNode.Weights[k] - vector.Values[k], 2);
                    }
                    distance = Math.Sqrt(distance);
                    nodeDistanceList.Add(new Tuple<double, Node>(distance, mapNode));
                }
            }
            return nodeDistanceList;
        }

        public Node FindNearestNode(List<Tuple<double, Node>> nodeDistanceList)
        {
            double minDistance = Double.MaxValue;
            Node nearestNode = null;
            foreach (Tuple<double, Node> tuple in nodeDistanceList)
            {
                if (tuple.Item1 <= minDistance)
                {
                    minDistance = tuple.Item1;
                    nearestNode = tuple.Item2;
                }
            }
            return nearestNode;
        }

        private List<Node> GetNodeNeighborhood(Node startNode, int distance)
        {
            List<Node> neighbors = new List<Node>();
            int xStart = (startNode.XPos - distance) > 0 ? startNode.XPos - distance : 0;
            int yStart = (startNode.Ypos - distance) > 0 ? startNode.Ypos - distance : 0;
            int xStop = (startNode.XPos + distance) < _nodesMatrix.GetLength(0) ? startNode.XPos + distance : _nodesMatrix.GetLength(0) - 1;
            int yStop = (startNode.Ypos + distance) < _nodesMatrix.GetLength(1) ? startNode.Ypos + distance : _nodesMatrix.GetLength(1) - 1;
            for (int xIdx = xStart; xIdx <= xStop; xIdx++)
            {
                for (int yIdx = yStart; yIdx < yStop; yIdx++)
                {
                    neighbors.Add(_nodesMatrix[xIdx, yIdx]);
                }
            }
            return neighbors;
        }

        private void UpdateCurrentDistance()
        {
            _currentDistance = (int)Math.Ceiling(_currentDistance * Math.Pow(Math.E, -(_currentIteration / _narrowingConstant)));
        }

        private void UpdateWeightFactor()
        {
            _weightFactor = _weightFactor * Math.Pow(Math.E, -(_currentIteration / _narrowingConstant));
        }

        private double GetDistanceFactor(Node node)
        {
            return Math.Pow(Math.E, -Math.Pow(GetNodeDistance(node, _currentNearstNode), 2));
        }

        private double GetNodeDistance(Node node1, Node node2)
        {
            return Math.Abs(node1.XPos - node2.XPos) + Math.Abs(node1.Ypos - node2.Ypos);
        }

        private void UpdateNeighborhood(Node startNode, InputVector vector)
        {
            List<Node> neighbors = GetNodeNeighborhood(startNode, _currentDistance);
            foreach (Node neighbor in neighbors)
            {
                for (int i = 0; i < neighbor.Weights.Count; i++)
                {
                    neighbor.Weights[i] = neighbor.Weights[i] + GetDistanceFactor(neighbor) * _weightFactor * (vector.Values[i] - neighbor.Weights[i]);
                }
            }
        }

        private bool CheckStopCondition()
        {
            if (_vectorToNodes.Count == 0)
            {
                return false;
            }
            foreach (KeyValuePair<InputVector, Node> node in _vectorToNodes)
            {
                if (GetDistance(node.Key, node.Value) > _desiredPrecision)
                {
                    return false;
                }
            }
            return true;
        }

        private double GetDistance(InputVector vector, Node node)
        {
            double distance = 0;
            for (int k = 0; k < node.Weights.Count; k++)
            {
                distance += Math.Pow(node.Weights[k] - vector.Values[k], 2);
            }
            distance = Math.Sqrt(distance);
            return distance;
        }

        public Node GetNode(int x, int y)
        {
            if (x >= 0 && x < _nodesMatrix.GetLength(0) && y >= 0 && y < _nodesMatrix.GetLength(1))
            {
                return _nodesMatrix[x, y];
            }
            return null;

        }
    }
}
