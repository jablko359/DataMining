using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public interface IClassificable
    {
        string Class { get; set; }        
        double[] Dimensions { get; set; }
    }

    public class SkipPropertyAttribute : Attribute
    {
    }
}
