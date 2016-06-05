using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace DataExploration
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            TransactionDeserialzier deserializer = new TransactionDeserialzier('\t');
            deserializer.IgnoreFirstLine = true;
            List<Transaction> _transactionlist;
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Stream openStream;
                    if ((openStream = fileDialog.OpenFile()) != null)
                    {
                        using (openStream)
                        {
                            _transactionlist = deserializer.Deserialzie(openStream).ToList();
                            List<IExplorable> _explorableList = new List<IExplorable>(_transactionlist);
                            DataExplorer explorer = new DataExplorer(_explorableList);
                            Console.WriteLine(DataExplorer.PrintDictionary(explorer.GetMostFrequent(0.3)));
                            Console.Read();
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }           
            
        }
    }
}
