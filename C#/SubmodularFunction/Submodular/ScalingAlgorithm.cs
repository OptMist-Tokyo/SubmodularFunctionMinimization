using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public abstract class ScalingAlgorithm : SubmodularFunctionMinimization
    {
        #region Common Members

        protected int[] hash;
        protected int[] baseOrder;
        protected double[][] phi;
        protected double[] z;
        protected int[] augmentingPath;   //for calclulation     
        protected HashSet<int> negative;      //S
        protected HashSet<int> positive;  //T
        protected HashSet<int> reachableFromNegative; //W
        protected Queue<int> que; //for calculation

        protected void InitializeFlowScaling(SubmodularOracle oracle, double absEps, double relativeEps)
        {
            Initialize(oracle, absEps, relativeEps);
            phi = new double[N][];
            for (int i = 0; i < phi.Length; i++)
            {
                phi[i] = new double[N];
            }//for i
            z = new double[N];
            augmentingPath = new int[N];
            negative = new HashSet<int>();
            positive = new HashSet<int>();
            reachableFromNegative = new HashSet<int>();
            hash = new int[N];
            baseOrder = new int[N];
            que = new Queue<int>();
        }

        protected abstract void DoScalingPhase(ref double delta, double endValue);
        protected abstract void InitializeAlgorithm(SubmodularOracle oracle, double absEps, double relativeEps);

        protected void SetCurrentVerticse()
        {
            int pos = 0;
            var order = components[0].Order;
            for (int i = 0; i < N; i++)
            {
                baseOrder[i] = order[i];
                hash[order[i]] = pos++;
            }//for i
        }

        protected void SetVariable()
        {
            var order = components[0].Order;
            for (int i = 0; i < N; i++)
            {
                int cur = baseOrder[i];
                z[hash[cur]] = x[cur];
                for (int j = 0; j < N; j++)
                {
                    phi[i][j] = 0;
                }//for j
            }//for i
        }

        protected void ModifyPhi(ref double delta)
        {
            delta *= 0.5;
            for (int u = 0; u < N; u++)
            {
                for (int v = 0; v < N; v++)
                {
                    double val = RoundingValue(phi[u][v], -delta);
                    if (val >=0)
                    {
                        phi[u][v] = val;
                        z[u] -= delta;
                        z[v] += delta;
                    }//if
                }//for v
            }//for u
        }

        [System.Diagnostics.Conditional("DEBUG")]
        protected void CheckZAndX()
        {
            for (int i = 0; i < N; i++)
            {
                double sum = x[baseOrder[i]];
                for (int j = 0; j < N; j++)
                {
                    sum += phi[i][j];
                    sum -= phi[j][i];
                }//for j
                if (RoundingValue(sum, -z[i]) != 0 && Math.Abs(sum) > 1e-3)
                {
                    System.Diagnostics.Debugger.Break();
                }//if
            }//for i
        }

        protected int SetVertices(double delta)
        {
            negative.Clear();
            positive.Clear();
            for (int v = 0; v < N; v++)
            {
                if (RoundingValue( z[v],delta) <= 0)
                    negative.Add(v);
                else if (RoundingValue( z[v],-delta) >= 0)
                    positive.Add(v);
            }//for v
            int terminalOfAugmentingPath = SetReachableFromNegative();
            return terminalOfAugmentingPath;
        }

        protected int SetReachableFromNegative()
        {
            for (int i = 0; i < N; i++)
            {
                augmentingPath[i] = -1;
            }//for i
            que.Clear();
            foreach (var v in negative)
            {
                que.Enqueue(v);
                augmentingPath[v] = v;
            }//foreach v
            while (que.Count > 0)
            {
                int cur = que.Dequeue();
                for (int next = 0; next < N; next++)
                {
                    if (augmentingPath[next] <0 && phi[cur][next] == 0)
                    {
                        augmentingPath[next] = cur;
                        if (positive.Contains(next))
                        {
                            return next;
                        }//if
                        que.Enqueue(next);
                    }//if
                }//for next
            }//while
            reachableFromNegative.Clear();
            for (int i = 0; i < N; i++)
            {
                if (augmentingPath[i]!=-1)
                {
                    reachableFromNegative.Add(i);
                }
            }
            return -1;
        }

        protected void Augment(int terminalOfAugmentingPath, double delta)
        {
            int cur = terminalOfAugmentingPath;
            while (augmentingPath[cur] != cur)    //reverse order
            {
                int prev = augmentingPath[cur];
                phi[prev][cur] = delta - phi[cur][prev];
                phi[cur][prev] = 0;
                cur = prev;
            }//while
            z[terminalOfAugmentingPath] -= delta;
            z[cur] += delta;
        }

        protected void SetTrivialMinimizer()
        {
            bool res = x.Any(val => val < 0);
            for (int i = 0; i < minimizer.Length; i++)
            {
                minimizer[i] = res;
            }//for i
        }

        protected double GetDelta()
        {
            double deltaMinus = 0;
            double deltaPlus = 0;
            foreach (var val in x)
            {
                if (val > 0)
                {
                    deltaPlus += val;
                }//if
                else
                {
                    deltaMinus -= val;
                }//else
            }//foreach val
            return Math.Min(deltaPlus, deltaMinus) / ((double)N * N);
        }

        private List<int[]> GetFirstOrders()
        {
            return GetFirstTrivialOrders();
        }

        #endregion

        #region Weakly Polytime

        protected SFMResult MinimizationWeakly(SubmodularOracle oracle, double absEps = DefaultAbsEps, double relativeEps = DefaultRelativeEps)
        {
            InitializeWeakly(oracle, absEps, relativeEps);
            double delta = GetDelta();
            if (delta == 0)
            {
                SetTrivialMinimizer();
            }//if
            else
            {
                DoScalingPhase(ref delta, (double)DefaultPrecision / ((double)N * N));
                SetMinimizerForWeakly(reachableFromNegative);
            }//else
            return SetResults(oracle);
        }

        private void InitializeWeakly(SubmodularOracle oracle, double absEps, double relativeEps)
        {
            InitializeAlgorithm(oracle, absEps, relativeEps);
            var firstOrders = GetFirstOrders();
            SetFirstComponents(firstOrders);
            SetX();
        }

        #endregion

        #region Strongly Polytime


        protected SFMResult MinimizationStrongly(SubmodularOracle oracle, double absEps = DefaultAbsEps, double relativeEps = DefaultRelativeEps)
        {
            InitializeStrongly(oracle, absEps, relativeEps);
            dataForContract = new DataForContract(N);
            DoIteratingPhase();
            return SetResults(oracle);
        }

        private void InitializeStrongly(SubmodularOracle oracle, double absEps, double relativeEps)
        {
            InitializeAlgorithm(oracle, absEps, relativeEps);
            contractOracle = new ContractOracle(oracle);
            this.oracle = contractOracle;

            var firstOrders = GetFirstOrders();
            SetFirstComponents(firstOrders);
            SetX();
        }

        private void DoIteratingPhase()
        {
            int maximizer; double eta; double fOfReachable;
            while (dataForContract.Remainder.Count > 0)
            {
                GetEta(out eta, out maximizer, out fOfReachable);
                if (eta <= 0)
                {
                    SetMinimizerForStrongly(dataForContract.Remainder,contractOracle);
                    break;
                }//if
                else
                {
                    FixisingPhase(maximizer, eta, fOfReachable);
                    N = oracle.N;
                }//else
            }//while
        }

        private void FixisingPhase(int maximizer, double eta, double fOfReachable)
        {
            if (3 * fOfReachable <= -eta)
            {
                Fix(dataForContract.ReducedVertices, eta, true);
                ReduceForStrongly();
                return;
            }//if
            double fOfAll = CalcFOfAll();
            if (3 * fOfAll >= eta)
            {
                Fix(dataForContract.DeletedVertices, eta, false);
                DeleteForStrongly();
                return;
            }//if
            Contract(maximizer, eta, Fix);
            return;
        }

        //[System.Diagnostics.Conditional("DEBUG")]
        //protected void CheckB()
        //{
        //    foreach (var component in components)
        //    {
        //        var b = new double[30];
        //        oracle.CalcBase(component.Order, b);
        //        foreach (var v in dataForContract.Remainder)
        //        {
        //            if (b[v] != component.B[v])
        //            {
        //                System.Diagnostics.Debugger.Break();
        //            }//if
        //        }//foreach v
        //    }//foreach component
        //}

        private void ModifyOrder(HashSet<int> tails)
        {
            int countContracted = 0;
            foreach (var v in tails)
            {
                if (dataForContract.Remainder.Contains(v))
                {
                    countContracted++;
                }//if
            }//foreach v
            foreach (var component in components)
            {
                int prevPos = 0;
                int suffPos = countContracted;
                var order = component.Order;
                for (int i = 0; i < N; i++)
                {
                    if (tails.Contains(order[i]))
                    {
                        dataForContract.OrderForCalc[prevPos++] = order[i];
                    }//if
                    else
                    {
                        dataForContract.OrderForCalc[suffPos++] = order[i];
                    }//else
                }//for i
                Copy(order, dataForContract.OrderForCalc, N);
            }//foreach component
        }

        //private void ReviveOrder(int prevCount, int nextCount, HashSet<int> tails)
        //{
        //    int diffCount = prevCount - nextCount;
        //    foreach (var component in components)
        //    {
        //        var order = component.Order;
        //        for (int i = 0; i < diffCount; i++)
        //        {
        //            dataForContract.OrderForCalc[i] = order[nextCount + i];
        //        }//for i
        //        for (int i = 0; i < nextCount; i++)
        //        {
        //            dataForContract.OrderForCalc[i + diffCount] = order[i];
        //        }//for i
        //        Copy(order, dataForContract.OrderForCalc, prevCount);
        //    }//foreach component
        //}

        private void Fix(HashSet<int> hash, double eta, bool contained)
        {
            double delta = eta;
            double endValue = eta / (3.0 * N * N * N);
            DoScalingPhase(ref delta, endValue);
            double threshold = delta * N * N;
            hash.Clear();
            foreach (var v in dataForContract.Remainder)
            {
                if ((contained && x[v] < -threshold) || (!contained && x[v] > threshold))
                {
                    hash.Add(v);
                }//if
            }//foreach v
        }

        private double CalcFOfReachable(int maximizer)
        {
            int pos = 0;
            int end = N - 1;
            foreach (var v in dataForContract.Remainder)
            {
                if (v == maximizer)
                {
                    continue;
                }//if
                if (dataForContract.Reachable[maximizer].Contains(v))
                {
                    dataForContract.OrderForCalc[pos++] = v;
                }//if
                else
                {
                    dataForContract.OrderForCalc[end--] = v;
                }//else
            }//foreach v
            dataForContract.OrderForCalc[pos] = maximizer;
            double fOfReachable = contractOracle.CalcValue(dataForContract.OrderForCalc, pos);
            return fOfReachable;
        }

        private double CalcFOfAll()
        {
            int pos = 0;
            foreach (var v in dataForContract.Remainder)
            {
                dataForContract.OrderForCalc[pos++] = v;
            }//foreach v
            contractOracle.CalcBase(dataForContract.OrderForCalc, dataForContract.BaseForCalc);
            double res = 0;
            foreach (var v in dataForContract.Remainder)
            {
                res += dataForContract.BaseForCalc[v];
            }//foreach v
            return res;
        }

        private void GetEta(out double eta, out int maximizer, out double fOfReachable)
        {
            eta = double.MinValue;
            maximizer = -1;
            fOfReachable = -1;
            foreach (var candMaximizer in dataForContract.Remainder)
            {
                int pos = 0;
                int end = dataForContract.Remainder.Count - 1;
                foreach (var v in dataForContract.Remainder)
                {
                    if (candMaximizer == v)
                    {
                        continue;
                    }
                    if (dataForContract.Reachable[candMaximizer].Contains(v))
                    {
                        dataForContract.OrderForCalc[pos++] = v;
                    }//if
                    else
                    {
                        dataForContract.OrderForCalc[end--] = v;
                    }//else
                }//foreach v
                dataForContract.OrderForCalc[pos] = candMaximizer;
                contractOracle.CalcBase(dataForContract.OrderForCalc, dataForContract.BaseForCalc);
                double curVal = dataForContract.BaseForCalc[dataForContract.OrderForCalc[pos]];
                if (curVal > eta)
                {
                    eta = curVal;
                    maximizer = candMaximizer;
                    fOfReachable = 0;
                    foreach (var v in dataForContract.Reachable[candMaximizer])
                    {
                        if (maximizer != v && dataForContract.Remainder.Contains(v))
                        {
                            fOfReachable += dataForContract.BaseForCalc[v];
                        }//if
                    }//foreach v
                }//if
            }//foreach u
        }

        #endregion

    }
}
