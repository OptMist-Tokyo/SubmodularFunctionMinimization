using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onigiri.Algorithm;

namespace Onigiri.Submodular
{
    public class FW:SubmodularFunctionMinimization
    {
        SubmodularLinearOptimization LOAlgorithm;

        public override SFMResult Minimization(SubmodularOracle oracle, double absEps = DefaultAbsEps, double relativeEps = DefaultRelativeEps)
        {
            InitializeFW(oracle, absEps, relativeEps);
            //var wolfeAlgo = new HeuristicFW(LOAlgorithm);
            //var wolfeAlgo = new OriginalFW(LOAlgorithm);
            var wolfeAlgo = new WolfeMinimumNormPoint(LOAlgorithm);
            //var wolfeAlgo = new WolfeMinimumNormPointLine();
            //var wolfeAlgo = new WolfeMinimumNormPointPlane();
            wolfeAlgo.CalcMinimumNormPoint();
            x = (double[])wolfeAlgo.X;
            Iteration = wolfeAlgo.Iteration;
            //Iteration = wolfeAlgo.MinorCycle;
            SetMinimizer();
            return SetResults(oracle);
        }

        private void SetMinimizer()
        {
            N = x.Length;
            Array.Resize(ref minimizer, N);
            double threshold = 0;// 1.0 / (2.0 * N);     //TODO : error
            for (int i = 0; i < N; i++)
            {
                minimizer[i] = (x[i] <= threshold);
            }//for i
        }

        private void InitializeFW(SubmodularOracle oracle, double absEps, double relativeEps)
        {
            Initialize(oracle, absEps, relativeEps);
            LOAlgorithm = new SubmodularLinearOptimization(oracle);
        }

        private List<int[]> GetFirstOrders()
        {
            return GetFirstTrivialOrders();
        }



    }

    class HeuristicFW : WolfeMinimumNormPoint
    {
        ReducedOracle ro;
        int[] cnt;
        int[][] data;
        List<int> lower;
       List<int> upper;
        

        public HeuristicFW(LinearOptimization LOAlgorithm, double[][] kernelMatrix = null, double absEps = DefaultAbsEps, double relativeEps = DefaultRelativeEps)
            : base(LOAlgorithm, kernelMatrix, absEps, relativeEps)
        {
            ro = ((SubmodularLinearOptimization)LOAlgorithm).Oracle as ReducedOracle;
            cnt = new int[LOAlgorithm.N];
            data = new int[LOAlgorithm.N + 1][];
            lower = new List<int>();
            upper = new List<int>();
        }

        private int Count
        {
            get;
            set;
        }

        //List<double> listF;

        //private void MakeList()
        //{
        //    if (listF==null)
        //    {
        //        listF = new List<double>();
        //    }
        //    int head = 0;
        //    int tail = X.Length - 1;
        //    int[] order = new int[X.Length];
        //    for (int i = 0; i < X.Length; i++)
        //    {
        //        if (X[i]<0)
        //        {
        //            order[head++] = i;
        //        }
        //        else
        //        {
        //            order[tail--] = i;
        //        }
        //    }

        //    listF.Add(ro.CalcValue(order, head));
        //    listF.Add(X.Sum(x => Math.Min(0, x))+ro.ReducedFOfEmpty);
        //}


        protected override bool HeuristicStop()
        {
            //MakeList();
            if (ro==null)
            {
                return false;
            }
            int n = ro.N;
            if (n<10||Iteration%10!=0)
            {
                return false;
            }
            if (n == 0 || X.Min() >= 0 || X.Max() <= 0)
            {
                return true;
            }
            int[] reorder;
            if (ReduceAndDeleteVertices(n, out reorder))
            {
                Reset(reorder);
            }
            return false;
        }

        private void Reset(int[]reorder)
        {
            int n = ro.N;
            ((SubmodularLinearOptimization)LOAlgorithm).Resize(n);
            components.Resize(n);
            Array.Resize(ref extremeBase, n);
            Array.Resize(ref b, n);

            components.Shuffle(reorder);
            components.SetX();
            currentNorm = double.MaxValue;
        }

        private bool ReduceAndDeleteVertices(int n,out int[]reorder)
        {
            SetData(components.GetData());
            //var list = new List<int>();
            //for (int i = 0; i < Count; i++)
            //{
            //    for (int j = 0; j < data[i].Length; j++)
            //    {
            //        if (data[i][j]==0)
            //        {
            //            break;
            //        }
            //        list.Add(data[i][j]);
            //    }
            //    list.Add(-1);
            //}
            int lowerIndex = SetLowerIndex(n);
            int upperIndex = SetUpperIndex(n);
            //double reducedValue = 0;
            //for (int i = 0; i < lowerIndex; i++)
            //{
            //    reducedValue+=X[data[0][i]];
            //}
            if (lowerIndex != -1 || upperIndex != n)
            {
                SetHash(n, lowerIndex, upperIndex);
                if (lower.Count > 0)
                {
                    ro.Reduce(lower);//,reducedValue);
                }
                if (upper.Count > 0)
                {
                    ro.Delete(upper);
                }
                reorder = ro.SetHash(n);
                return true;
            }
            reorder = null;
            return false;
        }

        private void SetHash(int n, int lowerIndex, int upperIndex)
        {
            lower.Clear();
            upper.Clear();
            for (int i = 0; i <= lowerIndex; i++)
            {
                lower.Add(data[0][i]);
            }
            for (int i = n - 1; i >= upperIndex; i--)
            {
                upper.Add(data[0][i]);
            }
        }

        private int SetUpperIndex(int n)
        {
            for (int i = 0; i < n; i++)
            {
                cnt[i] = 0;
            }
            int upperIndex = n;
            for (int i = n - 1; i >= 0; i--)
            {
                if (X[data[0][i]] < 0)
                {
                    break;
                }
                IsIdeal(ref upperIndex, i);
            }
            return upperIndex;
        }

        private int SetLowerIndex(int n)
        {
            for (int i = 0; i < n; i++)
            {
                cnt[i] = 0;
            }
            int lowerIndex = -1;
            for (int i = 0; i < n; i++)
            {
                if (X[data[0][i]] > 0)
                {
                    break;
                }
                IsIdeal(ref lowerIndex, i);
            }
            return lowerIndex;
        }

        private void IsIdeal(ref int upperIndex, int orderIndex)
        {
            for (int i = 0; i < Count; i++)
            {
                var cur = data[i][orderIndex];
                cnt[cur]++;
            }
            for (int i = 0; i <= orderIndex; i++)
            {
                if (cnt[data[0][i]] != Count)
                {
                    return;
                }
            }
            upperIndex = orderIndex;
        }

        private void SetData(object[] data)
        {
            Count = data.Length;
            for (int i = 0; i < data.Length; i++)
            {
                this.data[i] = (int[])data[i];
            }
        }

    }
}