//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Onigiri.FW
//{
//    public class FW
//    {
//        const double AbsEps = 1e-12;
//        const double RelativeEps = 1e-10;
//        const int All = 0;
//        const int Head = 1;
//        const int Tail = 2;
//        const double RepRatio = 0.02;
//        const double CutRatio = 0.35;
//        const int CutSize = 10;

//        double currentNorm;
//        ConvexComponentsQR components;
//        double[] extremeBase;
//        double[] x;
//        double[] tmpX;
//        List<int> reduceList;
//        List<int> deleteList;
//        List<int> list;
//        HashSet<int> hash;
//        int[] cnt;
//        int[] diffs;
//        int[] order;
//        int[][] data;
//        List<int> componentsReduceIndex;

//        public double Minimization(SubmodularOracle oracle)
//        {
//            double e = oracle.EmptyValue;
//            double m = oracle.FullValue;
//            //long iteration =0;
//            Initialize(oracle);
//            var timer = new Stopwatch();
//            //ModifyOracle(oracle);
//            timer.Start();

//            CalcMinimumNormPoint(oracle);

//            timer.Stop();
//            list.Clear();
//            for (int i = 0; i < components.X.Length; i++)
//            {
//                if (components.X[i] < 0)
//                {
//                    list.Add(i);
//                }
//            }
//            oracle.Reduce(list);
//            double val = oracle.EmptyValue;
//            var sw = new StreamWriter(@"C:\Users\onigiri\Desktop\output.txt", true);
//            sw.WriteLine(timer.ElapsedMilliseconds + " " + val);
//            //sw.WriteLine(new string(oracle.Minimizer.Select(x => x ? '1' : '0').ToArray()));
//            sw.Close();
//            Console.WriteLine(timer.ElapsedMilliseconds + " " + val);
//            return val;
//        }

//        //private void Add(SubmodularOracle b,SubmodularOracle oracle, List<int> modified, double[] x, int[] reorder, Func<double, bool> func)
//        //{
//        //    for (int i = 0; i < reorder.Length; i++)
//        //    {
//        //        if (reorder[i] != -1)
//        //        {
//        //            inverse[reorder[i]] = i;
//        //        }
//        //    }
//        //    for (int i = 0; i < x.Length; i++)
//        //    {
//        //        if (func.Invoke(x[i]))
//        //        {
//        //            modified.Add(inverse[i]);
//        //        }
//        //    }

//        //    //var hash = new HashSet<int>();
//        //    //for (int i = 0; i < x.Length; i++)
//        //    //{
//        //    //    if (func.Invoke(x[i]))
//        //    //    {
//        //    //        hash.Add(oracle.remainder[i]);
//        //    //    }
//        //    //}
//        //    //for (int i = 0; i < b.remainder.Length; i++)
//        //    //{
//        //    //    if (hash.Contains(b.remainder[i]))
//        //    //    {
//        //    //        modified.Add(i);
//        //    //    }
//        //    //}



//        //}

//        private void Resize(SubmodularOracle oracle, int[] reorder)
//        {
//            oracle.SetRemainder(reduceList, deleteList, reorder);
//            components.Resize(oracle.Count);
//            Array.Resize(ref extremeBase, oracle.Count);
//            Array.Resize(ref x, oracle.Count);
//        }

//        //private void ResizeSmall(int n)
//        //{
//        //    smallComponents.Resize(n);
//        //    Array.Resize(ref smallExtremeBase, n);
//        //    Array.Resize(ref smallX, n);
//        //    Array.Resize(ref smallRemainder, n);
//        //}

//        //private void ResizeBig(int state,SubmodularOracle oracle,int[]reorder=null)
//        //{
//        //    if (state == All)
//        //    {
//        //        oracle.SetRemainder(reduceList, deleteList, reorder);
//        //        components.Resize(oracle.Count);
//        //        Array.Resize(ref extremeBase, oracle.Count);
//        //        Array.Resize(ref x, oracle.Count);
//        //    }
//        //    else
//        //    {
//        //        oracle.SetRemainder(reduceList, deleteList, reorder);
//        //        smallComponents.Resize(oracle.Count);
//        //        Array.Resize(ref smallExtremeBase, oracle.Count);
//        //        Array.Resize(ref smallX, oracle.Count);
//        //    }
//        //}

//        //private void SectionReduce(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
//        //{
//        //    for (int i = 0; i < cnt.Length; i++)
//        //    {
//        //        cnt[i] = 0;
//        //    }
//        //    int reduceCount = 0;
//        //    for (int i = 0; i < x.Length; i++)
//        //    {
//        //        if (x[i] > 0)
//        //        {
//        //            break;
//        //        }
//        //        for (int j = 0; j < bigComponents.Count; j++)
//        //        {
//        //            cnt[data[j][i]]++;
//        //        }
//        //        var flg = true;
//        //        for (int j = reduceCount; j <= i && flg; j++)
//        //        {
//        //            flg &= (cnt[order[j]] == bigComponents.Count);
//        //        }
//        //        if (flg)
//        //        {
//        //            reduceCount = i + 1;
//        //        }
//        //    }
//        //    reduceList.Clear();
//        //    for (int i = 0; i < reduceCount; i++)
//        //    {
//        //        reduceList.Add(order[i]);
//        //    }
//        //    if (reduceList.Count == 0)
//        //    {
//        //        SectionReduceNonTrivial(oracle, x, order, sectionLength);
//        //    }
//        //    else
//        //    {
//        //        oracle.Reduce(reduceList);
//        //    }
//        //}

//        //private void SectionDelete(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
//        //{
//        //    for (int i = 0; i < cnt.Length; i++)
//        //    {
//        //        cnt[i] = 0;
//        //    }
//        //    int deleteCount = 0;
//        //    for (int i = x.Length - 1; i >= 0; i--)
//        //    {
//        //        if (x[i] <= 0)
//        //        {
//        //            break;
//        //        }
//        //        for (int j = 0; j < bigComponents.Count; j++)
//        //        {
//        //            cnt[data[j][i]]++;
//        //        }//for j
//        //        var flg = true;
//        //        for (int j = order.Length - deleteCount - 1; j >= i && flg; j--)
//        //        {
//        //            flg &= (cnt[order[j]] == bigComponents.Count);
//        //        }
//        //        if (flg)
//        //        {
//        //            deleteCount = x.Length - i;
//        //        }
//        //    }
//        //    deleteList.Clear();
//        //    for (int i = 0; i < deleteCount; i++)
//        //    {
//        //        deleteList.Add(order[order.Length - 1 - i]);
//        //    }
//        //    if (deleteList.Count == 0)
//        //    {
//        //        SectionDeleteNonTrivial(oracle, x, order, sectionLength);
//        //    }
//        //    else
//        //    {
//        //        oracle.Delete(deleteList);
//        //    }
//        //}

//        //private void SectionDeleteNonTrivial(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
//        //{
//        //    deleteList.Clear();
//        //    for (int i = x.Length - sectionLength; i >= 0 && x[i] > 0; i -= sectionLength)
//        //    {
//        //        SetIndex(oracle, order, i, sectionLength);
//        //        CalcMinimumNormPoint(oracle, smallComponents, smallExtremeBase, smallX, Tail, true);
//        //        oracle.remainder = memoRemainder;
//        //        if (smallComponents.X.Min() <= 0)
//        //        {
//        //            break;
//        //        }
//        //        list.Clear();
//        //        for (int j = 0; j < sectionLength; j++)
//        //        {
//        //            list.Add(order[i + j]);
//        //            deleteList.Add(order[i + j]);
//        //        }
//        //        oracle.Delete(list);
//        //    }
//        //}

//        //private void SectionReduceNonTrivial(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
//        //{
//        //    reduceList.Clear();
//        //    for (int i = 0; i + sectionLength < x.Length && x[i + sectionLength - 1] <= 0; i += sectionLength)
//        //    {
//        //        SetIndex(oracle, order, i, sectionLength);
//        //        CalcMinimumNormPoint(oracle, smallComponents, smallExtremeBase, smallX, Head, true);
//        //        oracle.remainder = memoRemainder;
//        //        if (smallComponents.X.Max() > 0)
//        //        {
//        //            break;
//        //        }
//        //        list.Clear();
//        //        for (int j = 0; j < sectionLength; j++)
//        //        {
//        //            list.Add(order[i + j]);
//        //            reduceList.Add(order[i + j]);
//        //        }
//        //        oracle.Reduce(list);
//        //    }
//        //}

//        //private void SetIndex(SubmodularOracle oracle, int[] order, int startIndex, int sectionLength)
//        //{
//        //    for (int i = 0; i < sectionLength; i++)
//        //    {
//        //        smallRemainder[i] = oracle.remainder[order[startIndex + i]];
//        //    }
//        //    memoRemainder = oracle.remainder;
//        //    oracle.remainder = smallRemainder;
//        //}

//        private void Initialize(SubmodularOracle oracle)
//        {
//            components = new ConvexComponentsQR(oracle.N, null, AbsEps, RelativeEps);
//            //smallComponents = new ConvexComponentsQR(baseSectionLength, null, AbsEps, RelativeEps);
//            extremeBase = new double[oracle.N];
//            x = new double[oracle.N];
//            reduceList = new List<int>();
//            deleteList = new List<int>();
//            list = new List<int>();
//            cnt = new int[oracle.N];
//            diffs = new int[oracle.N];
//            hash = new HashSet<int>();
//            tmpX = new double[oracle.N];
//            order = new int[oracle.N];
//            componentsReduceIndex = new List<int>();
//        }

//        private void CalcMinimumNormPoint(SubmodularOracle oracle)
//        {
//            if (oracle.Count == 0)
//            {
//                return;
//            }
//            components.Clear();
//            var order = Enumerable.Range(0, oracle.Count).ToArray();
//            oracle.CalcBase(order, extremeBase);
//            components.Add(extremeBase, order);
//            currentNorm = double.MaxValue;
//            long iteration = 0;
//            while (true)
//            {
//                iteration++;
//                components.SetX();
//                if (components.X.Max() <= 0 || components.X.Min() > 0)
//                {
//                    break;
//                }
//                if (oracle.Count > CutSize)
//                {
//                    ReduceAndDelete(oracle, components.X);
//                }
//                double nextNorm = components.CalcSquareKernel(components.X);
//                if (currentNorm <= nextNorm)
//                {
//                    break;
//                }//if
//                currentNorm = nextNorm;
//                var data = CalcLinearMinimizer(oracle, components.X, extremeBase);
//                if (IsMinimizer(currentNorm, extremeBase, components))
//                {
//                    break;
//                }//if
//                if (!components.Add(extremeBase, data))
//                {
//                    break;
//                }
//            }
//            components.SetXWithoutError();
//        }

//        private void ReduceAndDelete(SubmodularOracle oracle, double[] x)
//        {
//            data = components.GetData();
//            int n = x.Length;
//            if (!RemoveTrivial(oracle, x, n))
//            {
//                RemoveNonTrivial(oracle, x);
//            }
//        }

//        private void RemoveNonTrivial(SubmodularOracle oracle, double[] x)
//        {
//            componentsReduceIndex.Clear();
//            InitializeXAndOrder(x, oracle.Count);
//            HeuristicReduce(oracle, x);
//            HeuristicDelete(oracle, x);
//            componentsReduceIndex.Sort();
//            components.Delete(componentsReduceIndex);
//            ExecuteReduceAndDelete(oracle);
//        }

//        private bool HeuristicReduce(SubmodularOracle oracle, double[] x)
//        {
//            hash.Clear();
//            for (int i = 0; i < x.Length && tmpX[i] <= 0; i++)
//            {
//                hash.Add(order[i]);
//            }

//            int diff = InitializeDiffs(0);
//            for (int i = hash.Count - 1; i >= 0; i--)
//            {
//                if (CanSmallize(oracle, diff,(v=>v<=0),componentsReduceIndex))
//                {
//                    reduceList.Clear();
//                    for (int j = 0; j <= i; j++)
//                    {
//                        reduceList.Add(order[j]);
//                    }
//                    return true;
//                }
//                diff = UpdateDiffs(diff, i);
//            }
//            return false;
//        }

//        private bool HeuristicDelete(SubmodularOracle oracle, double[] x)
//        {
//            hash.Clear();
//            for (int i = x.Length - 1; i >= 0&&tmpX[i]>0; i--)
//            {
//                hash.Add(order[i]);
//            }

//            int diff = InitializeDiffs(x.Length - hash.Count);
//            for (int i = x.Length-1-hash.Count; i < x.Length; i++)
//            {
//                if (CanSmallize(oracle, diff, (v => v > 0),componentsReduceIndex))
//                {
//                    deleteList.Clear();
//                    for (int j = i; j < x.Length; j++)
//                    {
//                        deleteList.Add(order[j]);
//                    }
//                    return true;
//                }
//                diff = UpdateDiffs(diff, i);
//            }
//            return false;
//        }

//        private void InitializeXAndOrder(double[] x,int n)
//        {
//            for (int i = 0; i < x.Length; i++)
//            {
//                tmpX[i] = x[i];
//                order[i] = i;
//            }
//            Array.Sort(tmpX, order, 0, n);
//        }

//        private int UpdateDiffs(int diff, int index)
//        {
//            int cur = order[index];
//            hash.Remove(cur);
//            diff = 0;
//            for (int k = 0; k < data.Length; k++)
//            {
//                if (data[k][index] != cur)
//                {
//                    if (hash.Contains(data[k][index]))
//                    {
//                        diffs[k]++;
//                    }
//                    else
//                    {
//                        diffs[k]--;
//                    }
//                }
//                if (diffs[k] > 0)
//                {
//                    diff++;
//                }
//            }
//            return diff;
//        }

//        private int InitializeDiffs(int offset)
//        {
//            int diff = 0;
//            for (int i = 0; i < data.Length; i++)
//            {
//                diffs[i] = 0;
//                for (int j = 0; j < hash.Count; j++)
//                {
//                    int cur = data[i][j+offset];
//                    if (!hash.Contains(cur))
//                    {
//                        diffs[i]++;
//                    }
//                }
//                if (diffs[i] > 0)
//                {
//                    diff++;
//                }
//            }
//            return diff;
//        }

//        private bool CanSmallize(SubmodularOracle oracle, int diff,Func<double,bool> func,List<int> added)
//        {
//            if (diff>=CutRatio*data.Length)
//            {
//                return false;
//            }
//            list.Clear();
//            for (int i = 0; i < data.Length; i++)
//            {
//                if (diffs[i]>0)
//                {
//                    list.Add(i);
//                }
//            }
//            bool res =  components.CalcRemoveX(list, hash, func);
//            if (res)
//            {
//                foreach (var item in list)
//                {
//                    added.Add(item);
//                }
//            }
//            return res;
//        }

//        //private void SetHash(double[] x, Func<double, bool> func)
//        //{
//        //    hash.Clear();
//        //    for (int i = 0; i < x.Length; i++)
//        //    {
//        //        if (func.Invoke(x[i]))
//        //        {
//        //            hash.Add(i);
//        //        }
//        //    }
//        //}

//        //private bool HeuristicReduce(SubmodularOracle oracle, double[] x)
//        //{
//        //    SetHash(x, (v => v < 0));
//        //    list.Clear();
//        //    for (int i = 0; i < data.Length; i++)
//        //    {
//        //        for (int j = 0; j < hash.Count; j++)
//        //        {
//        //            if (!hash.Contains(data[i][j]))
//        //            {
//        //                list.Add(i);
//        //                break;
//        //            }
//        //        }
//        //    }
//        //    if (list.Count < CutRatio * data.Length)
//        //    {
//        //        Reduce(oracle,list);
//        //        return true;
//        //    }
//        //    return false;
//        //}

//        private void Reduce(SubmodularOracle oracle,List<int>list)
//        {
//            components.Delete(list);
//            deleteList.Clear();
//            reduceList.Clear();
//            foreach (var item in hash)
//            {
//                reduceList.Add(item);
//            }
//            oracle.Reduce(reduceList);
//            Reset(oracle);
//        }

//        //private bool HeuristicDelete(SubmodularOracle oracle, double[] x)
//        //{
//        //    SetHash(x, (v => v > 0));
//        //    list.Clear();
//        //    int n = x.Length;
//        //    for (int i = 0; i < data.Length; i++)
//        //    {
//        //        for (int j = 0; j < hash.Count; j++)
//        //        {
//        //            if (!hash.Contains(data[i][n - 1 - j]))
//        //            {
//        //                list.Add(i);
//        //                break;
//        //            }
//        //        }
//        //    }
//        //    if (list.Count < CutRatio * data.Length)
//        //    {
//        //        Delete(oracle,list);
//        //        return true;
//        //    }
//        //    return false;
//        //}

//        private void Delete(SubmodularOracle oracle,List<int>list)
//        {
//            components.Delete(list);
//            deleteList.Clear();
//            reduceList.Clear();
//            foreach (var item in hash)
//            {
//                deleteList.Add(item);
//            }
//            oracle.Delete(reduceList);
//            Reset(oracle);
//        }

//        private bool RemoveTrivial(SubmodularOracle oracle, double[] x, int n)
//        {
//            int lowerIndex = SetLowerIndex(x);
//            int upperIndex = SetUpperIndex(x);
//            SetHash(n, lowerIndex, upperIndex);
//            return ExecuteReduceAndDelete(oracle);
//        }

//        private bool ExecuteReduceAndDelete(SubmodularOracle oracle)
//        {
//            if (reduceList.Count > 0)
//            {
//                oracle.Reduce(reduceList);
//            }
//            if (deleteList.Count > 0)
//            {
//                oracle.Delete(deleteList);
//            }
//            if (reduceList.Count > 0 || deleteList.Count > 0)
//            {
//                Reset(oracle);
//                return true;
//            }
//            return false;
//        }

//        //private void AddModifyList(int[] reorder, List<int>addedList,List<int>modifiedList)
//        //{
//        //    for (int i = 0; i < reorder.Length; i++)
//        //    {
//        //        if (reorder[i]!=-1)
//        //        {
//        //            inverse[reorder[i]] = i;
//        //        }
//        //    }
//        //    foreach (var item in addedList)
//        //    {
//        //        modifiedList.Add(inverse[item]);
//        //    }
//        //}

//        private void Reset(SubmodularOracle oracle)
//        {
//            var reorder = new int[oracle.Count];
//            Resize(oracle, reorder);
//            components.Shuffle(reorder);
//            components.SetX();
//            currentNorm = double.MaxValue;
//        }

//        private void SetHash(int n, int lowerIndex, int upperIndex)
//        {
//            SetHashReduce(lowerIndex);
//            SetHashDelete(n, upperIndex);
//        }

//        private void SetHashDelete(int n, int upperIndex)
//        {
//            deleteList.Clear();
//            for (int i = n - 1; i >= upperIndex; i--)
//            {
//                deleteList.Add(data[0][i]);
//            }
//        }

//        private void SetHashReduce(int lowerIndex)
//        {
//            reduceList.Clear();
//            for (int i = 0; i <= lowerIndex; i++)
//            {
//                reduceList.Add(data[0][i]);
//            }
//        }

//        private int SetUpperIndex(double[] x)
//        {
//            int n = x.Length;
//            for (int i = 0; i < n; i++)
//            {
//                cnt[i] = 0;
//            }
//            int upperIndex = n;
//            for (int i = n - 1; i >= 0; i--)
//            {
//                if (x[data[0][i]] <= 0)
//                {
//                    break;
//                }
//                IsIdeal(ref upperIndex, i);
//            }
//            return upperIndex;
//        }

//        private int SetLowerIndex(double[] x)
//        {
//            int n = x.Length;
//            for (int i = 0; i < n; i++)
//            {
//                cnt[i] = 0;
//            }
//            int lowerIndex = -1;
//            for (int i = 0; i < n; i++)
//            {
//                if (x[data[0][i]] > 0)
//                {
//                    break;
//                }
//                IsIdeal(ref lowerIndex, i);
//            }
//            return lowerIndex;
//        }

//        private void IsIdeal(ref int upperIndex, int orderIndex)
//        {
//            for (int i = 0; i < data.Length; i++)
//            {
//                var cur = data[i][orderIndex];
//                cnt[cur]++;
//            }
//            for (int i = 0; i <= orderIndex; i++)
//            {
//                if (cnt[data[0][i]] != data.Length)
//                {
//                    return;
//                }
//            }
//            upperIndex = orderIndex;
//        }


//        //private void Check(SubmodularOracle oracle, ConvexComponents components, List<int> f)
//        //{
//        //    var r = new List<int>();
//        //    for (int i = 0; i < components.X.Length; i++)
//        //    {
//        //        if (components.X[i] <= 0)
//        //        {
//        //            r.Add(oracle.remainder[i]);
//        //        }
//        //    }
//        //    Console.WriteLine(r);
//        //    int cc = 0;
//        //    for (int i = 0; i < components.X.Length; i++)
//        //    {
//        //        if (components.X[i] <= 0 ^ aa[i] == '1')
//        //        {
//        //            cc++;
//        //        }
//        //    }
//        //    f.Add(cc);
//        //}

//        //private static void GetBase(SubmodularOracle oracle, double[] extremeBase, int state, int[] order)
//        //{
//        //    if (state == All)
//        //    {
//        //        oracle.CalcBase(order, extremeBase);
//        //    }
//        //    else if (state == Head)
//        //    {
//        //        oracle.CalcBaseHead(order, extremeBase);
//        //    }
//        //    else if (state == Tail)
//        //    {
//        //        oracle.CalcBaseTail(order, extremeBase);
//        //    }
//        //}

//        private int[] CalcLinearMinimizer(SubmodularOracle oracle, double[] x, double[] extremeBase)
//        {
//            int n = x.Length;
//            var order = new int[n];
//            for (int i = 0; i < n; i++)
//            {
//                order[i] = i;
//                tmpX[i] = x[i];
//            }
//            Array.Sort(tmpX, order, 0, n);
//            oracle.CalcBase(order, extremeBase);
//            return order;
//        }

//        private bool IsMinimizer(double norm, double[] extremeBase, ConvexComponents components)
//        {
//            double innerProduct = components.CalcInnerProductKernel(components.X, extremeBase);
//            if (Math.Abs(norm - innerProduct) <= Math.Max(norm, Math.Abs(innerProduct)) * RelativeEps)
//            {
//                return true;
//            }//if
//            return norm <= innerProduct;
//        }

//    }
//}
