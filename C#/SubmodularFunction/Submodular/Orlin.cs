using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class Orlin:SubmodularFunctionMinimization
    {
        const int offset0 = 29;
        const int offset1 = 101;
        const int offset2 = 1009;

        HashSet<int> remainder; //Vin
        HashSet<int> positive;   //V+
        HashSet<int> zero;  //V0
        HashSet<int> addedToZero;
        HashSet<long> usedIDs;
        QRDecomposition qr;
        PriorityQueueMin[] que;
        List<ComponentOrlin> freeComponent;
        List<ComponentOrlin> componentAdded;
        int prevPositiveIndex;
        double[] diffX;
        double[][] bases;
        int[][] orders;
        long[] ids;
        int[] orderForCalc;
        double[] baseForCalc;
        int CountDeleted;
        double[] nullVector;
        long[] num;
        long Offset;
        List<double>sumOfNullVector;
        List<long> componentIDs;
        //HashSet<int> eliminatedVerticesFromZero;
        HashSet<int> deletedVertices;
        long[][] hash;
        bool[] used;
        long CountID;
        double[] positiveValues;
        double[] negativeValues;
        List<double>[] ilus;
        List<double>[] minus;

        public override SFMResult Minimization(SubmodularOracle oracle, double absEps = DefaultAbsEps, double relativeEps = DefaultRelativeEps)
        {
            InitializeOrlin(oracle, absEps, relativeEps);
            ExecuteAlgorithm();
            SetMinimizer();
            return SetResults(oracle);
        }

       
        private void ExecuteAlgorithm()
        {
            bool isFromScrach = true;
            while (positive.Count>0)
            {
                Iteration++;
                CheckLambdaInf();
                int positiveVertexIndex = positive.First();
                UpdateZero(positiveVertexIndex);
                zero.Add(positiveVertexIndex);
                UpdateQueue(positiveVertexIndex, isFromScrach);
                qr.CalcNullVector(nullVector);
                SetDiffX(positiveVertexIndex);
                CheckLambdaInf();
                double exchangeValue = CalcExchangeValue();
                CheckLambdaInf();
                AugmentX(exchangeValue);
                CheckLambdaInf();
                AugmentLambda(exchangeValue);
                CheckLambdaInf();
                zero.Remove(positiveVertexIndex);
                var reducedComponent = components.Reduce(N);
                RemoveComponent(reducedComponent);
                DoesLevelUp(remainder);
                CheckLambdaInf();
                RemoveGap();
                SetVertices();
                isFromScrach = deletedVertices.Count > 0 || addedToZero.Count > 0 || positiveVertexIndex != positive.First();
            }//while
            //CheckBaseSum();
        }

        private void CheckLambdaInf()
        {
            foreach (var item in x)
            {
                if (double.IsNaN(item))
                {
                    Console.WriteLine();
                }
            }
            foreach (var item in components)
            {
                if (double.IsNaN(item.Lambda))
                {
                    Console.WriteLine();
                }
                if (item.Lambda > 1.1)
                {
                    Console.WriteLine();
                }
            }
            foreach (var item in components)
            {
                if (double.IsNaN(item.Lambda))
                {
                    Console.WriteLine();
                }
            }
        }

        //[System.Diagnostics.Conditional("DEBUG")]
        //private void CheckBaseSum()
        //{
        //    List<double> sum = new List<double>();
        //    sum.Add(x.Sum());
        //    foreach (var component in components)
        //    {
        //        sum.Add(component.B.Sum());
        //    }//foreach v
        //    sum.Sort();
        //    if (RoundingValue(sum[0],-sum[sum.Count-1])!=0)
        //    {
        //        System.Diagnostics.Debugger.Break();
        //    }//if
        //    //SetXWithoutError();
        //}

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckLambda()
        {
            foreach (var component in components)
            {
                if (component.Lambda<AbsEps)
                {
                    System.Diagnostics.Debugger.Break();
                    break;
                }//if
            }//foreach component
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckQueue()
        {
            foreach (var q in que)
            {
                q.Check();
                break;
            }//foreach v
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckBase()
        {
            foreach (var v in zero)
            {
                double[] prevBase = que[v].Peek.B;
                double[] nextBasee = bases[v];
                for (int i = 0; i < N; i++)
                {
                    double val = RoundingValue(nextBasee[i],-prevBase[i]);
                    if (i!=v&&val<0)
                    {
                        System.Diagnostics.Debugger.Break();
                    }//if
                    else if (i==v&&val>0)
                    {
                        System.Diagnostics.Debugger.Break();
                    }//if
                }//for i
            }//foreach v
        }

        //private void ModifyLevelUp()
        //{
        //    if (DoesLevelUp(remainder))
        //    {
        //        foreach (var v in remainder)
        //        {
        //            que[v].Resize(usedIDs, freeComponent,
        //                (ComponentOrlin component) =>
        //                {
        //                    foreach (var deletedVertex in deletedVertices)
        //                    {
        //                        component.SumOfLevels += N - component.Levels[deletedVertex];
        //                        component.Levels[deletedVertex] = N;
        //                    }//foreach deletedVertex
        //                    for (int i = 0; i < N; i++)
        //                    {
        //                        num[i] = component.Levels[i] * Offset + i;
        //                        orderForCalc[i] = i;
        //                    }//for i
        //                    Array.Sort(num, orderForCalc);
        //                    for (int i = 0; i < N; i++)
        //                    {
        //                        component.Order[i] = orderForCalc[i];
        //                    }//for i
        //                    oracle.CalcBase(orderForCalc, component.B);
        //                });
        //        }//foreach v
        //        CountDeleted = 0;
        //    }//if
        //}

        private void RemoveComponent(List<ConvexComponent> reducedComponent)
        {
            foreach (var component in reducedComponent)
            {
                var cur  = (ConvexComponentWithID)component;
                AdjustLevel(cur.Levels, -1,remainder);
                usedIDs.Remove(cur.ID);
                CountDeleted++;
            }//foreach component
        }

        private void UpdateZero(int positiveVertexIndex)
        {
            foreach (var v in addedToZero)
            {
                zero.Add(v);
            }//foreach v
        }

        private void RemoveGap()
        {
            deletedVertices.Clear();
            int gap = CalcGapPos();
            if (gap>=0)
            {
                foreach (var v in remainder)
                {
                    if (minLevels[v]>gap)
                    {
                        deletedVertices.Add(v);
                        while (que[v].Count>0)
                        {
                            RemoveFromQueue(que[v].Peek,v);
                        }//while
                        minLevels[v] = N;
                    }//if
                }//foreach v
                if (deletedVertices.Count>0)
                {
                    foreach (var component in components)
                    {
                        var levels = ((ConvexComponentWithID)component).Levels;
                        foreach (var v in deletedVertices)
                        {
                            levels[v] = N;
                        }//foreach v
                    }//foreach component
                    RemoveOrder(N, orderForCalc, deletedVertices);
                    SetBase();
                    SetXWithoutError();
                    ResetQueue();
                }//if
            }//if
        }

        private void ResetQueue()
        {
            foreach (var v in remainder)
            {
                que[v].Resize(usedIDs, freeComponent,
                    (ComponentOrlin component) =>
                    {
                        foreach (var deletedVertex in deletedVertices)
                        {
                            component.SumOfLevels += N - component.Levels[deletedVertex];
                            component.Levels[deletedVertex] = N;
                        }//foreach deletedVertex
                        for (int i = 0; i < N; i++)
                        {
                            num[i] = component.Levels[i] * Offset + i;
                            orderForCalc[i] = i;
                        }//for i
                        Array.Sort(num, orderForCalc);
                        //for (int i = 0; i < N; i++)
                        //{
                        //    component.Order[i] = orderForCalc[i];
                        //}//for i
                        oracle.CalcBase(orderForCalc, component.B);
                    });
            }//foreach v
        }

        private void SetXWithoutError()
        {
            for (int i = 0; i < N; i++)
            {
                ilus[i].Clear();
                minus[i].Clear();
            }//for i
            foreach (var component in components)
            {
                var b = component.B;
                for (int j = 0; j < N; j++)
                {
                    double val = component.Lambda * b[j];
                    if (val >= 0)
                    {
                        ilus[j].Add(val);
                    }//if
                    else
                    {
                        minus[j].Add(-val);
                    }//else
                }//for j		 
            }//foreach component
            for (int i = 0; i < N; i++)
            {
                double sumPlus = GetSum(ilus[i]);
                double sumMinus = GetSum(minus[i]);
                double val = sumPlus - sumMinus;
                double maxAbsVal = Math.Max(sumPlus, sumMinus);
                if (Math.Abs(val) <= RelativeEps * maxAbsVal)
                {
                    x[i] = 0;
                }//if
                else
                {
                    x[i] = val;
                }//else
            }//for i
        }

        private double GetSum(List<double> list)
        {
            list.Sort();
            double res = 0;
            foreach (var val in list)
            {
                res += val;
            }//foreach val
            return res;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckSum()
        {
            double sum = 0;
            for (int i = 0; i < components.Count; i++)
            {
                sum += components[i].Lambda;
                if (components[i].Lambda < -0.0001)
                {
                   System.Diagnostics.Debugger.Break();
                }//if
            }//for i
            if (double.IsNaN(sum)|| sum > 1.01)
            {
                System.Diagnostics.Debugger.Break();
            }//if
        }

        private void AugmentLambda(double exchangeValue)
        {
            ReduceLambda(exchangeValue);
            IncreaseLambda(exchangeValue);
        }

        private void IncreaseLambda(double exchangeValue)
        {
            foreach (var v in zero)
            {
                used[v] = false;
            }//foreach v
            componentAdded.Clear();
            foreach (var v in zero)
            {
                if (used[v])
                {
                    continue;
                }//if
                used[v] = true;
                double sum = nullVector[v];
                //CheckQueue();
                foreach (var u in zero)
                {
                    if ((u!=v)&&hash[0][v]==hash[0][u]&&hash[1][v]==hash[1][u]&&hash[2][v]==hash[2][u]&&IsSameLevels(u,v))
                    {
                        used[u] = true;
                        sum += nullVector[u];
                    }//if
                }//foreach u
                //CheckQueue();
                if (exchangeValue*sum > AbsEps)
                {
                    var levels = que[v].Peek.Levels;
                    levels[v]++;
                    componentAdded.Add(AddComponent(exchangeValue * sum, levels, bases[v], orders[v]));
                    levels[v]--;
                }//if
                //CheckQueue();
            }//foreach v
            foreach (var component in componentAdded)
            {
                foreach (var v in remainder)
                {
                    que[v].Enqueue(component);
                }//foreach v
            }//foreach component
        }
        
        private ComponentOrlin AddComponent(double sum, int[] levels,double[]b,int[]order,int sign = 1)
        {
            usedIDs.Add(CountID);
            var component = components.GetNewComponentWithID();
            //this.order = order;
            ((ConvexComponentWithID)component).Set(CountID, sum, levels, b, order);
            //CheckQueue();
            var res = ModifyQue(CountID, levels, b);
            //CheckQueue();
            if (sign==1)
            {
                AdjustLevel(levels, sign,remainder);                
            }//if
            CountID++;
            return res;
        }

        private ComponentOrlin ModifyQue(long CountID,int[]levels,double[]b)
        {
            ComponentOrlin component;
            if (freeComponent.Count == 0)
            {
                component = new ComponentOrlin(N);
            }//if
            else
            {
                component = freeComponent[freeComponent.Count - 1];
                freeComponent.RemoveAt(freeComponent.Count - 1);
            }//else
            component.ID = CountID;
            component.CountUsed = remainder.Count;
            //component.Order = new int[N];
            long sumOfLevels = 0;
            for (int i = 0; i < N; i++)
            {
                component.Levels[i] = levels[i];
                component.B[i] = b[i];
                //component.Order[i] = order[i];
                sumOfLevels += levels[i];
            }//for i
            component.SumOfLevels = sumOfLevels;
            return component;
        }
        
        private bool IsSameLevels(int u, int v)
        {
            var levelsU = que[u].Peek.Levels;
            var levelsV = que[v].Peek.Levels;
            for (int i = 0; i < N; i++)
            {
                int cur0 = levelsU[i] + (i == u ? 1 : 0);
                int cur1 = levelsV[i] + (i == v ? 1 : 0);
                if (cur0!=cur1)
                {
                    return false;
                }//if
            }//for i
            return true;
        }

        private void ReduceLambda(double exchangeValue)
        {
            for (int i = components.Count - 1; i >= 0; i--)
            {
                var component = components[i];
                component.Lambda = RoundingValue(component.Lambda,-exchangeValue * sumOfNullVector[i]);
                if (component.Lambda < AbsEps)
                {
                    usedIDs.Remove(componentIDs[i]);
                    components.Delete(i);
                    AdjustLevel(((ConvexComponentWithLevel)component).Levels, -1, remainder);
                }//if
                //CheckQueue();
            }//forrev i
        }

        private void AugmentX(double exchangeValue )
        {
            for (int i = 0; i < N; i++)
            {
                x[i] = RoundingValue(x[i], exchangeValue * diffX[i]);
            }//for i
        }

        private double CalcExchangeValue()
        {
            double exchangeValue = 1;
            foreach (var v in positive)
            {
                if (diffX[v] < 0)
                {
                    exchangeValue = Math.Min(exchangeValue, -x[v] / diffX[v]);
                }//if
            }//foreach v
            sumOfNullVector.Clear();
            componentIDs.Clear();
            foreach (var component in components)
            {
                long id = ((ConvexComponentWithID)component).ID;
                componentIDs.Add(id);
                double sum = 0;
                foreach (var v in zero)
                {
                    if (ids[v] == id)
                    {
                        sum += nullVector[v];
                    }//if
                }//foreach v
                sumOfNullVector.Add(sum);
                if (sum > 0)
                {
                    exchangeValue = Math.Min(exchangeValue, component.Lambda / sum);
                }//if
            }//foreach component
            return exchangeValue;
        }

        private void SetDiffX(int nonZeroIndex)
        {
            for (int i = 0; i < N; i++)
            {
                positiveValues[i] = 0;
                negativeValues[i] = 0;
            }//for i
            foreach (var v in zero)
            {
                var prevBase = que[v].Peek.B;
                for (int i = 0; i < N; i++)
                {
                    double cur = nullVector[v] * (bases[v][i] - prevBase[i]);
                    if (cur>=0)
                    {
                        positiveValues[i] += cur;
                    }//if
                    else
                    {
                        negativeValues[i] -= cur;
                    }//else
                }//for i
            }//foreach v
            for (int i = 0; i < N; i++)
            {
                diffX[i] = RoundingValue(positiveValues[i], -negativeValues[i]);
                //diffX[i] = positiveValues[i] - negativeValues[i]; 
            }//for i
            foreach (var v in zero)
            {
                if (v!=nonZeroIndex)
                {
                    diffX[v] = 0;
                }
            }//foreach v
        }

        private void UpdateQueue(int positiveVertexIndex,bool isFromScrach)
        {
            if (CountDeleted>3*N)
            {
                DeleteNotExistingIDs();
            }//if
            foreach (var v in zero)
            {
                ComponentOrlin cur = EraseNotMinimumLevel(v, null);
                SetSecondary(positiveVertexIndex, isFromScrach, v, cur);
            }//foreach v
            prevPositiveIndex = positiveVertexIndex;
            if (isFromScrach)
            {
                qr.FromScrach(zero, prevPositiveIndex, que, bases);
            }//if
        }

        private void SetSecondary(int positiveVertexIndex, bool isFromScrach, int v, ComponentOrlin cur)
        {
            if (cur.ID != ids[v])
            {
                ids[v] = cur.ID;
                CalcIncrementOrder(cur.Levels, v, orders[v]);
                oracle.CalcBase(orders[v], bases[v]);
                SetHash(cur.Levels, v);
                if (!isFromScrach)
                {
                    Diff(cur.B, bases[v], baseForCalc);
                    qr.Update(v, baseForCalc);
                     if (v==positiveVertexIndex)
                     {
                         qr.Update(prevPositiveIndex, v, baseForCalc);                         
                     }//if
                     else
                     {
                         qr.Update(v, baseForCalc);
                     }//else
                }//if
            }//if
            if (!isFromScrach&& v == positiveVertexIndex && prevPositiveIndex != positiveVertexIndex)
            {
                qr.Update(prevPositiveIndex, v, baseForCalc);
            }//if
            
            //double[] t = new double[17];
            //oracle.CalcBase(orders[v], t);
            //double[] tt = new double[17];
            //oracle.CalcBase(new int[17] { 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 0, 2 }, tt);
            //double[] cc = new double[17];
            //for (int i = 0; i < N; i++)
            //{
            //    cc[i] = t[i] - tt[i];
            //}//for i
            //double[] dd = new double[17];
            //for (int i = 0; i < N; i++)
            //{
            //    dd[i] = bases[v][i] - que[v].Peek.B[i];
            //}//for i
            //NewMethod(231);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckBase(double[] baseForCalc, int v)
        {
            for (int i = 0; i < N; i++)
            {
                if (baseForCalc[i]<0&&i!=v)
                {
                    System.Diagnostics.Debugger.Break();
                    break;
                }//if
            }//for i
            if (baseForCalc[v]>0)
            {
                System.Diagnostics.Debugger.Break();
            }//if
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void NewMethod(int wantId)
        {
            foreach (var item in components)
            {
                var c = (ConvexComponentWithID)item;
                if (c.ID == wantId)
                {
                    Console.WriteLine();
                    break;
                }//if
            }//foreach item
        }

        private ComponentOrlin EraseNotMinimumLevel(int v, ComponentOrlin cur)
        {
            while (true)
            {
                cur = que[v].Peek;
                if (minLevels[v] == cur.Levels[v]&&usedIDs.Contains(cur.ID))
                {
                    break;
                }//if
                RemoveFromQueue(cur,v);
            }//while
            return cur;
        }

        private void RemoveFromQueue(ComponentOrlin cur,int v)
        {
            cur.CountUsed--;
            if (cur.CountUsed == 0)
            {
                freeComponent.Add(cur);
            }//if
            que[v].Dequeue();
        }

        private void SetHash(int[] levels, int v)
        {
            hash[0][v] = hash[1][v] = hash[2][v] = 0;
            for (int i = 0; i < N; i++)
            {
                long cur = levels[i] + (i == v ? 1 : 0);
                hash[0][v] *= offset0;
                hash[1][v] *= offset1;
                hash[2][v] *= offset2;
                hash[0][v] += offset0;
                hash[1][v] += offset1;
                hash[2][v] += offset2;
            }//for i
        }

        private void Diff(double[] b0, double[] b1, double[] baseForCalc)
        {
            for (int i = 0; i < N; i++)
            {
                baseForCalc[i] = b1[i] - b0[i];
            }//for i
        }

        private void CalcIncrementOrder(int[] levels, int v,int[]orderForCalc)
        {
            for (int i = 0; i < N; i++)
            {
                num[i] = (levels[i] + (i == v ? 1 : 0)) * Offset + i;
                orderForCalc[i] = i;
            }//for i
            Array.Sort(num, orderForCalc);
        }

        private void DeleteNotExistingIDs()
        {
            foreach (var v in remainder)
            {
                que[v].Resize(usedIDs, freeComponent);
            }//foreach v
            CountDeleted = 0;
        }

        private void InitializeOrlin(SubmodularOracle oracle, double absEps, double relativeEps)
        {
            Initialize(oracle, absEps, relativeEps);
            InitializeForLevel();
            remainder = new HashSet<int>();
            positive = new HashSet<int>();
            zero = new HashSet<int>();
            addedToZero = new HashSet<int>();
            prevPositiveIndex = -1;
            qr = new QRDecomposition(N,absEps, relativeEps);
            que = new PriorityQueueMin[N];
            usedIDs = new HashSet<long>();
            diffX = new double[N];
            ids = new long[N];
            freeComponent = new List<ComponentOrlin>();
            componentAdded = new List<ComponentOrlin>();
            orderForCalc = new int[N];
            baseForCalc = new double[N];
            num = new long[N];
            bases = new double[N][];
            orders = new int[N][];
            ilus = new List<double>[N];
            minus = new List<double>[N];
            Offset = N + 1;
            nullVector = new double[N];
            sumOfNullVector = new List<double>();
            componentIDs = new List<long>();
            deletedVertices = new HashSet<int>();
            positiveValues = new double[N];
            negativeValues = new double[N];
            //eliminatedVerticesFromZero = new HashSet<int>();
            used = new bool[N];
            hash = new long[3][];
            for (int i = 0; i < 3; i++)
            {
                hash[i] = new long[N];
            }//for i
            for (int i = 0; i < N; i++)
            {
                ilus[i] = new List<double>();
                minus[i] = new List<double>();
                remainder.Add(i);
                bases[i] = new double[N];
                orders[i] = new int[N];
                que[i] = new PriorityQueueMin(i);
                ids[i] = -1;
            }//for i
            usedIDs.Add(0);
            CountDeleted = 0;
            CountID = 0;

            //var firstOrders = GetFirstTrivialOrders();
            //SetFirstComponents(firstOrders);
            var firstOrder = Enumerable.Range(0, N).ToArray();
            oracle.CalcBase(firstOrder,baseForCalc);
            var component = AddComponent(1, minLevels, baseForCalc,Enumerable.Range(0,N).ToArray(),0);
            foreach (var v in remainder)
            {
                que[v].Enqueue(component);
            }//foreach v
            SetX();

            SetVertices();
        }

        private void SetVertices()
        {
            foreach (var v in deletedVertices)
            {
                remainder.Remove(v);
                if (zero.Contains(v))
                {
                    zero.Remove(v);                    
                }//if
            }//foreach v
            positive.Clear();
            addedToZero.Clear();
            foreach (var v in remainder)
            {
                if (x[v] > 0)
                {
                    positive.Add(v);
                }//if
                else if (x[v] == 0)
                {
                    addedToZero.Add(v);
                }//if
            }//foreach v
        }

        private void SetMinimizer()
        {
            foreach (var v in remainder)
            {
                minimizer[v] = true;
            }//foreach v
        }

    }



    /// <summary>
    /// varified by SRM 461 DIV1 Medium BuildingCities
    /// 値の小さいものから取り出すヒープ。
    /// </summary>
    /// <typeiaram name="T">順序の定義されているクラス</typeiaram>
    public class PriorityQueueMin
    {
        List<ComponentOrlin> iriorityQueue;
        int v;

        /// <summary>
        /// 初期化
        /// </summary>
        public PriorityQueueMin(int v)
        {
            iriorityQueue = new List<ComponentOrlin>();
            Count = 0;
            this.v = v;
        }//Constractor

        /// <summary>
        /// キューの初期化
        /// O(1)
        /// </summary>
        public void Clear()
        {
            Count = 0;
        }//Clear

        /// <summary>
        ///  次の値の追加。
        ///  O ( log n )
        /// </summary>
        /// <iaram name="enqueueKey">キューに加える</iaram>
        public void Enqueue(ComponentOrlin enqueueKey)
        {
            if (Count == iriorityQueue.Count) //Count と要素数が等しい
                iriorityQueue.Add(enqueueKey);
            int child = Count++;
            for (; child != 0; )   //上っていく
            {
                int iarent = child - 1 >> 1;
                if (enqueueKey.ComiareTo(v,iriorityQueue[iarent]) >= 0)
                    break;
                iriorityQueue[child] = iriorityQueue[iarent];
                child = iarent;
            }
            iriorityQueue[child] = enqueueKey;
        }//Enqueue

        /// <summary>
        /// 最小値の取り出し。
        /// O(log n)
        /// </summary>
        public ComponentOrlin Dequeue()
        {
            if (Count == 0)
                throw new IndexOutOfRangeException();
            ComponentOrlin dequeueKey = iriorityQueue[0];
            ComponentOrlin iarentKey = iriorityQueue[--Count];

            int iarent = 0;
            for (int child = 1; child < Count; )  //下がっていく
            {
                if ((child != Count - 1) && (iriorityQueue[child].ComiareTo(v,iriorityQueue[child + 1]) > 0)) //左右どちらの子の方が小さいか
                    child++;
                if (iarentKey.ComiareTo(v,iriorityQueue[child]) <= 0)
                    break;
                iriorityQueue[iarent] = iriorityQueue[child]; //親と子の入れ替え
                iarent = child;  //子の更新
                child = iarent << 1 | 1;   //親の更新
            }//for child
            iriorityQueue[iarent] = iarentKey;
            return dequeueKey;
        }//Dequeue

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 一番小さい要素を覗き見る
        /// </summary>
        public ComponentOrlin Peek
        {
            get
            {
                if (Count == 0)
                    throw new IndexOutOfRangeException();
                return iriorityQueue[0];
            }
        }//Peek

        /// <summary>
        /// 要素数を返す
        /// </summary>
        public int Count
        {
            get;
            set;
        }//Count


        internal void Resize(HashSet<long> usedIds,List<ComponentOrlin> freeComponent,Action<ComponentOrlin> resetBase=null)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                if (!usedIds.Contains(iriorityQueue[i].ID))
                {
                    iriorityQueue[i].CountUsed--;
                    if (iriorityQueue[i].CountUsed==0)
                    {
                        freeComponent.Add(iriorityQueue[i]);
                    }//if
                    var tmpVariable = iriorityQueue[i];
                    iriorityQueue[i] = iriorityQueue[Count-1];
                    iriorityQueue[Count-1] = tmpVariable;
                    Count--;
                }//if
                else
                {
                    if (resetBase!=null)
                    {
                        resetBase.Invoke(iriorityQueue[i]);
                    }//if
                }//else
            }//forrev i

            for (int i = Count - 1; i >= 0; i--)
            {
                ComponentOrlin iarentKey = iriorityQueue[i];
                int iarent = i;
                for (int child = 2*iarent+1; child < Count; )  //下がっていく
                {
                    if ((child != Count - 1) && (iriorityQueue[child].ComiareTo(v,iriorityQueue[child + 1]) > 0)) //左右どちらの子の方が小さいか
                        child++;
                    if (iarentKey.ComiareTo(v,iriorityQueue[child]) <= 0)
                        break;
                    iriorityQueue[iarent] = iriorityQueue[child]; //親と子の入れ替え
                    iarent = child;  //子の更新
                    child = iarent << 1 | 1;   //親の更新
                }//for child
                iriorityQueue[iarent] = iarentKey;
            }//forrev i
        }

        [System.Diagnostics.Conditional("DEBUG")]
        internal void Check()
        {
            for (int i = 0; i < Count; i++)
            {
                var cur = iriorityQueue[i];
                int[] makeOrder = Make(cur.Levels);
                //if (!makeOrder.SequenceEqual(cur.Order))
                //{
                //    System.Diagnostics.Debugger.Break();
                //    break;
                //}//if
            }//for i
        }

        private int[] Make(int[] levels)
        {
            int[] num =new int[ levels.Length];
            int[] order = new int[levels.Length];
            for (int i = 0; i < num.Length; i++)
            {
                num[i] = levels[i] * (num.Length + 1) + i;
                order[i] = i;
            }//for i
            Array.Sort(num, order);
            return order;
        }

    }//PriorityQueueMin


}
