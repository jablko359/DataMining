using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataExploration
{
    public class TransactionDeserialzier
    {
        private readonly char _separator;
        private bool _ignoreFirstLine;

        public bool IgnoreFirstLine
        {
            get { return _ignoreFirstLine; }
            set { _ignoreFirstLine = value; }
        }

        public TransactionDeserialzier(char separator)
        {
            _separator = separator;
        }

        public IList<Transaction> Deserialzie(Stream file)
        {
            string[] lines = null;
            IList<Transaction> transactionList = new List<Transaction>();
            using (TextReader reader = new StreamReader(file))
            {
                lines = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            }
            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                if(_ignoreFirstLine && lineNumber == 0)
                {
                    continue;
                }
                string line = lines[lineNumber];
                string[] rows = line.Split(new char[] { _separator });
                rows = rows.Where(value => !string.IsNullOrWhiteSpace(value)).ToArray();
                long id = Int64.Parse(rows[0]);
                List<string> transactionElements = rows.ToList();
                transactionElements.RemoveAt(0);
                Transaction transaction = new Transaction(id, transactionElements);
                transactionList.Add(transaction);
            }
            return transactionList;

        }
    }
}
