using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class GraphicMatroid:SubmodularOracle
    {
            double[] modular;
           int[] heads;
           int[] tails;
           UnionFind uf;    //or calculation

           private int V
           {
               get;
               set;
           }


        public GraphicMatroid(int n, int V, double[] modular, int[]heads, int[]tails)
        {
            SetVariables(n, V, modular, heads, tails);
        }

        public GraphicMatroid(string path)
        {
            var streamReader = new StreamReader(path);
            int n = int.Parse(streamReader.ReadLine());
            var modular = new double[n];
            for (int i = 0; i < n; i++)
            {
                modular[i] = double.Parse(streamReader.ReadLine());
            }//for i
            var V = int.Parse(streamReader.ReadLine());
            var heads = new int[V];
            var tails = new int[V];
            for (int i = 0; i < V; i++)
            {
                int[] tmp = Array.ConvertAll<string, int>(streamReader.ReadLine().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries), int.Parse);
                heads[i] = tmp[0];
                tails[i] = tmp[1];
            }//for i
            streamReader.Close();
            SetVariables(n, V, modular, heads, tails);
        }

        private void SetVariables(int n, int V, double[] modular, int[] heads, int[] tails)
        {
            SetVariable(n, 0);
            this.V = V;
            this.modular = modular;
            this.heads = heads;
            this.tails = tails;

            uf = new UnionFind(V);
        }

        internal override double Value(int[] order, int cardinality)
        {
            double res = cardinality;
            uf.Clear();
            for (int i = 0; i < cardinality; i++)
            {
                res += modular[order[i]];
                if (uf.Same(heads[order[i]],tails[order[i]]))
                {
                    res--;
                }//if
                else
                {
                    uf.Unite(heads[order[i]], tails[order[i]]);
                }//else
            }//for i
            return res;
        }

        internal override void Base(int[] order, double[] b)
        {
            uf.Clear();
            for (int i = 0; i < N; i++)
            {
                b[order[i]] = modular[order[i]];
                if (!uf.Same(heads[order[i]], tails[order[i]]))
                {
                    uf.Unite(heads[order[i]], tails[order[i]]);
                    b[order[i]]++;
                }//if
            }//for i
        }

        //protected override void ContractDerived(int element)
        //{
        //    throw new NotImilementedException();
        //}

    }
}
