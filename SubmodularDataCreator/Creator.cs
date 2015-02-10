using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PseudoRandom;

namespace SubmodularDataSet
{
    class Creator
    {
        public static int GetRangeInt(int min, int max)
        {
            int range = checked(max - min + 1);
            if (range <= 0)
            {
                throw new ArgumentException("The range must be non-negative.");
            }//if
            return range;
        }

        public static double GetRangeDouble(double min, double max)
        {
            double range = max - min;
            if (range < 0)
            {
                throw new ArgumentException("The range must be non-negative.");
            }//if
            return range;
        }

        public static int[] MakeRandomArray(int n, int min, int max, MersenneTwister rand)
        {
            var range = GetRangeInt(min, max);
            return Enumerable.Range(0, n)
                .Select(x => rand.genrand_N(max - min + 1) + min).ToArray();
        }

        public static double[] MakeRandomArray(int n, double min, double max, MersenneTwister rand)
        {
            var range = GetRangeDouble(min, max);
            return Enumerable.Range(0, n)
                .Select(x => rand.genrand_real1() * (max - min) + min).ToArray();
        }


        public static int[][] MakeRandomSymmetricMatrix(int n, int min, int max, MersenneTwister rand)
        {
            var range = GetRangeInt(min, max);
            var res = new int[n][];
            for (int i = 0; i < n; i++)
            {
                res[i] = new int[n];
            }//for i
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    res[i][j] = res[j][i] = rand.genrand_N(range) + min;
                }//for j
            }//for i
            return res;
        }

        public static double[][] MakeRandomSymmetricMatrix(int n, double min, double max, MersenneTwister rand)
        {
            var range = GetRangeDouble(min, max);
            var res = new double[n][];
            for (int i = 0; i < n; i++)
            {
                res[i] = new double[n];
            }//for i
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    res[i][j] = res[j][i] = rand.genrand_real1() * range + min;
                }//for j
            }//for i
            return res;
        }

        internal static int[][] MakeRandomMatrix(int n, int min, int max, MersenneTwister rand, bool withoutDiagonal)
        {
            var range = GetRangeInt(min, max);
            var res = new int[n][];
            for (int i = 0; i < n; i++)
            {
                res[i] = new int[n];
            }//for i
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!withoutDiagonal || i != j)
                    {
                        res[i][j] = rand.genrand_N(range) + min;
                    }//if
                }//for j
            }//for i
            return res;
        }

        internal static double[][] MakeRandomMatrix(int n, double min, double max, MersenneTwister rand, bool withoutDiagonal)
        {
            var range = GetRangeDouble(min, max);
            var res = new double[n][];
            for (int i = 0; i < n; i++)
            {
                res[i] = new double[n];
            }//for i
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!withoutDiagonal || i != j)
                    {
                        res[i][j] = rand.genrand_real1() * range + min;
                    }//if
                }//for j
            }//for i
            return res;
        }


        internal static int MakeValue(int min, int max, MersenneTwister rand)
        {
            var range = GetRangeInt(min, max);
            return rand.genrand_N(range) + min;
        }

        internal static double MakeValue(double min, double max, MersenneTwister rand)
        {
            var range = GetRangeDouble(min, max);
            return range * rand.genrand_real1() + min;
        }


        internal static List<int>[] MakeRandomConnectedGraph(int n, double probOfEdge, MersenneTwister rand)
        {
            bool[][] edges = MakeRandomConnctedGraphByBool(n, probOfEdge, rand);
            return MakeAdjacencyList(n, edges);
        }

        private static List<int>[] MakeAdjacencyList(int n, bool[][] edges)
        {
            var res = new List<int>[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = new List<int>();
                for (int j = 0; j < n; j++)
                {
                    if (edges[i][j])
                    {
                        res[i].Add(j);
                    }//if
                }//for j
            }//for i
            return res;
        }


        //internal static string[] MakeRandomSymmetricMatrixEdge(int n, double probOfEdge, MersenneTwister rand)
        //{
        //    bool[][] edges = MakeRandomConnctedGraphByBool(n, probOfEdge, rand);
        //    var res = new string[n];
        //    for (int i = 0; i < n; i++)
        //    {
        //        var stringBuilder = new StringBuilder();
        //        for (int j = 0; j < n; j++)
        //        {
        //            stringBuilder.Append((edges[i][j] ? "1" : "0"));
        //        }//for j
        //        res[i] = stringBuilder.ToString();
        //    }//for i
        //    return res;
        //}

        private static bool[][] MakeRandomConnctedGraphByBool(int n, int numOfVertices, double probOfEdge, MersenneTwister rand)
        {
            bool[][] edges = new bool[n][];
            for (int i = 0; i < n; i++)
            {
                edges[i] = new bool[numOfVertices];
            }//for i
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < numOfVertices; j++)
                {
                    edges[i][j] = (rand.genrand_real1() <= probOfEdge);
                }//for j
            }//for i
            return edges;
        }

        private static bool[][] MakeRandomConnctedGraphByBool(int n, double probOfEdge, MersenneTwister rand)
        {
            bool[][] edges = new bool[n][];
            for (int i = 0; i < n; i++)
            {
                edges[i] = new bool[n];
            }//for i
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    edges[i][j] = edges[j][i]
                        = (j == i + 1) || rand.genrand_real1() <= probOfEdge;
                }//for j
            }//for i
            return edges;
        }

        internal static int[][] MakeRandomGraphWithSomeEdges(int numberOfEdge, int numberOfVertices, MersenneTwister rand)
        {
            int[][] edges = new int[numberOfEdge][];
            for (int i = 0; i < numberOfEdge; i++)
            {
                edges[i] = new int[2];
                edges[i][0] = rand.genrand_N(numberOfVertices);
                edges[i][1] = rand.genrand_N(numberOfVertices);
            }//for i
            return edges;
        }

        //internal static string[] MakeRandomConnectedGraphWithSomeEdges(int n, int numberOfEdge, Random rand)
        //{
        //    var graph = new bool[n][];
        //    for (int i = 0; i < n; i++)
        //    {
        //        graph[i] = new bool[n];
        //    }//for i
        //    for (int i = 0; i < numberOfEdge; i++)
        //    {
        //        int u = rand.Next(n);
        //        int v = rand.Next(n);
        //        graph[u][v] = graph[v][u] = true;
        //    }//for i
        //    var res = new string[n];
        //    for (int i = 0; i < n; i++)
        //    {
        //        var stringBuilder = new StringBuilder();
        //        for (int j = 0; j < n; j++)
        //        {
        //            if (j!=0)
        //            {
        //                stringBuilder.Append(" ");
        //            }//if
        //            stringBuilder.Append(graph[i][j] ? "1" : "0");
        //        }//for j
        //        res[i] = stringBuilder.ToString();
        //    }//for i
        //    return res;
        //}



        internal static List<int>[] MakeRandomGraph(int n, int numOfVertices, double probOfEdge, MersenneTwister rand)
        {
            var graph = MakeRandomConnctedGraphByBool(n, numOfVertices, probOfEdge, rand);
            return MakeAdjacencyList(n, graph);
        }


        internal static string[] MakeValueRondomBinaryMatrix(int n, int rowLength, double probOfElement, MersenneTwister rand)
        {
            var res = new string[rowLength];
            for (int i = 0; i < rowLength; i++)
            {
                var rowVector = new StringBuilder();
                for (int j = 0; j < n; j++)
                {
                    rowVector.Append((rand.genrand_real1() <= probOfElement ? 1 : 0));
                }//for j
                res[i] = rowVector.ToString();
            }//for i
            return res;
        }


        internal static int[][] MakeTriangleMatrix(int n, int weightElementMin, int weightElementMax, MersenneTwister rand)
        {
            var res = new int[n][];
            for (int i = 0; i < n; i++)
            {
                res[i] = new int[n];
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    res[i][j] = rand.genrand_N(GetRangeInt(weightElementMin, weightElementMax)) + weightElementMin;
                }
            }
            return res;
        }

        internal static double[][] MakeTriangleMatrix(int n, double weightElementMin, double weightElementMax, MersenneTwister rand)
        {
            var res = new double[n][];
            for (int i = 0; i < n; i++)
            {
                res[i] = new double[n];
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    res[i][j] = rand.genrand_real1() * GetRangeDouble(weightElementMin, weightElementMax) + weightElementMin;
                }
            }
            return res;
        }



    }



}
