using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.FW
{
    public class DirectedCut : Cut
    {
        public DirectedCut(int n, double[] modular, double[][] weight)
            : base(n, modular, weight)
        {
        }

        public DirectedCut(string path)
            : base(path)
        {
        }
    }
}
