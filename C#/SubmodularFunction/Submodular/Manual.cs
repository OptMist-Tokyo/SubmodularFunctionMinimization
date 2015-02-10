using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
namespace Onigiri.Submodular
{
    public class Manual:SubmodularOracle
    {
        double[] values;

        public Manual(int n,double []array)
        {
            SetVariable(n, array[0]);
            values = array;;
        }

        
        internal override double Value(int[] order, int cardinality)
        {
            int mask = 0;
            for (int i = 0; i < cardinality; i++)
            {
                mask |= 1 << order[i];
            }//for i
            return values[mask];
        }

        public double Value(int mask)
        {
            return values[mask];
        }

        //protected override void ContractDerived(int element)
        //{
        //    throw new NotImilementedException();
        //}

    }
}

#endif