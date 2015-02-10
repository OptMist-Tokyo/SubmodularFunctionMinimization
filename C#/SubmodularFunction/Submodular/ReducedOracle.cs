using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class ReducedOracle : OracleWithOperation
    {
        protected List<int> remainder;    //vertices which is still remained
        protected bool[] reduced;   //for memorization
        HashSet<int> hash;
        List<int> subRemainder;

        public ReducedOracle(SubmodularOracle oracle, bool reduce = true)
        {
            Initialize(oracle);
            ReducedFOfEmpty = fOfEmpty;
            this.reduced = new bool[base.N];
            this.remainder = new List<int>();
            subRemainder = new List<int>();
            hash = new HashSet<int>();
            if (reduce)
            {
                Reduce();
                Delete();
            }
            SetHash();
        }

        public double ReducedFOfEmpty
        {
            get;
            private set;
        }

        public int[] SetHash(int length  = 0)
        {
            int[] inverse = null;
            int[] reorder = null;
            if (length>0)
            {
                inverse = new int[BaseN];
                for (int i = 0; i < length; i++)
                {
                    inverse[remainder[i]] = i;
                }
                reorder = Enumerable.Repeat(-1, length).ToArray();
            }

            remainder.Clear();
            for (int i = CountReduced; i < BaseN-CountDeleted; i++)
            {
                int cur = order[i];
                remainder.Add(cur);
            }//for i
            if (length>0)
            {
                for (int i = 0; i < remainder.Count; i++)
                {
                    reorder[inverse[remainder[i]]] = i;
                }
            }
            return reorder;
        }

        private void SetHash(List<int> list)
        {
            hash.Clear();
            foreach (var item in list)
            {
                hash.Add(remainder[item]);
            }
        }

        private void Delete()
        {
            double deletedFOfAll = oracle.CalcValue(order, BaseN);
            for (int i = order.Length - 1; i >= CountReduced; i--)
            {
                var tmpVariable = order[order.Length-1-CountDeleted];
                order[order.Length-1-CountDeleted] = order[i];
                order[i] = tmpVariable;
                double curVal = oracle.CalcValue(order, order.Length - 1 - CountDeleted);
                if (deletedFOfAll-curVal>=0)
                {
                    CountDeleted++;
                    deletedFOfAll = curVal;
                }//if
            }//forrev i
        }

        public void Delete(List<int>deleteList)
        {
            SetHash(deleteList);
            for (int i = order.Length - 1 - CountDeleted; i >= CountReduced; i--)
            {
                int cur = order[i];
                if (hash.Contains(cur))
                {
                    var tmpVariable = order[order.Length-1-CountDeleted];
                    order[order.Length-1-CountDeleted] = order[i];
                    order[i] = tmpVariable;
                    CountDeleted++;
                }
            }
        }

        private void Reduce()
        {
            for (int i = 0; i < order.Length; i++)
            {
                var tmpVariable = order[CountReduced];
                order[CountReduced] = order[i];
                order[i] = tmpVariable;
                double curVal = oracle.CalcValue(order, CountReduced + 1);
                if (curVal-ReducedFOfEmpty<=0)
                {
                    reduced[order[CountReduced]] = true;
                    CountReduced++;
                    ReducedFOfEmpty = curVal;
                }//if
            }//for i
        }

        public void Reduce(List<int> reduceList)//,double reducedValue)
        {
            //ReducedFOfEmpty += reducedValue;
            SetHash(reduceList);
            for (int i = CountReduced; i < order.Length-CountDeleted; i++)
            {
                int cur = order[i];
                if (hash.Contains(cur))
                {
                    var tmpVariable = order[CountReduced];
                    order[CountReduced] = order[i];
                    order[i] = tmpVariable;
                    reduced[order[CountReduced]] = true;
                    CountReduced++;
                }
            }
        }

        public override string GetMinimizer(bool[] X)
        {
            UpdateContained(X, true);
            StringBuilder minimizer = new StringBuilder();
            for (int i = 0; i < reduced.Length; i++)
            {
                minimizer.Append((reduced[i] ? "1" : "0"));
            }//for i
            UpdateContained(X, false);
            return minimizer.ToString();
        }

        private void UpdateContained(bool[] X,bool color)
        {
            for (int i = 0; i < X.Length; i++)
            {
                if (X[i])
                {
                    reduced[remainder[i]] = color;
                }//if
            }//for i
        }

        protected override void CopyB(int[] order, double[] b)
        {
            for (int i = 0; i < N;i++)
            {
                int cur = order[i];
                b[cur] = this.b[remainder[cur]];
            }//foreach cur
        }

        protected override int SetExtendedOrder(int[] order,int cardinality = -1)
        {
            for (int i = 0; i < N; i++)
            {
                this.order[CountReduced + i] = remainder[order[i]];
            }//for i
            return cardinality;
        }

        public List<int> CopyRemainder()
        {
            return remainder.ToList();
        }

        public int GetCountReduced()
        {
            return CountReduced;
        }

        public int GetCountDeleted()
        {
            return CountDeleted;
        }

        internal void SortOrder(int[]order)
        {
            for (int i = 0; i < remainder.Count; i++)
            {
                this.order[CountReduced + i] = remainder[order[i]];
            }
        }

        internal void SetNewState(int[]order,int reduce, int delete,int length)
        {
            CountReduced += reduce;
            CountDeleted += delete;
            subRemainder.Clear();
            for (int i = reduce; i < reduce+length; i++)
            {
                subRemainder.Add(remainder[order[i]]);
            }
            remainder = subRemainder;
        }

        internal void Reverse(List<int> copyRemainder, int countReduce, int countDelete)
        {
            remainder = copyRemainder;
            CountReduced = countReduce;
            CountDeleted = countDelete;
        }
    }
}
