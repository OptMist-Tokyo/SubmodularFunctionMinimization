using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Onigiri.FW
{
    public class SetCover:SubmodularOracle
    {

        protected double[] modular;
        protected double[] weight;
        protected int[][] edges;
        protected int[] emptyUsed;
        protected int[] fullUsed;
        protected int[] memo;

        protected int M { get; set; }
        protected double SumOfEmptyModular { get; set; }
        protected double SumOfEmptyWeight { get; set; }
        protected double SumOfFullModular { get; set; }
        protected double SumOfFullWeight { get; set; }

        public SetCover(int N,int[]remainder,int countReduce,int countDelete,int count,double[]modular,double[]weight,int[][]edges,int[]emptyUsed,int[]fullUsed,int M,double SumOfEmptyModular,double SumOfEmptyWeight,double SumOfFullModular,double SumOfFullWeight,Stopwatch sw) 
        {
            Copy(N, remainder, countReduce, countDelete, count,sw);
            this.modular = modular;
            this.weight = weight;
            this.edges = edges;
            this.emptyUsed = new int[M];
            this.fullUsed = new int[M];
            this.memo = new int[M];
            this.M = M;
            this.SumOfEmptyModular = SumOfEmptyModular;
            this.SumOfEmptyWeight = SumOfEmptyWeight;
            this.SumOfFullModular = SumOfFullModular;
            this.SumOfFullWeight = SumOfFullWeight;
            for (int i = 0; i < M; i++)
            {
                this.emptyUsed[i] = emptyUsed[i];
                this.fullUsed[i] = fullUsed[i];
            }
        }

        public SetCover(int n, int m, double[] modular, double[] weight, int[][] edges)
        {
            SetVariables(n, m, modular, weight, edges);
        }

        public SetCover(string path)
        {
            var streamReader = new StreamReader(path);
            int n = int.Parse(streamReader.ReadLine());
            var modular = new double[n];
            for (int i = 0; i < n; i++)
            {
                modular[i] = double.Parse(streamReader.ReadLine());
            }
            int m = int.Parse(streamReader.ReadLine());
            var weight = new double[m];
            for (int i = 0; i < m; i++)
            {
                weight[i] = double.Parse(streamReader.ReadLine());
            }
            var edges = new int[n][];
            for (int i = 0; i < n; i++)
            {
                edges[i] = streamReader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Skip(1)
                    .Select(x => int.Parse(x)).ToArray();
            }
            streamReader.Close();
            SetVariables(n, m, modular, weight, edges);
        }

        private void SetVariables(int n, int m, double[] modular, double[] weight, int[][] edges)
        {
            SetParentVariable(n);
            this.M = m;
            this.modular = modular;
            this.weight = weight;
            this.edges = edges;
            emptyUsed = new int[M];
            fullUsed = new int[M];
            memo = new int[M];
            SumOfEmptyModular = SumOfEmptyWeight = 0;
            SetFullState(modular, weight, edges);
        }

        private void SetFullState(double[] modular, double[] weight, int[][] edges)
        {
            SumOfFullModular = modular.Sum();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < edges[i].Length; j++)
                {
                    fullUsed[edges[i][j]]++;
                }
            }
            SumOfFullWeight = 0;
            for (int i = 0; i < M; i++)
            {
                if (fullUsed[i] > 0)
                {
                    SumOfFullWeight += weight[i];
                }
            }
        }

        public override double EmptyValue 
        { get { return CalcValue(SumOfEmptyModular, SumOfEmptyWeight); } }

        public override double FullValue
        { get { return CalcValue(SumOfFullModular, SumOfFullWeight); } }

        protected override void CalcBaseInherent(int[] order, double[] b)
        {
            double modular = SumOfEmptyModular;
            double weight = SumOfEmptyWeight;
            double prev = CalcValue(modular, weight);
            Copy(emptyUsed, memo);
            for (int i = 0; i < Count; i++)
            {
                int index = order[i];
                int cur = remainder[index];
                modular += this.modular[cur];
                foreach (var v in edges[cur])
                {
                    if (emptyUsed[v] == 0)
                    {
                        weight += this.weight[v];
                    }
                    emptyUsed[v]++;
                }
                double next = CalcValue(modular, weight);
                b[index] = next - prev;
                prev = next;
            }
            Copy(memo, emptyUsed);
        }

        private void Copy(int[] baseArray, int[] copy)
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                copy[i] = baseArray[i];
            }
        }

        protected override void CalcBaseHeadInherent(int[] order, double[] b)
        {
            double modular = SumOfEmptyModular;
            double weight = SumOfEmptyWeight;
            double prev = CalcValue(modular, weight);
            foreach (var index in order)
            {
                int cur = remainder[index];
                modular += this.modular[cur];
                foreach (var v in edges[cur])
                {
                    if (emptyUsed[v] == 0)
                    {
                        weight += this.weight[v];
                    }
                    emptyUsed[v]++;
                }
                double next = CalcValue(modular, weight);
                b[index] = next - prev;
                prev = next;

            }

            for (int i = 0; i < order.Length; i++)
            {
                int cur = remainder[order[i]];
                foreach (var v in edges[cur])
                {
                    emptyUsed[v]--;
                }
            }
        }

        protected override void CalcBaseTailInherent(int[] order, double[] b)
        {
            double modular = SumOfFullModular;
            double weight = SumOfFullWeight;
            double prev = CalcValue(modular, weight);
            foreach (var index in order.Reverse())
            {
                int cur = remainder[index];
                modular -= this.modular[cur];
                foreach (var v in edges[cur])
                {
                    fullUsed[v]--;
                    if (fullUsed[v] == 0)
                    {
                        weight -= this.weight[v];
                    }
                }
                double next = CalcValue(modular, weight);
                b[index] = prev - next;
                prev = next;                
            }

            for (int i = 0; i < order.Length; i++)
            {
                int cur = remainder[order[i]];
                foreach (var v in edges[cur])
                {
                    fullUsed[v]++;
                }
            }
        }

        public override void Contract(List<int> reduceList)
        {
            foreach (var item in reduceList)
            {
                int cur = remainder[item];
                SumOfEmptyModular += modular[cur];
                foreach (var v in edges[cur])
                {
                    if (emptyUsed[v]==0)
                    {
                        SumOfEmptyWeight += weight[v];
                    }
                    emptyUsed[v]++;
                }
            }
        }

        public override void Delete(List<int> deleteList)
        {
            foreach (var item in deleteList)
            {
                int cur = remainder[item];
                SumOfFullModular -= modular[cur];
                foreach (var v in edges[cur])
                {
                    fullUsed[v]--;
                    if (fullUsed[v] == 0)
                    {
                        SumOfFullWeight -= weight[v];
                    }
                }
            }
        }

        public override SubmodularOracle Copy()
        {
            return new SetCover(N,remainder,CountContract,CountDelete,Count,modular, weight, edges, emptyUsed, fullUsed, M, SumOfEmptyModular, SumOfEmptyWeight, SumOfFullModular, SumOfFullWeight,sw);
        }

        protected virtual double CalcValue(double modular,double weight)
        {
            return modular + weight;
        }

        
    }
}
