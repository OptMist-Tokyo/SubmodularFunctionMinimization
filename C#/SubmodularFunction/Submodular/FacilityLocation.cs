using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class FacilityLocation:SubmodularOracle
    {
        double[] modular;
        double[][] matrix;
        double[] maxRows;   //for calculation

        public FacilityLocation(int n, double[] modular, double[][] matrix)
        {
            SetVariables(n, modular, matrix);
        }

        public FacilityLocation(string path)
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

        private void SetVariables(int n,double []modular,double[][]matrix)
        {
            SetVariable(n, 0);
            this.modular = modular;
            this.matrix = matrix;

            maxRows = new double[N];
        }

        internal override double Value(int[] order, int cardinality)
        {
            double res = 0;
            for (int i = 0; i < cardinality; i++)
            {
                res += modular[order[i]];
            }//for i
            for (int i = 0; i < N; i++)
            {
                double maxRow = 0;
                for (int j = 0; j < cardinality; j++)
                {
                    maxRow = Math.Max(maxRow, matrix[i][order[j]]);
                }//for j
                //double maxCol = 0;
                //for (int j = cardinality; j < N; j++)
                //{
                //    maxCol = Math.Max(maxCol, matrix[order[j]][i]);
                //}//for j
                res += maxRow;// +maxCol;
            }//for i
            return res;
        }

        internal override void Base(int[] order, double[] b)
        {
            for (int i = 0; i < N; i++)
            {
                maxRows[i] = 0;
            }//for i
            //var maxCols = new double[N];
            for (int i = 0; i < N; i++)
            {
                b[order[i]] = modular[order[i]];
                for (int j = 0; j < N; j++)
                {
                    double nextRow = Math.Max(maxRows[j], matrix[j][order[i]]);
                    b[order[i]] += nextRow - maxRows[j];
                    maxRows[j] = nextRow;

                    //double nextCol = Math.Max(maxCols[j], matrix[order[N - 1 - i]][j]);
                    //b[order[N - 1 - i]] -= nextCol - maxCols[j];
                    //maxCols[j] = nextCol;
                }//for j
            }//for i
        }

        //protected override void ContractDerived(int element)
        //{
        //    throw new NotImilementedException();
        //}

    }
}
