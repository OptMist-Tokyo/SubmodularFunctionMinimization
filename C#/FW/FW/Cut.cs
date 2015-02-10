using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.FW
{
    public class Cut : SubmodularOracle
    {
        double[] modular;
        double[][] weight;
        double emptyValue;
        double fullValue;
        double[] reduceInEdges;
        double[] reduceOutEdges;
        double[] deleteInEdges;
        double[] deleteOutEdges;

        public Cut(int N, int[] remainder, int countReduce, int countDelete, int count, double[] modular, double[][] weight, double emptyValue, double fullValue, double[] reduceInEdges, double[] reduceOutEdges, double[] deleteInEdges, double[] deleteOutEdges,Stopwatch sw)
        {
            Copy(N, remainder, countReduce, countDelete, count,sw);
            this.modular = modular;
            this.weight = weight;
            this.emptyValue = emptyValue;
            this.fullValue = fullValue;
            this.reduceInEdges = new double[N];
            this.reduceOutEdges = new double[N];
            this.deleteInEdges = new double[N];
            this.deleteOutEdges = new double[N];
            for (int i = 0; i < N; i++)
            {
                this.reduceInEdges[i] = reduceInEdges[i];
                this.reduceOutEdges[i] = reduceOutEdges[i];
                this.deleteInEdges[i] = deleteInEdges[i];
                this.deleteOutEdges[i] = deleteOutEdges[i];
            }
        }

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
            SetParentVariable(n);
            this.modular = modular;
            this.weight = weight;
            emptyValue = fullValue = 0;
            reduceInEdges = new double[n];
            reduceOutEdges = new double[n];
            deleteInEdges = new double[n];
            deleteOutEdges = new double[n];
            SetInitialState();
        }

        private void SetInitialState()
        {
            for (int i = 0; i < N; i++)
            {
                fullValue += modular[i];
                for (int j = 0; j < N; j++)
                {
                    reduceOutEdges[i] += weight[i][j];
                    deleteInEdges[i] += weight[j][i];
                }
            }
        }

        public override double EmptyValue
        { get { return emptyValue; } }

        public override double FullValue
        { get { return fullValue; } }

        protected override void CalcBaseInherent(int[] order, double[] b)
        {
            for (int i = 0; i < Count; i++)
            {
                int index = order[i];
                int cur = remainder[index];
                b[index] = modular[cur] + reduceOutEdges[cur] - reduceInEdges[cur];
                for (int j = 0; j < i; j++)
                {
                    int v = remainder[order[j]];
                    b[index] -= weight[cur][v] + weight[v][cur];
                }
            }
        }

        protected override void CalcBaseHeadInherent(int[] order, double[] b)
        {
            for (int i = 0; i < order.Length; i++)
            {
                int index = order[i];
                int cur = remainder[index];
                b[index] = modular[cur] + reduceOutEdges[cur] - reduceInEdges[cur];
                for (int j = 0; j < i; j++)
                {
                    int v = remainder[order[j]];
                    b[index] -= weight[cur][v] + weight[v][cur];
                }
            }
        }

        protected override void CalcBaseTailInherent(int[] order, double[] b)
        {
            for (int i = order.Length - 1; i >= 0; i--)
            {
                int index = order[i];
                int cur = remainder[index];
                b[index] = modular[cur] + deleteInEdges[cur] - deleteOutEdges[cur];
                for (int j = i+1; j < order.Length; j++)
                {
                    int v = remainder[order[j]];
                    b[index] -= weight[cur][v] + weight[v][cur];
                }
            }
        }

        public override void Contract(List<int> reduceList)
        {
            foreach (var item in reduceList)
            {
                int cur = remainder[item];
                emptyValue += modular[cur];
                emptyValue -= reduceInEdges[cur];
                emptyValue += reduceOutEdges[cur];
                foreach (var v in remainder)
                {
                    reduceInEdges[v] += weight[cur][v];
                    reduceOutEdges[v] -= weight[v][cur];
                }
            }
        }

        public override void Delete(List<int> deleteList)
        {
            foreach (var item in deleteList)
            {
                int cur = remainder[item];
                fullValue -= modular[cur];
                fullValue += deleteInEdges[cur];
                fullValue -= deleteOutEdges[cur];
                foreach (var v in remainder)
                {
                    deleteInEdges[v] -= weight[cur][v];
                    deleteOutEdges[v] += weight[v][cur];
                }
            }
        }

        public override SubmodularOracle Copy()
        {
            return new Cut(N,remainder,CountContract,CountDelete,Count, modular, weight, emptyValue, fullValue, reduceInEdges, reduceOutEdges, deleteInEdges, deleteOutEdges,sw);
        }

    }
}
