using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.FW
{
    public class FWBinary : FW
    {
         double AbsEps = 1e-10;
         double RelativeEps = 1e-10;
        const int All = 0;
        const int Head = 1;
        const int Tail = 2;
        const double RepRatio = 0.02;
        const double RepRatioSmall = 0.01;
        const double CutRatio = 0.35;
        const double CutRatioSmall = 0.20;
        const int naiveSize = 100;

        double currentNorm;
        ConvexComponentsQR bigComponents;
        ConvexComponentsQR smallComponents;
        double[] bigExtremeBase;
        double[] smallExtremeBase;
        double[] bigX;
        double[] smallX;
        List<int> contractList;
        List<int> deleteList;
        List<int> list;
        List<int> modifiedDeleteList;
        List<int> modifiedContractList;
        HashSet<int> hash;
        int[] inverse;
        int[] memoRemainder;
        int[] smallRemainder;
        int[] cnt;
        int[][] data;
        int cutIteration;


        public long Iteration
        {
            get;
            private set;
        }


        public override void Minimization(SubmodularOracle oracle,double absEps = 1e-10,double relativeEps = 1e-10)
        {
            this.AbsEps = absEps;
            this.RelativeEps = relativeEps;
            Initialize(oracle);
            int smallReptation = Math.Max(10, (int)(oracle.N * RepRatio));
            bool fromScrach = true;
            while (true)
            {
                if (oracle.Count < naiveSize)
                //if(true)
                {
                    CalcMinimumNormPoint(oracle, bigComponents, bigExtremeBase, bigX, All, true);
                    break;
                }
                else
                {
                    CalcMinimumNormPoint(oracle, bigComponents, bigExtremeBase, bigX, All, fromScrach, maxIteration: smallReptation);
                    //Check(oracle);
                    if (oracle.Count==0||bigComponents.X.Min() > 0 || bigComponents.X.Max() <= 0)
                    {
                        break;
                    }
                    contractList.Clear();
                    deleteList.Clear();
                    var x = bigComponents.X;
                    for (int i = 0; i < bigComponents.X.Length; i++)
                    {
                        if (x[i] <= 0)
                        {
                            contractList.Add(i);
                        }
                        else
                        {
                            deleteList.Add(i);
                        }
                    }

                    var reorderPositive = new int[oracle.Count];
                    var reorderNegative = new int[oracle.Count];
                    var positiveOracle = oracle.Copy();
                    var negativeOracle = oracle.Copy();
                    list.Clear();
                    positiveOracle.Contract(contractList);
                    negativeOracle.Delete(deleteList);
                    positiveOracle.SetRemainder(contractList, list, reorderPositive);
                    negativeOracle.SetRemainder(list, deleteList, reorderNegative);
                    modifiedDeleteList.Clear();
                    modifiedContractList.Clear();

                    if (!fromScrach || positiveOracle.Count <= negativeOracle.Count)
                    {
                        ResizeSmall(positiveOracle.Count);
                        CalcMinimumNormPoint(positiveOracle, smallComponents, smallExtremeBase, smallX, Tail, true, reorderPositive, modifiedDeleteList);
                        oracle.AddBaseCall(positiveOracle.BaseCall);
                        Add(oracle, positiveOracle, modifiedDeleteList, smallComponents.X, reorderPositive, (s) => s > 0);
                        oracle.Delete(modifiedDeleteList);
                    }
                    if (!fromScrach || negativeOracle.Count <= positiveOracle.Count)
                    {
                        ResizeSmall(negativeOracle.Count);
                        CalcMinimumNormPoint(negativeOracle, smallComponents, smallExtremeBase, smallX, Head, true, reorderNegative, modifiedContractList);
                        oracle.AddBaseCall(negativeOracle.BaseCall);
                        Add(oracle, negativeOracle, modifiedContractList, smallComponents.X, reorderNegative, (s) => s <= 0);
                        oracle.Contract(modifiedContractList);
                    }


                    if (modifiedContractList.Count==0&&modifiedDeleteList.Count==0)
                    {
                        fromScrach = false;
                    }
                    else
                    {
                        fromScrach = true;
                        var tmpVariable = modifiedDeleteList;
                        modifiedDeleteList = deleteList;
                        deleteList = tmpVariable;
                        tmpVariable = modifiedContractList;
                        modifiedContractList = contractList;
                        contractList = tmpVariable;
                        ResizeBig(All, oracle);
                       //Check(oracle);
                    }
                }
            }
            SetResult(oracle);
        }

        //private static void Check(SubmodularOracle oracle)
        //{
        //    var need = new int[] { 67, 276, 287, 310, 373, 513, 629, 706, 752, 790, 809, 897, 968, 989 };
        //    foreach (var item in need)
        //    {
        //        if (!oracle.remainder.Contains(item))
        //        {
        //            Console.WriteLine();
        //            //System.Diagnostics.Debugger.Break();
        //        }
        //    }
        //}

        private void SetResult(SubmodularOracle oracle)
        {
            sw.Stop();
            var minimizer = oracle.Minimizer;

            MinimumValue = oracle.EmptyValue;
            for (int i = 0; i < oracle.Count; i++)
            {
                if (bigComponents.X[i] <= 0)
                {
                    minimizer[oracle.remainder[i]] = true;
                    MinimumValue += bigComponents.X[i];
                }
            }
            Minimizer = new string(minimizer.Select(x => x ? '1' : '0').ToArray());
        }

        private void Add(SubmodularOracle b, SubmodularOracle oracle, List<int> modified, double[] x, int[] reorder, Func<double, bool> func)
        {
            for (int i = 0; i < reorder.Length; i++)
            {
                if (reorder[i] != -1)
                {
                    inverse[reorder[i]] = i;
                }
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (func.Invoke(x[i]))
                {
                    modified.Add(inverse[i]);
                }
            }

            //var hash = new HashSet<int>();
            //for (int i = 0; i < x.Length; i++)
            //{
            //    if (func.Invoke(x[i]))
            //    {
            //        hash.Add(oracle.remainder[i]);
            //    }
            //}
            //for (int i = 0; i < b.remainder.Length; i++)
            //{
            //    if (hash.Contains(b.remainder[i]))
            //    {
            //        modified.Add(i);
            //    }
            //}



        }

        private void ResizeSmall(int n)
        {
            smallComponents.Resize(n);
            Array.Resize(ref smallExtremeBase, n);
            Array.Resize(ref smallX, n);
            Array.Resize(ref smallRemainder, n);
        }

        private void ResizeBig(int state, SubmodularOracle oracle, int[] reorder = null)
        {
            if (state == All)
            {
                oracle.SetRemainder(contractList, deleteList, reorder);
                bigComponents.Resize(oracle.Count);
                Array.Resize(ref bigExtremeBase, oracle.Count);
                Array.Resize(ref bigX, oracle.Count);
            }
            else
            {
                oracle.SetRemainder(contractList, deleteList, reorder);
                smallComponents.Resize(oracle.Count);
                Array.Resize(ref smallExtremeBase, oracle.Count);
                Array.Resize(ref smallX, oracle.Count);
            }
        }

        //private void SectionReduce(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
        //{
        //    for (int i = 0; i < cnt.Length; i++)
        //    {
        //        cnt[i] = 0;
        //    }
        //    int reduceCount = 0;
        //    for (int i = 0; i < x.Length; i++)
        //    {
        //        if (x[i] > 0)
        //        {
        //            break;
        //        }
        //        for (int j = 0; j < bigComponents.Count; j++)
        //        {
        //            cnt[data[j][i]]++;
        //        }
        //        var flg = true;
        //        for (int j = reduceCount; j <= i && flg; j++)
        //        {
        //            flg &= (cnt[order[j]] == bigComponents.Count);
        //        }
        //        if (flg)
        //        {
        //            reduceCount = i + 1;
        //        }
        //    }
        //    reduceList.Clear();
        //    for (int i = 0; i < reduceCount; i++)
        //    {
        //        reduceList.Add(order[i]);
        //    }
        //    if (reduceList.Count == 0)
        //    {
        //        SectionReduceNonTrivial(oracle, x, order, sectionLength);
        //    }
        //    else
        //    {
        //        oracle.Reduce(reduceList);
        //    }
        //}

        //private void SectionDelete(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
        //{
        //    for (int i = 0; i < cnt.Length; i++)
        //    {
        //        cnt[i] = 0;
        //    }
        //    int deleteCount = 0;
        //    for (int i = x.Length - 1; i >= 0; i--)
        //    {
        //        if (x[i] <= 0)
        //        {
        //            break;
        //        }
        //        for (int j = 0; j < bigComponents.Count; j++)
        //        {
        //            cnt[data[j][i]]++;
        //        }//for j
        //        var flg = true;
        //        for (int j = order.Length - deleteCount - 1; j >= i && flg; j--)
        //        {
        //            flg &= (cnt[order[j]] == bigComponents.Count);
        //        }
        //        if (flg)
        //        {
        //            deleteCount = x.Length - i;
        //        }
        //    }
        //    deleteList.Clear();
        //    for (int i = 0; i < deleteCount; i++)
        //    {
        //        deleteList.Add(order[order.Length - 1 - i]);
        //    }
        //    if (deleteList.Count == 0)
        //    {
        //        SectionDeleteNonTrivial(oracle, x, order, sectionLength);
        //    }
        //    else
        //    {
        //        oracle.Delete(deleteList);
        //    }
        //}

        //private void SectionDeleteNonTrivial(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
        //{
        //    deleteList.Clear();
        //    for (int i = x.Length - sectionLength; i >= 0 && x[i] > 0; i -= sectionLength)
        //    {
        //        SetIndex(oracle, order, i, sectionLength);
        //        CalcMinimumNormPoint(oracle, smallComponents, smallExtremeBase, smallX, Tail, true);
        //        oracle.remainder = memoRemainder;
        //        if (smallComponents.X.Min() <= 0)
        //        {
        //            break;
        //        }
        //        list.Clear();
        //        for (int j = 0; j < sectionLength; j++)
        //        {
        //            list.Add(order[i + j]);
        //            deleteList.Add(order[i + j]);
        //        }
        //        oracle.Delete(list);
        //    }
        //}

        //private void SectionReduceNonTrivial(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
        //{
        //    reduceList.Clear();
        //    for (int i = 0; i + sectionLength < x.Length && x[i + sectionLength - 1] <= 0; i += sectionLength)
        //    {
        //        SetIndex(oracle, order, i, sectionLength);
        //        CalcMinimumNormPoint(oracle, smallComponents, smallExtremeBase, smallX, Head, true);
        //        oracle.remainder = memoRemainder;
        //        if (smallComponents.X.Max() > 0)
        //        {
        //            break;
        //        }
        //        list.Clear();
        //        for (int j = 0; j < sectionLength; j++)
        //        {
        //            list.Add(order[i + j]);
        //            reduceList.Add(order[i + j]);
        //        }
        //        oracle.Reduce(list);
        //    }
        //}

        private void SetIndex(SubmodularOracle oracle, int[] order, int startIndex, int sectionLength)
        {
            for (int i = 0; i < sectionLength; i++)
            {
                smallRemainder[i] = oracle.remainder[order[startIndex + i]];
            }
            memoRemainder = oracle.remainder;
            oracle.remainder = smallRemainder;
        }

        private void Initialize(SubmodularOracle oracle)
        {
            sw = new Stopwatch();
            sw.Start();
            Iteration = 0;
            bigComponents = new ConvexComponentsQR(oracle.N, null, AbsEps, RelativeEps);
            //smallComponents = new ConvexComponentsQR(baseSectionLength, null, AbsEps, RelativeEps);
            smallComponents = new ConvexComponentsQR(oracle.N, null, AbsEps, RelativeEps);
            bigExtremeBase = new double[oracle.N];
            smallExtremeBase = new double[oracle.N];
            bigX = new double[oracle.N];
            smallX = new double[oracle.N];
            contractList = new List<int>();
            deleteList = new List<int>();
            list = new List<int>();
            smallRemainder = new int[oracle.N];
            inverse = new int[oracle.N];
            cnt = new int[oracle.N];
            modifiedDeleteList = new List<int>();
            modifiedContractList = new List<int>();
            hash = new HashSet<int>();
            cutIteration = Math.Max(10, (int)(oracle.N * RepRatioSmall));
        }

        private void CalcMinimumNormPoint(SubmodularOracle oracle, ConvexComponents components, double[] extremeBase, double[] tmpX, int state, bool needInitialize, int[] reorder = null, List<int> modifiedList = null, long maxIteration = long.MaxValue)
        {
            if (oracle.Count == 0)
            {
                return;
            }
            if (needInitialize)
            {
                components.Clear();
                var order = Enumerable.Range(0, tmpX.Length).ToArray();
                oracle.CalcBase(order, extremeBase);
                components.Add(extremeBase, order);
            }
            currentNorm = double.MaxValue;
            long iteration = 0;
            while (iteration < maxIteration)
            {
                iteration++;
                components.SetX();
                //Check(oracle, components, f);
                if (components.X.Max() <= 0 || components.X.Min() > 0)
                {
                    break;
                }
                Reduce(iteration,oracle, components, state, components.X, reorder, modifiedList);
                double nextNorm = components.CalcSquareKernel(components.X);
                if (currentNorm <= nextNorm)
                {
                    break;
                }//if
                currentNorm = nextNorm;
                var data = CalcLinearMinimizer(oracle, tmpX, state, components.X, extremeBase);
                if (IsMinimizer(currentNorm, extremeBase, components))
                {
                    break;
                }//if
                if (!components.Add(extremeBase, data))
                {
                    break;
                }
            }
            components.SetXWithoutError();
            Iteration += iteration;
        }

        private void Reduce(long iteration,SubmodularOracle oracle, ConvexComponents components, int state, double[] x,int[] reorder = null, List<int> modifiedList = null)
        {
            data = components.GetData();
            int n = x.Length;
            var flg = false;
            if (state == All)
            {
                flg = RemoveTrivial(oracle, components, x, n, state);
            }
            else if (state == Head)
            {
                flg = RemoveTrivial(oracle, components, x, n, state, reorder, modifiedList);
            }
            else if (state == Tail)
            {
                flg = RemoveTrivial(oracle, components, x, n, state, reorder, modifiedList);
            }
            if (!flg)
            {
                if (state == Head)
                {
                    HeuristicDelete(oracle, components, state, x, reorder,iteration);
                }
                else if (state == Tail)
                {
                    HeuristicContract(oracle, components, state, x, reorder,iteration);
                }
            }
        }

        //private void HeuristicDelete(SubmodularOracle oracle, ConvexComponents components, int state, double[] x, int[] reorder)
        //{
        //    int n = x.Length;
        //    var order = new int[n];
        //    var tmpX = new double[n];
        //    for (int i = 0; i < n; i++)
        //    {
        //        order[i] = i;
        //        tmpX[i] = x[i];
        //    }
        //    Array.Sort(tmpX, order);

        //    hash.Clear();
        //    var hashes = new HashSet<int>[data.Length];
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        hashes[i] = new HashSet<int>();
        //    }

        //    for (int i = 0; i < n * 0.9; i++)
        //    {
        //        int cnt = 0;
        //        int cur = order[i];
        //        for (int j = 0; j < data.Length; j++)
        //        {
        //            int need = data[j][i];
        //            if (need != cur)
        //            {
        //                if (hashes[j].Contains(cur))
        //                {
        //                    hashes[j].Remove(cur);
        //                }
        //                else if (!hash.Contains(need))
        //                {
        //                    hashes[j].Add(need);
        //                }
        //            }
        //            if (hashes[j].Count != 0)
        //            {
        //                cnt++;
        //            }
        //        }
        //        hash.Add(cur);
        //        if (0.15*n<i&&cnt < CutRatio * data.Length)
        //        {
        //            list.Clear();
        //            for (int j = 0; j < data.Length; j++)
        //            {
        //                if (hashes[j].Count != 0)
        //                {
        //                    list.Add(j);
        //                }
        //            }
        //            components.Delete(list);
        //            deleteList.Clear();
        //            contractList.Clear();
        //            foreach (var item in hash)
        //            {
        //                deleteList.Add(item);
        //            }
        //            oracle.Delete(contractList);
        //            ResetBig(state, components, oracle, reorder);
        //            break;
        //        }
        //    }

        //}

        //private void HeuristicContract(SubmodularOracle oracle, ConvexComponents components, int state, double[] x, int[] reorder)
        //{
        //    int n = x.Length;
        //    var order = new int[n];
        //    var tmpX = new double[n];
        //    for (int i = 0; i < n; i++)
        //    {
        //        order[i] = i;
        //        tmpX[i] = x[i];
        //    }
        //    Array.Sort(tmpX, order);

        //    hash.Clear();
        //    var hashes = new HashSet<int>[data.Length];
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        hashes[i] = new HashSet<int>();
        //    }

            
        //    for (int i = n - 1; i >= n * 0.1; i--)
        //    {
        //        int cnt = 0;
        //        int cur = order[i];
        //        for (int j = 0; j < data.Length; j++)
        //        {
        //            int need = data[j][i];
        //            if (need != cur)
        //            {
        //                if (hashes[j].Contains(cur))
        //                {
        //                    hashes[j].Remove(cur);
        //                }
        //                else if (!hash.Contains(need))
        //                {
        //                    hashes[j].Add(need);
        //                }
        //            }
        //            if (hashes[j].Count != 0)
        //            {
        //                cnt++;
        //            }
        //        }
        //        hash.Add(cur);
        //        if (i<0.85*n&&cnt < CutRatio * data.Length)
        //        {
        //            list.Clear();
        //            for (int j = 0; j < data.Length; j++)
        //            {
        //                if (hashes[j].Count != 0)
        //                {
        //                    list.Add(j);
        //                }
        //            }
        //            components.Delete(list);
        //            deleteList.Clear();
        //            contractList.Clear();
        //            foreach (var item in hash)
        //            {
        //                deleteList.Add(item);
        //            }
        //            oracle.Delete(contractList);
        //            ResetBig(state, components, oracle, reorder);
        //            break;
        //        }
        //    }

        //}

        private void HeuristicDelete(SubmodularOracle oracle, ConvexComponents components, int state, double[] x, int[] reorder,long iteration)
        {
            if (iteration % cutIteration == cutIteration - 1)
            {
                int index = 0;
                for (int i = 0; i < components.Count; i++)
                {
                    if (components.Lambdas[i] > components.Lambdas[index])
                    {
                        index = i;
                    }
                }
                list.Clear();
                for (int i = 0; i < components.Count; i++)
                {
                    if (i != index)
                    {
                        list.Add(i);
                    }
                }
                hash.Clear();
                int start = (int)(Math.Ceiling((1 - CutRatioSmall) * x.Length));
                for (int i = start; i < x.Length; i++)
                {
                    hash.Add(data[index][i]);
                }
                ExecuteDelete(oracle, components, state, reorder);
                return;
            }
            else
            {
                hash.Clear();
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] > 0)
                    {
                        hash.Add(i);
                    }
                }
                double sum = 0;
                list.Clear();
                int n = x.Length;
                for (int i = 0; i < data.Length; i++)
                {
                    for (int j = 0; j < hash.Count; j++)
                    {
                        if (!hash.Contains(data[i][n - 1 - j]))
                        {
                            sum += components.Lambdas[i];
                            list.Add(i);
                            break;
                        }
                    }
                }
                //if(sum<CutRatio)
                if (list.Count < CutRatio * data.Length)
                {
                    ExecuteDelete(oracle, components, state, reorder);
                }
            }
        }

        private void ExecuteDelete(SubmodularOracle oracle, ConvexComponents components, int state, int[] reorder)
        {
            components.Delete(list);
            deleteList.Clear();
            contractList.Clear();
            foreach (var item in hash)
            {
                deleteList.Add(item);
            }
            oracle.Delete(contractList);
            ResetBig(state, components, oracle, reorder);
        }

        private void HeuristicContract(SubmodularOracle oracle, ConvexComponents components, int state, double[] x, int[] reorder, long iteration)
        {
            if (iteration % cutIteration == cutIteration - 1)
            {
                int index = 0;
                for (int i = 0; i < components.Count; i++)
                {
                    if (components.Lambdas[i] > components.Lambdas[index])
                    {
                        index = i;
                    }
                }
                list.Clear();
                for (int i = 0; i < components.Count; i++)
                {
                    if (i != index)
                    {
                        list.Add(i);
                    }
                }
                hash.Clear();
                int end = (int)(Math.Floor(CutRatioSmall * x.Length));
                for (int i = 0; i <end; i++)
                {
                    hash.Add(data[index][i]);
                }
                ExecuteContract(oracle, components, state, reorder);
                return;
            }

            hash.Clear();
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] <= 0)
                {
                    hash.Add(i);
                }
            }
            double sum = 0;
            list.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < hash.Count; j++)
                {
                    if (!hash.Contains(data[i][j]))
                    {
                        sum += components.Lambdas[i];
                        list.Add(i);
                        break;
                    }
                }
            }
            //if(sum<CutRatio)
            if (list.Count < CutRatio * data.Length)
            {
                ExecuteContract(oracle, components, state, reorder);
            }
        }

        private void ExecuteContract(SubmodularOracle oracle, ConvexComponents components, int state, int[] reorder)
        {
            components.Delete(list);
            deleteList.Clear();
            contractList.Clear();
            foreach (var item in hash)
            {
                contractList.Add(item);
            }
            oracle.Contract(contractList);
            ResetBig(state, components, oracle, reorder);
        }

        private bool RemoveTrivial(SubmodularOracle oracle, ConvexComponents components, double[] x, int n, int state, int[] reorder = null, List<int> modifiedList = null)
        {
            int lowerIndex = SetLowerIndex(x);
            int upperIndex = SetUpperIndex(x);
            SetHash(n, lowerIndex, upperIndex);
            if (lowerIndex != -1)
            {
                if (state == Head)
                {
                    AddModifyList(reorder, contractList, modifiedList);
                }
                oracle.Contract(contractList);
            }
            if (upperIndex != x.Length)
            {
                if (state == Tail)
                {
                    AddModifyList(reorder, deleteList, modifiedDeleteList);
                }
                oracle.Delete(deleteList);
            }
            if (contractList.Count > 0 || deleteList.Count > 0)
            {

                ResetBig(state, components, oracle, reorder);
                return true;
            }
            return false;
        }

        private void AddModifyList(int[] reorder, List<int> addedList, List<int> modifiedList)
        {
            for (int i = 0; i < reorder.Length; i++)
            {
                if (reorder[i] != -1)
                {
                    inverse[reorder[i]] = i;
                }
            }
            foreach (var item in addedList)
            {
                modifiedList.Add(inverse[item]);
            }
        }

        private void ResetBig(int state, ConvexComponents components, SubmodularOracle oracle, int[] prevReorder = null)
        {
            var reorder = new int[oracle.Count];
            ResizeBig(state, oracle, reorder);
            components.Shuffle(reorder);
            components.SetX();
            currentNorm = double.MaxValue;
            if (prevReorder != null)
            {
                for (int i = 0; i < prevReorder.Length; i++)
                {
                    if (prevReorder[i] != -1)
                    {
                        prevReorder[i] = reorder[prevReorder[i]];
                    }
                }
            }
        }

        private void SetHash(int n, int lowerIndex, int upperIndex)
        {
            SetHashContract(lowerIndex);
            SetHashDelete(n, upperIndex);
        }

        private void SetHashDelete(int n, int upperIndex)
        {
            deleteList.Clear();
            for (int i = n - 1; i >= upperIndex; i--)
            {
                deleteList.Add(data[0][i]);
            }
        }

        private void SetHashContract(int lowerIndex)
        {
            contractList.Clear();
            for (int i = 0; i <= lowerIndex; i++)
            {
                contractList.Add(data[0][i]);
            }
        }

        private int SetUpperIndex(double[] x)
        {
            int n = x.Length;
            for (int i = 0; i < n; i++)
            {
                cnt[i] = 0;
            }
            int remainder = 0;
            int upperIndex = n;
            for (int i = n - 1; i >= 0; i--)
            {
                if (x[data[0][i]] <= 0)
                {
                    break;
                }
                IsIdeal(ref remainder, ref upperIndex, i);                
            }
            return upperIndex;
        }

        private int SetLowerIndex(double[] x)
        {
            int n = x.Length;
            for (int i = 0; i < n; i++)
            {
                cnt[i] = 0;
            }
            int remainder = 0;
            int lowerIndex = -1;
            for (int i = 0; i < n; i++)
            {
                if (x[data[0][i]] > 0)
                {
                    break;
                }
                IsIdeal(ref remainder, ref lowerIndex, i);
            }
            return lowerIndex;
        }

        private void IsIdeal(ref int remainder, ref int filledIndex, int index)
        {
            for (int k = 0; k < data.Length; k++)
            {
                int cur = data[k][index];
                if (cnt[cur] == 0)
                {
                    remainder++;
                }
                cnt[cur]++;
                if (cnt[cur] == data.Length)
                {
                    remainder--;
                }
            }
            if (remainder == 0)
            {
                filledIndex = index;
            }
        }


        //private void Check(SubmodularOracle oracle, ConvexComponents components, List<int> f)
        //{
        //    var r = new List<int>();
        //    for (int i = 0; i < components.X.Length; i++)
        //    {
        //        if (components.X[i] <= 0)
        //        {
        //            r.Add(oracle.remainder[i]);
        //        }
        //    }
        //    Console.WriteLine(r);
        //    int cc = 0;
        //    for (int i = 0; i < components.X.Length; i++)
        //    {
        //        if (components.X[i] <= 0 ^ aa[i] == '1')
        //        {
        //            cc++;
        //        }
        //    }
        //    f.Add(cc);
        //}

        //private static void GetBase(SubmodularOracle oracle, double[] extremeBase, int state, int[] order)
        //{
        //    if (state == All)
        //    {
        //        oracle.CalcBase(order, extremeBase);
        //    }
        //    else if (state == Head)
        //    {
        //        oracle.CalcBaseHead(order, extremeBase);
        //    }
        //    else if (state == Tail)
        //    {
        //        oracle.CalcBaseTail(order, extremeBase);
        //    }
        //}

        private int[] CalcLinearMinimizer(SubmodularOracle oracle, double[] tmpX, int state, double[] x, double[] extremeBase)
        {
            int n = x.Length;
            var order = new int[n];
            for (int i = 0; i < n; i++)
            {
                order[i] = i;
                tmpX[i] = x[i];
            }
            Array.Sort(tmpX, order, 0, n);
            oracle.CalcBase(order, extremeBase);
            return order;
        }

        private bool IsMinimizer(double norm, double[] extremeBase, ConvexComponents components)
        {
            double innerProduct = components.CalcInnerProductKernel(components.X, extremeBase);
            if (Math.Abs(norm - innerProduct) <= Math.Max(norm, Math.Abs(innerProduct)) * RelativeEps)
            {
                return true;
            }//if
            return norm <= innerProduct;
        }

    }
}
