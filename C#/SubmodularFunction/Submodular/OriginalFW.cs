using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onigiri.Algorithm;

namespace Onigiri.Submodular
{
    public class OriginalFW:WolfeMinimumNormPoint
    {
        const int baseSectionLength = 20;
        const int baseNumIteration = 50;
        ReducedOracle ro;
        List<int> delete;
        List<int> reduce;

        public OriginalFW(LinearOptimization LOAlgorithm,double[][]kernelMatrix=null, double absEps=DefaultAbsEps, double relativeEps=DefaultRelativeEps):base (LOAlgorithm,kernelMatrix,absEps,relativeEps)
        {
            delete = new List<int>();
            reduce = new List<int>();
            ro = ((SubmodularLinearOptimization)LOAlgorithm).Oracle as ReducedOracle;
        }


        public override double[] CalcMinimumNormPoint(long maxReputation = long.MaxValue)
        {
            int numIteration = baseNumIteration;
            int sectionLength = baseSectionLength;
            while (true)
            {
                if (ro.N >= 2 * sectionLength)
                {
                    var x = base.CalcMinimumNormPoint(numIteration);
                    var order = Enumerable.Range(0, x.Length).ToArray();
                    Array.Sort(x, order);
                    SectionReduce(x,order,sectionLength);
                    SectionDelete(x,order,sectionLength);
                    if (reduce.Count==0&&delete.Count==0)
                    {
                        numIteration += baseNumIteration;
                        sectionLength += baseSectionLength;
                    }
                    if (reduce.Count>0)
                    {
                        ro.Reduce(reduce);
                    }
                    if (delete.Count>0)
                    {
                        ro.Delete(delete);
                    }
                    ro.SetHash();
                    components.Resize(ro.N);
                    //var reorder = ro.SetHash(x.Length);
                    //Array.Sort(order, reorder);
                    //Reset(reorder);
                }
                else
                {
                    return base.CalcMinimumNormPoint();
                }
            }
        }

        private void Reset(int[] reorder)
        {
            int n = ro.N;
            ((SubmodularLinearOptimization)LOAlgorithm).Resize(n);
            components.Resize(n);
            Array.Resize(ref extremeBase, n);
            Array.Resize(ref b, n);

            //if (a == 3)
            //{
            components.Clear();
            //var data = LOAlgorithm.GetInitialBase(b);
            //components.Add(b, data);
            //}
            //components.Shuffle(reorder);
            //components.SetX();
            //currentNorm = double.MaxValue;
        }

        private void SectionDelete(double[] x,int[]order,int sectionLength)
        {
            delete.Clear();
            for (int i = x.Length - sectionLength; i >= 0 && x[i ] > 0; i -= sectionLength)
            {
                double[] subX = ExecuteWolfeAlgorithm(i,order,sectionLength);
                if (subX.Min() < 0)
                {
                    break;
                }
                for (int j = 0; j < sectionLength; j++)
                {
                    delete.Add(order[i + j]);
                }
            }
        }
        
        private void SectionReduce(double[] x,int[]order,int sectionLength)
        {
            reduce.Clear();
            for (int i = 0; i + sectionLength < x.Length && x[i + sectionLength - 1] <= 0; i += sectionLength)
            {
                double[] subX = ExecuteWolfeAlgorithm(i,order,sectionLength);
                if (subX.Max() > 0)
                {
                    break;
                }
                for (int j = 0; j < sectionLength; j++)
                {
                    reduce.Add(order[i + j]);
                }
            }
        }

        private double[] ExecuteWolfeAlgorithm(int index,int[]order,int sectionLength)
        {
            var copyRemainder = ro.CopyRemainder();
            var countReduce = ro.GetCountReduced();
            var countDelete = ro.GetCountDeleted();
            ro.SortOrder(order);
            ro.SetNewState(order,index, ro.N - sectionLength - index,sectionLength);
            var memo = components.X;
            Resize(sectionLength);
            double[] res = base.CalcMinimumNormPoint();
            ro.Reverse(copyRemainder, countReduce, countDelete);
            Resize(order.Length);
            return res;
        }

        private void Resize(int length)
        {
            ((SubmodularLinearOptimization)LOAlgorithm).Resize(length);
            components.Resize(length);
            Array.Resize(ref extremeBase, length);
            Array.Resize(ref b, length);
        }

    }
}
