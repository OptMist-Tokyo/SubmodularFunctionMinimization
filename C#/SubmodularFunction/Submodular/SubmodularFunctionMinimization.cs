using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public abstract class SubmodularFunctionMinimization
    {

        #region Common
        protected const double DefaultAbsEps = 1e-10;
        protected const double DefaultRelativeEps = 1e-10;
        protected const double DefaultPrecision = 1;
        protected SubmodularOracle oracle;
        SFMResult result;
        Stopwatch timer;
        protected ConvexComponents components;
        protected bool[] minimizer;   //for calculation
        protected double[] x;

        protected SubmodularFunctionMinimization()
        {
            result = new SFMResult(null);
            timer = new Stopwatch();
        }

        protected void Initialize(SubmodularOracle oracle,double absEps,double relativeEps)
        {
            oracle.Clear();
            this.oracle = oracle;
            result = new SFMResult(oracle);
            N = oracle.N;
            timer.Restart();
            components = new ConvexComponents(N, absEps, relativeEps);
            x = new double[N];
            minimizer = new bool[N];
            this.AbsEps = absEps;
            this.RelativeEps = relativeEps;
            this.Iteration = 0;
        }

        public abstract SFMResult Minimization(SubmodularOracle oracle, double absEps =DefaultAbsEps, double relativeEps = DefaultRelativeEps);


        protected int N
        {
            get;
            set;
        }

        protected SFMResult Result
        {
            get { return result; }
        }

        protected double AbsEps
        {
            get;
            private set;
        }

        protected double RelativeEps
        {
            get;
            private set;
        }

        protected long Iteration
        {
            get;
            set;
        }


         protected double RoundingValue(double val0, double val1)
        {
            double sum = val0 + val1;
            double absSum = Math.Abs(sum);
            double maxAbsValue = Math.Max(Math.Abs(val0), Math.Abs(val1));
            if (absSum<AbsEps||absSum<maxAbsValue*RelativeEps)
            {
                return 0;
            }//if
            return sum;
        }


        //protected double CalcValue(int[] order, int cardinality)
        //{
        //    return oracle.CalcValue(order, cardinality);
        //    result.StartOracle();
        //    var res = oracle.CalcValue(order, cardinality);
        //    result.StopOracle();
        //    return res;
        //}

        //protected void CalcBase(int[]order,double[]b)
        //{
        //    result.StartBase();
        //    oracle.CalcBase(order,b);
        //    result.StopBase();
        //}

        //protected void SetOracle(SubmodularOracle oracle)
        //{
        //    this.oracle = oracle;
        //    result = new SFMResult(oracle.N);
        //}

        //protected void SetResult(double[]x,long iteration)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < N; i++)
        //    {
        //        sb.Append(x[i]<=0?"1":"0");
        //    }//for i
        //    string minimizer = sb.ToString();
        //    SetResult(x,minimizer,iteration);
        //}

        protected void SetX()
        {
            bool first = true;
            foreach (var component in components)
            {
                var order = component.Order;
                for (int i = 0; i < N; i++)
                {
                    int cur = order[i];
                    if (first)
                    {
                        x[cur] = 0;
                    }//if
                    x[cur] += component.Lambda * component.B[cur];
                }//for i
                first = false;
            }//foreach component
        }

        protected List<int[]> GetFirstTrivialOrders()
        {
            List<int[]> list = new List<int[]>();
            list.Add(Enumerable.Range(0, N).ToArray());
            return list;
        }

        protected virtual ConvexComponent GetNewComponent()
        {
            return components.GetNewComponent();
        }

        protected virtual void ActForNewConvexComponent(ConvexComponent component)
        {
        }

        protected void SetFirstComponents(List<int[]> firstOrders)
        {
            var path = @"E:\seed.txt";
            var sr = new System.IO.StreamReader(path);
            var tmp = int.Parse(sr.ReadLine());
            sr.Close();
            var sw = new System.IO.StreamWriter(path);
            sw.WriteLine((tmp + 1) % 25);
            sw.Close();
            var seq = RandomSequence(firstOrders[0].Length, tmp);
            for (int i = 0; i < seq.Length; i++)
            {
                firstOrders[0][i] = seq[i];
            }


            foreach (var order in firstOrders)
            {
                var component = GetNewComponent();
                oracle.CalcBase(order, component.B);
                component.Lambda = 1.0 / firstOrders.Count;
                for (int i = 0; i < order.Length; i++)
                {
                    component.Order[i] = order[i];
                }//for i
                ActForNewConvexComponent(component);
            }//foreach order
        }

        /// <summary>
        /// 長さ n の乱数列の生成
        /// 配列の中身は 0 ～ n-1 までの自然数
        /// O( n )
        /// </summary>
        /// <param name="n">配列の長さ</param>
        /// <param name="seed">乱数のシード</param>
        /// <returns>長さ n の乱数列</returns>
        public static int[] RandomSequence(int n, int seed)
        {
            Random random = new Random(seed);
            int[] seq = new int[n];

            int pos;
            for (int i = 0; i < n; i++)
            {
                pos = random.Next(i + 1);
                seq[i] = seq[pos];
                seq[pos] = i;
            }
            return seq;
        }//RandomSequence

         protected double GetMinimumValue(string minimizer)
         {
             int usedBit;
             var order = SubmodularOracle.GetOrder(minimizer, out usedBit);
             double minimumValue = oracle.Value(order, usedBit);
             return minimumValue;
         }

         protected SFMResult SetResults(SubmodularOracle oracle)
         {
             string minimizer = oracle.GetMinimizer(this.minimizer);
             double minValue = oracle.CalcValue(this.minimizer);
             SetResult(x, minimizer, minValue);
             return Result;
         }

        private void SetResult(double[]x,string minimizer,double minimumValue)
        {
            timer.Stop();
            result.SetResult(x, minimizer, minimumValue, Iteration, timer.ElapsedMilliseconds,components,oracle);
        }

        protected void RemoveOrder(int prevCount,int[]orderForCalc, HashSet<int> removeHash)
        {
            foreach (var component in components)
            {
                int pos = 0;
                int removedPos = prevCount - removeHash.Count;
                var order = component.Order;
                for (int i = 0; i < prevCount; i++)
                {
                    if (removeHash.Contains(order[i]))
                    {
                        orderForCalc[removedPos++] = order[i];
                    }//if
                    else
                    {
                        orderForCalc[pos++] = order[i];
                    }//else
                }//for i
                Copy(order, orderForCalc, prevCount);
            }//foreach component
        }

        protected void Copy(int[] order, int[] orderForCalc, int Count)
        {
            for (int i = 0; i < Count; i++)
            {
                order[i] = orderForCalc[i];
            }//for i
        }

        protected void SetBaseAndX()
        {
            foreach (var component in components)
            {
                oracle.CalcBase(component.Order, component.B);
            }//foreach component
            SetX();
        }

        protected void SetBase()
        {
            foreach (var component in components)
            {
                oracle.CalcBase(component.Order, component.B);
            }//foreach component
        }
        
        protected void SetMinimizerForWeakly(HashSet<int> reachable)
        {
            foreach (var v in reachable)
            {
                minimizer[v] = true;
            }//foreach item
        }

        protected void SetMinimizerForStrongly(HashSet<int> remainder,ContractOracle contractOracle)
        {
            foreach (var contractedV in remainder)
            {
                foreach (var v in contractOracle[contractedV])
                {
                    minimizer[v] = true;
                }//foreach item
            }//foreach v
        }

        #endregion 

        #region strongly

        protected ContractOracle contractOracle;
        protected DataForContract dataForContract;

        protected void ReduceForStrongly()
        {
            Remove(dataForContract.ReducedVertices, (v) =>
            {
                foreach (var u in contractOracle[v])
                {
                    minimizer[u] = true;
                }//foreach item
                contractOracle.Reduce(v);
            });
        }

        protected void DeleteForStrongly()
        {
            Remove(dataForContract.DeletedVertices, (v) => contractOracle.Delete(v));
        }

        private void Remove(HashSet<int> hash, Action<int> actionForOracle)
        {
            int prevCount = dataForContract.Remainder.Count;
            foreach (var v in hash)
            {
                actionForOracle.Invoke(v);
                dataForContract.Remainder.Remove(v);
                dataForContract.Reachable.Remove(v);
            }//foreach v
            RemoveOrder(prevCount, dataForContract.OrderForCalc, hash);
            SetBaseAndX();
        }

        protected void Contract(int v, double eta,Action<HashSet<int>,double ,bool> getTails)
        {
            int memoCountContracted;
            double memoReducedFOfEmpty;
            int prevCount = N;
            contractOracle.Reduce(dataForContract.Reachable[v], out memoCountContracted, out memoReducedFOfEmpty);
            N = oracle.N;
            RemoveOrder(prevCount, dataForContract.OrderForCalc, dataForContract.Reachable[v]);
            SetBaseAndX();
            getTails.Invoke(dataForContract.Tails, eta, true);
            contractOracle.ReviveBeforeReducing(dataForContract.Reachable[v], memoCountContracted, memoReducedFOfEmpty);
            N = prevCount;
            RemoveWhichDoesNotExist(dataForContract.Tails);
            Contract(v);
            SetConsistentOrder();
            SetBaseAndX();
        }

        private void RemoveWhichDoesNotExist(HashSet<int> tails)
        {
            var list = dataForContract.DoesNotExist;
            list.Clear();
            foreach (var v in tails)
            {
                if (!dataForContract.Remainder.Contains(v))
                {
                    list.Add(v);
                }//if
            }//foreach v
            foreach (var v in list)
            {
                tails.Remove(v);
            }//foreach v
        }

        private void SetConsistentOrder()
        {
            dataForContract.TS.Clear(N);
            int pos = 0;
            foreach (var v in dataForContract.Remainder)
            {
                dataForContract.HashForCalc[v] = pos++;
            }//foreach v
            foreach (var v in dataForContract.Remainder)
            {
                foreach (var next in dataForContract.Reachable[v])
                {
                    if (v != next && dataForContract.Remainder.Contains(next))
                    {
                        dataForContract.TS.AddEdge(dataForContract.HashForCalc[v], dataForContract.HashForCalc[next]);
                    }//if
                }//foreach next
            }//foreach v
            dataForContract.TS.SortToiologically();
            //if (!ts.SortToiologically())
            //{
            //    Console.WriteLine();
            //}
            var tsOrder = dataForContract.TS.toiologicalSort;
            for (int i = 0; i < N; i++)
            {
                dataForContract.HashForCalc[tsOrder[i]] = i;
            }//for i
            components.Clear();
            var component = components.GetNewComponent();
            component.Lambda = 1;
            var newOrder = component.Order;
            pos = 0;
            foreach (var v in dataForContract.Remainder)
            {
                newOrder[dataForContract.HashForCalc[pos++]] = v;
            }//foreach v
        }

        private void Contract(int head)
        {
            foreach (var tail in dataForContract.Tails)
            {
                if (tail != head&&dataForContract.Remainder.Contains(tail))
                {
                    AddEdge(head, tail);
                }//if
            }//foreach v
        }

        private void AddEdge(int head, int tail)
        {
            if (dataForContract.Reachable[tail].Contains(head))
            {
                Contract(head, tail);
            }//if
            else
            {
                UpdateReachable(head, tail);
            }//else
        }

        private void Contract(int head, int tail)
        {
            CalcContractVertex(head, tail);
            int prevCount = dataForContract.Remainder.Count;
            contractOracle.Contract(dataForContract.ContractedVertices, head);
            foreach (var cur in dataForContract.ContractedVertices)
            {
                if (cur == head)
                {
                    continue;
                }//if
                dataForContract.Remainder.Remove(cur);
                dataForContract.Reachable.Remove(cur);
            }//foreach cur
            //RemoveOrder(prevCount, dataForContract.OrderForCalc, dataForContract.ContractedVertices);
        }

        private void CalcContractVertex(int head, int tail)
        {
            dataForContract.ContractedVertices.Clear();
            foreach (var midPoint in dataForContract.Reachable[tail])
            {
                if (dataForContract.Reachable.ContainsKey(midPoint) && dataForContract.Reachable[midPoint].Contains(head) && dataForContract.Remainder.Contains(midPoint))
                {
                    dataForContract.ContractedVertices.Add(midPoint);
                }//if
            }//foreach item
        }

        private void UpdateReachable(int head, int tail)
        {
            foreach (var v in dataForContract.Remainder)
            {
                if (dataForContract.Reachable[v].Contains(head))
                {
                    foreach (var reachableVertex in dataForContract.Reachable[tail])
                    {
                        dataForContract.Reachable[v].Add(reachableVertex);
                    }//foreach reachableV
                }//if
            }//foreach v
        }

        #endregion

        #region level

        protected int[] minLevels;    //d
        int[][] cntLevels;
        int[] cntForGap;  //for calculation


        protected void InitializeForLevel()
        {
            minLevels = new int[N];
            cntLevels = new int[N][];
            cntForGap = new int[N + 1];
            for (int i = 0; i < N; i++)
            {
                cntLevels[i] = new int[2];
                cntLevels[i][0] = 1;
            }//for i
        }

        protected void ResetLevels()
        {
            foreach (var v in dataForContract.Remainder)
            {
                minLevels[v] = 0;
                cntLevels[v][0] = 1;
                cntLevels[v][1] = 0;
            }//foreach v
        }

        protected bool DoesLevelUp(HashSet<int>remainder)
        {
            bool res = false;
            foreach (var v in remainder)
            {
                if (cntLevels[v][0] == 0 && cntLevels[v][1] > 0)
                {
                    cntLevels[v][0] = cntLevels[v][1];
                    cntLevels[v][1] = 0;
                    minLevels[v]++;
                    res = true;
                }//if
            }//foreach v
            return res;
        }

        protected void AdjustLevel(int[] levels, int sign,HashSet<int> remainder)
        {
            foreach (var v in remainder)
            {
                if (levels[v] == minLevels[v])
                {
                    cntLevels[v][0] += sign;
                }//if
                else if (levels[v] == minLevels[v] + 1)
                {
                    cntLevels[v][1] += sign;
                }//if
                else if (levels[v] - 1 == minLevels[v])
                {
                    cntLevels[v][1] = cntLevels[v][0];
                    cntLevels[v][0] += sign;
                }//if
                else
                {
                    throw new Exception("Procedure for gap is wrong.");
                }//else
            }//foreach v
        }
        
        protected int CalcGapPos()
        {
            for (int i = 0; i <= N; i++)
            {
                cntForGap[i] = 0;
            }//for i
            var order = components[0].Order;
            for (int i = 0; i < N; i++)
            {
                int cur = order[i];
                cntForGap[minLevels[cur]]++;
            }//for i
            int gap = -1;
            for (int i = 0; i < N; i++)
            {
                if (cntForGap[i] == 0 && cntForGap[i + 1] > 0)
                {
                    gap = i;
                    break;
                }//if
            }//for i
            return gap;
        }

        
        #endregion

    }
}
