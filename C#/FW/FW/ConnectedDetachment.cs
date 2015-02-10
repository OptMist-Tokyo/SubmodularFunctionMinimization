//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.Diagnostics;
//using System.Threading.Tasks;

//namespace Onigiri.FW
//{
//    public class ConnectedDetachment : SubmodularOracle
//    {

//        double[] modular;
//        HashSet<int>[] edges;
//        UnionFind uf;
//        bool[] contractedVertex;
//        bool[] deletedVertex;


//        double emptyValue;
//        double fullValue;

//        public ConnectedDetachment(int N, int[] remainder, int countContract, int countDelete, int count, double[] modular, HashSet<int>[]edges,UnionFind uf,bool[]contractedVertex,bool[]deletedVertex, double emptyValue, double fullValue, Stopwatch sw)
//        {
//            Copy(N, remainder, countContract, countDelete, count,sw);
//            this.modular = modular;
//            this.edges = edges;
//            this.emptyValue = emptyValue;
//            this.fullValue = fullValue;
//            this.uf = uf.Copy();
//            this.contractedVertex = new bool[N];
//            this.deletedVertex = new bool[N];
//            for (int i = 0; i < N; i++)
//            {
//                this.contractedVertex[i] = contractedVertex[i];
//                this.deletedVertex[i] = deletedVertex[i];
//            }
//        }

//        public ConnectedDetachment(int n, double[] modular, HashSet<int>[] edges)
//        {
//            SetVariables(n, modular, edges);
//        }

//        public ConnectedDetachment(string path)
//        {
//            var streamReader = new StreamReader(path);
//            int n = int.Parse(streamReader.ReadLine());
//            var modular = new double[n];
//            for (int i = 0; i < n; i++)
//            {
//                modular[i] = double.Parse(streamReader.ReadLine());
//            }//for i
//            var edges = new HashSet<int>[n];
//            for (int i = 0; i < n; i++)
//            {
//                edges[i] = new HashSet<int>();
//                var list = Array.ConvertAll<string, int>(streamReader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), int.Parse);
//                foreach (var e in list.Skip(1))
//                {
//                    edges[i].Add(e);
//                }//foreach e
//            }//for i
//            streamReader.Close();
//            SetVariables(n, modular, edges);
//        }

//        private void SetVariables(int n, double[] modular, HashSet<int>[] edges)
//        {
//            SetParentVariable(n);
//            this.modular = modular;
//            this.edges = edges;
//            this.uf = new UnionFind(N);
//            this.contractedVertex = new bool[N];
//            this.deletedVertex = new bool[N];
//            emptyValue = 0;
//            fullValue = -1;
//            int cntEdges  =0;
//            for (int i = 0; i < N; i++)
//            {
//                fullValue -= modular[i];
//                cntEdges += edges[i].Count;
//            }
//            fullValue += (cntEdges >> 1);
//        }

//        public override double EmptyValue
//        {
//            get { return emptyValue; }
//        }

//        public override double FullValue
//        {
//            get { return fullValue; }
//        }

//        protected override void CalcBaseInherent(int[] order, double[] b)
//        {
//            throw new NotImplementedException();
//        }

//        protected override void CalcBaseHeadInherent(int[] order, double[] b)
//        {
//            throw new NotImplementedException();
//        }

//        protected override void CalcBaseTailInherent(int[] order, double[] b)
//        {
//            throw new NotImplementedException();
//        }

//        public override void Contract(List<int> reduceList)
//        {
//            foreach (var item in reduceList)
//            {
//                int cur = remainder[item];
//                emptyValue -= modular[cur];
//                foreach (var v in edges[cur])
//                {
//                    if (!contractedVertex[v])
//                    {
//                        emptyValue++;
//                    }
//                }
//                contractedVertex[cur] = true;
//            }
//        }

//        public override void Delete(List<int> deleteList)
//        {
//            foreach (var item in deleteList)
//            {
//                int cur = remainder[item];
//                fullValue += modular[cur];
//                foreach (var v in edges[cur])
//                {
//                    if (deletedVertex[v])
//                    {
//                        fullValue--;
//                    }
//                }
//                deletedVertex[cur] = true;
//            }
//        }


//        public override SubmodularOracle Copy()
//        {
//            return new ConnectedDetachment(N, remainder, CountContract, CountDelete, Count, modular, edges, uf,contractedVertex,deletedVertex, emptyValue, fullValue, sw);
//        }

//    }
//}
