using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Onigiri.FW
{
    public class NonPositiveSymmetricMatrixSummation:SubmodularOracle
    {
        double[] modular;
        double[][] matrix;
        double emptyValue;
        double fullValue;
        double[] reduced;
        double[] deleted;

        public NonPositiveSymmetricMatrixSummation(int N, int[] remainder, int countReduce, int countDelete, int count, double[] modular, double[][] matrix, double emptyValue, double fullValue, double[] reduced, double[] deleted,Stopwatch sw)
        {
            Copy(N, remainder, countReduce, countDelete, count,sw);
            this.modular = modular;
            this.matrix = matrix;
            this.emptyValue = emptyValue;
            this.fullValue = fullValue;
            this.reduced = new double[N];
            this.deleted = new double[N];
            for (int i = 0; i < N; i++)
            {
                this.reduced[i] = reduced[i];
                this.deleted[i] = deleted[i];
            }
        }

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

        private void SetVariables(int n, double[] modular, double[][] matrix)
        {
            SetParentVariable(n);
            this.modular = modular;
            this.matrix = matrix;
            reduced = new double[n];
            deleted = new double[n];
            emptyValue = 0;
            SetFullState();
        }

        private void SetFullState()
        {
            fullValue = 0;
            for (int i = 0; i < N; i++)
            {
                fullValue += modular[i];
                for (int j = 0; j < N; j++)
                {
                    deleted[i] += matrix[i][j];
                }
                fullValue += deleted[i];
            }
        }

        public override double EmptyValue
        {
            get { return emptyValue; }
        }

        public override double FullValue
        {
            get { return fullValue; }
        }

        protected override void CalcBaseInherent(int[] order, double[] b)
        {
            for (int i = 0; i < Count; i++)
            {
                int index = order[i];
                int cur = remainder[index];
                double val = reduced[cur];
                for (int j = 0; j < i; j++)
                {
                    val += matrix[cur][remainder[order[j]]];
                }
                b[index] = 2 * val + modular[cur];
            }
        }

        protected override void CalcBaseHeadInherent(int[] order, double[] b)
        {
            for (int i = 0; i < order.Length; i++)
            {
                int index = order[i];
                int cur = remainder[index];
                double val = reduced[cur];
                for (int j = 0; j < i; j++)
                {
                    val +=  matrix[cur][remainder[order[j]]];
                }
                b[index] = 2 * val + modular[cur];
            }
        }

        protected override void CalcBaseTailInherent(int[] order, double[] b)
        {
            for (int i = order.Length - 1; i >= 0; i--)
            {
                int index = order[i];
                int cur = remainder[index];
                double val = deleted[cur];
                for (int j = order.Length - 1; j > i; j--)
                {
                    val -=  matrix[cur][remainder[order[j]]];
                }
                b[index] = 2*val + modular[cur];
            }
        }

        public override void Contract(List<int> reduceList)
        {
            foreach (var item in reduceList)
            {
                int cur = remainder[item];
                emptyValue += modular[cur];
                emptyValue += 2 * reduced[cur];
                foreach (var v in remainder)
                {
                    reduced[v] += matrix[cur][v];
                }
            }
        }

        public override void Delete(List<int> deleteList)
        {
            foreach (var item in deleteList)
            {
                int cur = remainder[item];
                fullValue -= modular[cur];
                fullValue -= 2 * deleted[cur];
                foreach (var v in remainder)
                {
                    deleted[v] -= matrix[cur][v];
                }
            }
        }

        public override SubmodularOracle Copy()
        {
            return new NonPositiveSymmetricMatrixSummation(N,remainder,CountContract,CountDelete,Count, modular, matrix, emptyValue, fullValue, reduced, deleted,sw);
        }

    }
}
