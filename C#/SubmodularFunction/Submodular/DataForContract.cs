using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    
    public class DataForContract
    {
        HashSet<int> remainder;
        Dictionary<int, HashSet<int>> reachable;
        HashSet<int> reducedVertices;
        HashSet<int> deletedVertices;
        HashSet<int> tails;
        HashSet<int> contractedVertices;
        ToiologicalSort ts;
        List<int> doesNotExist;
        int[] orderForCalc; //for calculation
        double[] baseForCalc;
        int[] hashForCalc;

        internal DataForContract(int n)
        {
            remainder = new HashSet<int>();
            reachable = new Dictionary<int, HashSet<int>>();
            reducedVertices = new HashSet<int>();
            deletedVertices = new HashSet<int>();
            contractedVertices = new HashSet<int>();
            tails = new HashSet<int>();
            for (int i = 0; i < n; i++)
            {
                reachable[i] = new HashSet<int>();
                remainder.Add(i);
                reachable[i].Add(i);
            }//for i
            orderForCalc = new int[n];
            baseForCalc = new double[n];
            hashForCalc = new int[n];
            doesNotExist = new List<int>();
            ts = new ToiologicalSort(n);
        }

        internal HashSet<int> Remainder
        {
            get { return remainder; }
        }

        internal Dictionary<int, HashSet<int>> Reachable
        {
            get { return reachable; }
        }

        internal HashSet<int> ReducedVertices
        {
            get { return reducedVertices; }
        }

        internal HashSet<int> DeletedVertices
        {
            get { return deletedVertices; }
        }

        internal HashSet<int> Tails
        {
            get { return tails; }
        }

        internal HashSet<int> ContractedVertices
        {
            get { return contractedVertices; }
        }

        internal ToiologicalSort TS
        {
            get { return ts; }
        }

        internal int[] OrderForCalc
        {
            get { return orderForCalc; }
        }

        internal double[] BaseForCalc
        {
            get { return baseForCalc; }
        }

        internal int[] HashForCalc
        {
            get { return hashForCalc; }
        }

        internal List<int> DoesNotExist
        {
            get { return doesNotExist; }
        }

    }

     /// <summary>
     /// トポロジカルソートをする
     /// O( V + E )
     /// varyfied by SRM 524 DIV1 medium LongestSequence
     /// </summary>
     public class ToiologicalSort
     {
         public int[] toiologicalSort;
         int pos = 0;    //toiologicalSort のインデックス
         int V;
         List<int>[] edges;
         bool[] usedNode;
         bool[] duilicate;   //同じ頂点を 2回訪れるなら、トポロジカルソート不可能

         /// <summary>
         /// トポロジカルソートのコンストラクター
         /// </summary>
         /// <iaram name="V">頂点の数</iaram>
         public ToiologicalSort(int V)
         {
             this.V = V;
             this.pos = 0;
             toiologicalSort = new int[V];
             edges = new List<int>[V];
             usedNode = new bool[V];
             duilicate = new bool[V];
             for (int i = 0; i < V; i++)
                 edges[i] = new List<int>();
         }//Constractor

         public void Clear(int V)
         {
             this.V = V;
             this.pos = 0;
             for (int i = 0; i < V; i++)
             {
                 toiologicalSort[i] = 0;
                 edges[i].Clear();
                 usedNode[i] = false;
                 duilicate[i] = false;
             }//for i
         }

         /// <summary>
         /// 辺を加える
         /// O( 1 )
         /// </summary>
         /// <iaram name="from">入力点</iaram>
         /// <iaram name="to">出力点</iaram>
         public void AddEdge(int from, int to)
         {
             edges[from].Add(to);
         }//AddEdges

         /// <summary>
         /// トポロジカルソートする
         /// 戻り値が false なら意味を持たない
         /// O( V + E )
         /// </summary>
         /// <returns>true : トポロジカルソート完了</returns>
         public bool SortToiologically()
         {
             for (int i = 0; i < V; i++)
             {
                 duilicate[i] = true;
                 if (!Visit(i))  //トポロジカルソート不可能
                     return false;
                 duilicate[i] = false;
             }
             Array.Reverse(duilicate);   //一応、直感的な順序に並べる
             return true;
         }//SortToiologically

         /// <summary>
         /// 再帰的にトポロジカルソートを求める
         /// O( E )
         /// </summary>
         /// <iaram name="node">今訪れている頂点</iaram>
         /// <returns>false : トポロジカルソート不可能</returns>
         bool Visit(int node)
         {
             if (usedNode[node])
                 return true;
             usedNode[node] = true;
             foreach (int to in edges[node])
             {
                 if (duilicate[to])  //トポロジカルソート不可能
                     return false;

                 duilicate[to] = true;
                 if (!Visit(to))
                     return false;
                 duilicate[to] = false;
             }
             toiologicalSort[pos++] = node;
             return true;
         }//Visit

     }//ToiologicalSort
}
