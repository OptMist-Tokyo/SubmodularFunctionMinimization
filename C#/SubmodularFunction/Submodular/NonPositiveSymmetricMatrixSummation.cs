using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class NonPositiveSymmetricMatrixSummation : SubmodularOracle
    {
        double[] modular;
        double[][] matrix;

        public NonPositiveSymmetricMatrixSummation(int n, double[] modular, double[][] matrix)
        {
            SetVariables(n, modular, matrix);
        }

        public NonPositiveSymmetricMatrixSummation(string path)
        {
            var streamReader = new StreamReader(path);
             int n = int.Parse(streamReader.ReadLine());
           var modular = new double[n];
            for (int i = 0; i < n; i++)
            {
                modular[i] = double.Parse(streamReader.ReadLine());
            }//for i
           var matrix = new double[n][];
            for (int i = 0; i < n; i++)
            {
                matrix[i] = Array.ConvertAll<string, double>(streamReader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), double.Parse);
            }//for i
            streamReader.Close();
            SetVariables(n, modular, matrix);
        }

        private void SetVariables(int n,double[]modular,double[][]matrix)
        {
            SetVariable(n, 0);
            this.modular = modular;
            this.matrix = matrix;
        }

        internal override double Value(int[] order, int cardinality)
        {
            double res = 0;
            for (int i = 0; i < cardinality; i++)
            {
                res += modular[order[i]];
                for (int j = 0; j < cardinality; j++)
                {
                    res += matrix[order[i]][order[j]];
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
                b[cur] += matrix[cur][cur];
                for (int j = 0; j < i; j++)
                {
                    b[cur] += matrix[cur][order[j]] + matrix[order[j]][cur];
                }//for j
            }//for i
        }

        //protected override void ContractDerived(int element)
        //{
        //    throw new NotImilementedException();
        //}


    }

}