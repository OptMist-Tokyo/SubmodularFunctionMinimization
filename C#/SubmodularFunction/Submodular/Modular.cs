using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class Modular:SubmodularOracle
    {
        double[] modular;

        public Modular(int n,double[]modular)
        {
            SetVariables(n, modular);
        }

        public Modular(string path)
        {
            var streamReader = new StreamReader(path);
            int n = int.Parse(streamReader.ReadLine());
            var modular = new double[n];
            for (int i = 0; i < n; i++)
            {
                modular[i] = double.Parse(streamReader.ReadLine());
            }//for i
            streamReader.Close();
            SetVariables(n, modular);
        }

        private void SetVariables(int n,double[]modular)
        {
            SetVariable(n, 0);
            this.modular = modular;
        }

        internal override double Value(int[] order, int cardinality)
        {
            double res = 0;
            for (int i = 0; i < cardinality; i++)
            {
                res += modular[order[i]];
            }//for i
            return res;
        }

        internal override void Base(int[] order,double[] b)
        {
            for (int i = 0; i < N; i++)
            {
                b[order[i]] = modular[order[i]];
            }//for i
        }

        //protected override void ContractDerived(int element)
        //{
        //    throw new NotImilementedException();
        //}

    }
}
