using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExploration
{
    public class Transaction : IExplorable
    {
        private readonly List<string> _transactionElements;
        private readonly long _id;

        public List<string> Elemnets
        {
            get { return _transactionElements; }
        }

        public Transaction(long id, IEnumerable<string> elements)
        {
            _transactionElements = elements.ToList();
            _id = id;
        }

    }

    public interface IExplorable
    {
        List<string> Elemnets { get; }
    }

}
