using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.FW
{
    public class UndirectedCut : Cut
    {
        public UndirectedCut(int n, double[] modular, double[][] weight)
            : base(n, modular, weight)
        {
        }

        public UndirectedCut(string path)
            : base(path)
        {
        }
    }
}
