using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class Cut : SubmodularOracle
    {
        double[] modular;
        double[][] weight;

        public Cut(int n, double[] modular, double[][] weight)
        {
            SetVariables(n, modular, weight);
        }

        public Cut(string path)
        {
            var streamReader = new StreamReader(path);
            int n = int.Parse(streamReader.ReadLine());
            var modular = new double[n];
            for (int i = 0; i < n; i++)
            {
                modular[i] = double.Parse(streamReader.ReadLine());
            }//for i
            var weight = new double[n][];
            for (int i = 0; i < n; i++)
            {
                weight[i] = Array.ConvertAll<string, double>(streamReader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), double.Parse);
            }//for i
            streamReader.Close();
            SetVariables(n, modular, weight);
        }

        private void SetVariables(int n, double[] modular, double[][] weight)
        {
            SetVariable(n, 0);
            this.modular = modular;
            this.weight = weight;
        }

        internal override double Value(int[] order, int cardinality)
        {
            double res = 0;
            for (int i = 0; i < cardinality; i++)
            {
                res += modular[order[i]];
                for (int j = cardinality; j < N; j++)
                {
                    res += weight[order[i]][order[j]];
                }//for j
            }//for i
            return res;
        }

        internal override void Base(int[] order, double[] b)
        {
            for (int i = 0; i < N; i++)
            {
                int cur = order[i];
                b[cur] = modular[cur];
                for (int j = 0; j < i; j++)
                {
                    b[cur] -= weight[order[j]][cur];
                }//for j
                for (int j = i + 1; j < N; j++)
                {
                    b[cur] += weight[cur][order[j]];
                }//for j
            }//for i
        }

        //protected override void ContractDerived(int element)
        //{
        //    throw new NotImilementedException();
        //}
     

    }
}
