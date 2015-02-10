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
//        const int baseSectionLength = 5;
//        const int baseNumIteration = 350;
//        const double AbsEps = 1e-12;
//        const double RelativeEps = 1e-10;
//        const int All = 0;
//        const int Head = 1;
//        const int Tail = 2;

//        ConvexComponentsQR bigComponents;
//        ConvexComponentsQR smallComponents;
//        double[] bigExtremeBase;
//        double[] smallExtremeBase;
//        double[] bigX;
//        double[] smallX;
//        int[] bigOrder;
//        List<int> reduceList;
//        List<int> deleteList;
//        List<int> list;
//        int[] memoRemainder;
//        int[] smallRemainder;
//        int[] cnt;
//        int[][] data;

//        public double Minimization(SubmodularOracle oracle)
//        {
//            double e = oracle.EmptyValue;
//            double m = oracle.FullValue;
//            int numIteration = baseNumIteration;
//            int sectionLength = baseSectionLength;
//            //long iteration =0;
//            Initialize(oracle);
//            var timer = new Stopwatch();
//            //ModifyOracle(oracle);
//            timer.Start();
//            bool first = true;
//            while (true)
//            {
//                //Console.WriteLine(oracle.Count+" "+oracle.CountReduce+" "+oracle.CountDelete);
//                if (oracle.Count < 2 * sectionLength)
//                {
//                    CalcMinimumNormPoint(oracle, bigComponents, bigExtremeBase, bigX, All, true);
//                    break;
//                }
//                else
//                {
//                    //iteration++;

//                    CalcMinimumNormPoint(oracle, bigComponents, bigExtremeBase, bigX, All, first, numIteration);
//                    for (int i = 0; i < bigOrder.Length; i++)
//                    {
//                        bigOrder[i] = i;
//                        bigX[i] = bigComponents.X[i];
//                    }
//                    Array.Sort(bigX, bigOrder);
//                    data = bigComponents.GetData();
//                    SectionReduce(oracle, bigX, bigOrder, sectionLength);
//                    SectionDelete(oracle, bigX, bigOrder, sectionLength);
//                    if (reduceList.Count > 0 || deleteList.Count > 0)
//                    {
//                        ResizeBig(oracle);
//                        first = true;
//                    }
//                    else
//                    {
//                        //numIteration += baseNumIteration;
//                        sectionLength += baseSectionLength;
//                        ResizeSmall(sectionLength);
//                        first = false;
//                    }
//                }
//            }
//            timer.Stop();
//            list.Clear();
//            for (int i = 0; i < bigComponents.X.Length; i++)
//            {
//                if (bigComponents.X[i] < 0)
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

//        List<int> f = new List<int>();
//        string aa = "0110000000000000100100000000000100001001000001000010000000000000100001010100000001000000100100110101000000000000100000000000000001010010000010000100001000000101000000000011010000000010000000000001000000000000000000000000000000010100100000000100110000100000100000100010010000001000000000000000100000000000100011010111000000100000100010100000000011000001010000100000110000000000000000000000000000000000000100000100000010100100010000100000000000000000000000000101000000000011000110010000000000000000010001000000000000000010100010100100100000010000000000000000000000010000000100000000000000000000001010000001000010000001000000010100000000000001000000100000000000010000000001000010110000000000000001000001000000000001100010000011000010010000010000000000100100000000010000000000100000000000000000000000001110010000010100000000100000000000000000000000010000000011110000000101001000101000000010100000001000000000000100000000000000000010000010000000000000000010000000100000010000000000100001000000000010000001000000010000000000000000";

//        private void ModifyOracle(SubmodularOracle oracle)
//        {
//            deleteList.Clear();
//            reduceList.Clear();
//            for (int i = 0; i < aa.Length; i++)
//            {
//                if (aa[i] == '0')
//                {
//                    //deleteList.Add(i);
//                }
//                if (aa[i] == '1')
//                {
//                    reduceList.Add(i);
//                }
//            }
//            int len = 10;
//            //reduceList.RemoveRange(reduceList.Count - len, len);
//            oracle.Reduce(reduceList);
//            //oracle.Delete(deleteList);
//            ResizeBig(oracle);
//        }

//        private void ResizeSmall(int sectionLength)
//        {
//            smallComponents.Resize(sectionLength);
//            Array.Resize(ref smallExtremeBase, sectionLength);
//            Array.Resize(ref smallX, sectionLength);
//            Array.Resize(ref smallRemainder, sectionLength);
//        }

//        private void ResizeBig(SubmodularOracle oracle)
//        {
//            oracle.SetRemainder(reduceList, deleteList);
//            bigComponents.Resize(oracle.Count);
//            Array.Resize(ref bigExtremeBase, oracle.Count);
//            Array.Resize(ref bigOrder, oracle.Count);
//            Array.Resize(ref bigX, oracle.Count);
//        }

//        private void SectionReduce(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
//        {
//            for (int i = 0; i < cnt.Length; i++)
//            {
//                cnt[i] = 0;
//            }
//            int reduceCount = 0;
//            for (int i = 0; i < x.Length; i++)
//            {
//                if (x[i] > 0)
//                {
//                    break;
//                }
//                for (int j = 0; j < bigComponents.Count; j++)
//                {
//                    cnt[data[j][i]]++;
//                }
//                var flg = true;
//                for (int j = reduceCount; j <= i && flg; j++)
//                {
//                    flg &= (cnt[order[j]] == bigComponents.Count);
//                }
//                if (flg)
//                {
//                    reduceCount = i + 1;
//                }
//            }
//            reduceList.Clear();
//            for (int i = 0; i < reduceCount; i++)
//            {
//                reduceList.Add(order[i]);
//            }
//            if (reduceList.Count == 0)
//            {
//                SectionReduceNonTrivial(oracle, x, order, sectionLength);
//            }
//            else
//            {
//                oracle.Reduce(reduceList);
//            }
//        }

//        private void SectionDelete(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
//        {
//            for (int i = 0; i < cnt.Length; i++)
//            {
//                cnt[i] = 0;
//            }
//            int deleteCount = 0;
//            for (int i = x.Length - 1; i >= 0; i--)
//            {
//                if (x[i] <= 0)
//                {
//                    break;
//                }
//                for (int j = 0; j < bigComponents.Count; j++)
//                {
//                    cnt[data[j][i]]++;
//                }//for j
//                var flg = true;
//                for (int j = order.Length - deleteCount - 1; j >= i && flg; j--)
//                {
//                    flg &= (cnt[order[j]] == bigComponents.Count);
//                }
//                if (flg)
//                {
//                    deleteCount = x.Length - i;
//                }
//            }
//            deleteList.Clear();
//            for (int i = 0; i < deleteCount; i++)
//            {
//                deleteList.Add(order[order.Length - 1 - i]);
//            }
//            if (deleteList.Count == 0)
//            {
//                SectionDeleteNonTrivial(oracle, x, order, sectionLength);
//            }
//            else
//            {
//                oracle.Delete(deleteList);
//            }
//        }

//        private void SectionDeleteNonTrivial(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
//        {
//            deleteList.Clear();
//            for (int i = x.Length - sectionLength; i >= 0 && x[i] > 0; i -= sectionLength)
//            {
//                SetIndex(oracle, order, i, sectionLength);
//                CalcMinimumNormPoint(oracle, smallComponents, smallExtremeBase, smallX, Tail, true);
//                oracle.remainder = memoRemainder;
//                if (smallComponents.X.Min() <= 0)
//                {
//                    break;
//                }
//                list.Clear();
//                for (int j = 0; j < sectionLength; j++)
//                {
//                    list.Add(order[i + j]);
//                    deleteList.Add(order[i + j]);
//                }
//                oracle.Delete(list);
//            }
//        }

//        private void SectionReduceNonTrivial(SubmodularOracle oracle, double[] x, int[] order, int sectionLength)
//        {
//            reduceList.Clear();
//            for (int i = 0; i + sectionLength < x.Length && x[i + sectionLength - 1] <= 0; i += sectionLength)
//            {
//                SetIndex(oracle, order, i, sectionLength);
//                CalcMinimumNormPoint(oracle, smallComponents, smallExtremeBase, smallX, Head, true);
//                oracle.remainder = memoRemainder;
//                if (smallComponents.X.Max() > 0)
//                {
//                    break;
//                }
//                list.Clear();
//                for (int j = 0; j < sectionLength; j++)
//                {
//                    list.Add(order[i + j]);
//                    reduceList.Add(order[i + j]);
//                }
//                oracle.Reduce(list);
//            }
//        }

//        private void SetIndex(SubmodularOracle oracle, int[] order, int startIndex, int sectionLength)
//        {
//            for (int i = 0; i < sectionLength; i++)
//            {
//                smallRemainder[i] = oracle.remainder[order[startIndex + i]];
//            }
//            memoRemainder = oracle.remainder;
//            oracle.remainder = smallRemainder;
//        }

//        private void Initialize(SubmodularOracle oracle)
//        {
//            bigComponents = new ConvexComponentsQR(oracle.N, null, AbsEps, RelativeEps);
//            //smallComponents = new ConvexComponentsQR(baseSectionLength, null, AbsEps, RelativeEps);
//            smallComponents = new ConvexComponentsQR(oracle.N, null, AbsEps, RelativeEps);
//            smallComponents.Resize(baseSectionLength);
//            bigExtremeBase = new double[oracle.N];
//            smallExtremeBase = new double[baseSectionLength];
//            bigX = new double[oracle.N];
//            smallX = new double[baseSectionLength];
//            bigOrder = new int[oracle.N];
//            reduceList = new List<int>();
//            deleteList = new List<int>();
//            list = new List<int>();
//            smallRemainder = new int[baseSectionLength];
//            cnt = new int[oracle.N];
//        }

//        private void CalcMinimumNormPoint(SubmodularOracle oracle, ConvexComponents components, double[] extremeBase, double[] tmpX, int state, bool needInitialize, long maxIteration = long.MaxValue)
//        {
//            if (oracle.Count == 0)
//            {
//                return;
//            }
//            if (needInitialize)
//            {
//                components.Clear();
//                var order = Enumerable.Range(0, tmpX.Length).ToArray();
//                GetBase(oracle, extremeBase, state, order);
//                components.Add(extremeBase, order);
//            }
//            double currentNorm = double.MaxValue;
//            long iteration = 0;
//            while (iteration < maxIteration)
//            {
//                iteration++;
//                components.SetX();
//                Check(oracle, components, f);
//                if (components.X.Max() <= 0 || components.X.Min() > 0)
//                {
//                    break;
//                }
//                double nextNorm = components.CalcSquareKernel(components.X);
//                if (currentNorm <= nextNorm)
//                {
//                    break;
//                }//if
//                currentNorm = nextNorm;
//                var data = CalcLinearMinimizer(oracle, tmpX, state, components.X, extremeBase);
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

//        private void Check(SubmodularOracle oracle, ConvexComponents components, List<int> f)
//        {
//            var r = new List<int>();
//            for (int i = 0; i < components.X.Length; i++)
//            {
//                if (components.X[i] <= 0)
//                {
//                    r.Add(oracle.remainder[i]);
//                }
//            }
//            Console.WriteLine(r);
//            int cc = 0;
//            for (int i = 0; i < components.X.Length; i++)
//            {
//                if (components.X[i] <= 0 ^ aa[i] == '1')
//                {
//                    cc++;
//                }
//            }
//            //f.Add(cc);
//            f.Add(r.Count);
//        }

//        private static void GetBase(SubmodularOracle oracle, double[] extremeBase, int state, int[] order)
//        {
//            if (state == All)
//            {
//                oracle.CalcBase(order, extremeBase);
//            }
//            else if (state == Head)
//            {
//                oracle.CalcBaseHead(order, extremeBase);
//            }
//            else if (state == Tail)
//            {
//                oracle.CalcBaseTail(order, extremeBase);
//            }
//        }

//        private int[] CalcLinearMinimizer(SubmodularOracle oracle, double[] tmpX, int state, double[] x, double[] extremeBase)
//        {
//            var order = new int[tmpX.Length];
//            for (int i = 0; i < order.Length; i++)
//            {
//                order[i] = i;
//                tmpX[i] = x[i];
//            }
//            Array.Sort(tmpX, order);
//            GetBase(oracle, extremeBase, state, order);
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
