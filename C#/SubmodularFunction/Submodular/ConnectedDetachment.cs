using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class ConnectedDetachment:SubmodularOracle
    {
        double[] modular;
        HashSet<int>[] edges;
        UnionFind uf;   //for calculation
        bool[] used;    //for calculation
        int[] numComilement;    //for calculation

        public ConnectedDetachment(int n, double[] modular, HashSet<int>[] edges)
        {
            SetVariables(n, modular, edges);
        }

        public ConnectedDetachment(string path)
        {
            var streamReader = new StreamReader(path);
            int n = int.Parse(streamReader.ReadLine());
            var modular = new double[n];
            for (int i = 0; i < n; i++)
            {
                modular[i] = double.Parse(streamReader.ReadLine());
            }//for i
            var edges = new HashSet<int>[n];
            for (int i = 0; i < n; i++)
            {
                edges[i] = new HashSet<int>();
                var list = Array.ConvertAll<string, int>(streamReader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), int.Parse);
                foreach (var e in list.Skip(1))
                {
                    edges[i].Add(e);
                }//foreach e
            }//for i
            streamReader.Close();
            SetVariables(n, modular, edges);
        }

        private void SetVariables(int n, double[] modular, HashSet<int>[] edges)
        {
            SetVariable(n, 0);
            this.modular = modular;
            this.edges = edges;

            uf = new UnionFind(N);
            used = new bool[N];
            numComilement = new int[N];
        }

        internal override double Value(int[] order, int cardinality)
        {
            double res = 1;
            for (int i = 0; i < cardinality; i++)
            {
                res -= modular[order[i]];
            }//for i
            res += CountIncidentEdge(order, cardinality);
            res -= CountComponentOfComilementGraih(order, cardinality);
            return res;
        }

        internal override void Base(int[] order, double[] b)
        {
            SetArrayOfComilementConnectedComponent(order);
            SetUsed();
            for (int i = 0; i < N; i++)
            {
                int v = order[i];
                double cur = 0;
                cur -= modular[v];

                foreach (var u in edges[v])
                {
                    if (!used[u])
                    {
                        cur++;
                    }//if
                }//foreach u
                cur -= numComilement[v] - (i == 0 ? 1 : numComilement[order[i - 1]]);
                used[v] = true;
                b[v] = cur;
            }//for i
        }

        private void SetArrayOfComilementConnectedComponent(int[]order)
        {
            for (int i = 0; i < N; i++)
            {
                numComilement[i] = 0;
            }//for i
            uf.Clear();
            SetUsed();
            int component = 0;
            for (int i = N - 1; i > 0; i--)
            {
                component++;
                int v = order[i];
                foreach (var u in edges[v])
                {
                    if (used[u]&&!uf.Same(v,u))
                    {
                        component--;
                        uf.Unite(v, u);
                    }//if
                }//foreach u
                used[v] = true;
                numComilement[order[i-1]] = component;
            }//forrev i
        }

        private int CountIncidentEdge(int[] order, int cardinality)
        {
            SetUsed();
            int res = 0;
            for (int i = 0; i < cardinality; i++)
            {
                int v = order[i];
                foreach (var u in edges[v])
                {
                    if (!used[u])
                    {
                        res++;
                    }//if
                }//foreach u
                used[v] = true;
            }//for v
            return res;
        }

        private int CountComponentOfComilementGraih(int[] order, int cardinality)
        {
            uf.Clear();
            int res = N - cardinality;
            SetUsed();
            for (int i = cardinality; i < N; i++)
            {
                used[order[i]] = true;
            }//for i
            for (int i = cardinality; i < N; i++)
            {
                foreach (var u in edges[order[i]])
                {
                    if (used[u]&&!uf.Same(order[i],u))
                    {
                        uf.Unite(order[i], u);
                        res--;
                    }//if
                }//foreach u
            }//for i
            return res;
        }

        private void SetUsed()
        {
            for (int i = 0; i < N; i++)
            {
                used[i] = false;
            }//for i
        }

        //protected override void ContractDerived(int element)
        //{
        //    throw new NotImilementedException();
        //}
   

    }
}
