using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class SetCover : SubmodularOracle
    {
        protected double[] modular;
        protected double[] weight;
        protected int[][] edges;
        protected bool[] used;    //for calculation
        //bool[] usedMemo;    //for calculation

        public SetCover(int n, int m,double[] modular, double[]weight, int[][] edges)
        {
            SetVariables(n,m,modular, weight, edges);
        }
        
        public SetCover(string path)
        {
            var streamReader = new StreamReader(path);
            int n = int.Parse(streamReader.ReadLine());
            var modular = new double[n];
            for (int i = 0; i < n; i++)
            {
                modular[i] = double.Parse(streamReader.ReadLine());
            }//for i
            int m = int.Parse(streamReader.ReadLine());
            var weight = new double[m];
            for (int i = 0; i < m; i++)
            {
                weight[i] = double.Parse(streamReader.ReadLine());
            }//for i
            var edges = new int[n][];
            for (int i = 0; i < n; i++)
            {
                edges[i] = streamReader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Skip(1)
                    .Select(x=>int.Parse(x)).ToArray();
            }//for i
            streamReader.Close();
            SetVariables(n,m, modular, weight, edges);
        }

        private void SetVariables(int n,int m,double[] modular, double[] weight, int[][] edges)
        {
            SetVariable(n, fOfEmpty);
            this.M = m;
            this.modular = modular;
            this.weight = weight;
            this.edges = edges;

            used = new bool[M];
            //usedMemo = new bool[M];
        }

        protected int M
        {
            get;
            set;
        }

        internal override double Value(int[] order, int cardinality)
        {
            double res = 0;
            for (int i = 0; i < M; i++)
            {
                used[i] = false;
            }//for i
            for (int i = 0; i < cardinality; i++)
            {
                int cur = order[i];
                res += modular[cur];
                foreach (var neighboor in edges[cur])
                {
                    if (!used[neighboor])
                    {
                        used[neighboor] = true;
                        res += weight[neighboor];
                    }//if
                }//foreach neighboor
            }//for i
            return res;
        }

        internal override void Base(int[] order, double[] b)
        {
            for (int i = 0; i < M; i++)
            {
                used[i] = false;
            }//for i
            for (int i = 0; i < N; i++)
            {
                int cur = order[i];
                b[cur] = modular[cur];
                foreach (var neighboor in edges[cur])
                {
                    if (!used[neighboor])
                    {
                        used[neighboor] = true;
                        b[cur] += weight[neighboor];
                    }//if
                }//foreach neighboor
            }//for i
        }

        //protected override void ContractDerived(int element)
        //{
        //    throw new NotImilementedException();
        //}


    }
}
