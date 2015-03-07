using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.FW
{
    public class FWOriginal : FW
    {
        double AbsEps = 1e-10;
        double RelativeEps = 1e-10;
        const int All = 0;

        double currentNorm;
        ConvexComponentsQR bigComponents;
        double[] bigExtremeBase;
        double[] bigX;
        int[] inverse;
        int[] memoRemainder;
        int[] smallRemainder;
        int[] cnt;

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
            while (true)
            {
                CalcMinimumNormPoint(oracle, bigComponents, bigExtremeBase, bigX, All, true);
                break;
            }
            SetResult(oracle);
        }

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
            bigExtremeBase = new double[oracle.N];
            bigX = new double[oracle.N];
            smallRemainder = new int[oracle.N];
            inverse = new int[oracle.N];
            cnt = new int[oracle.N];
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
