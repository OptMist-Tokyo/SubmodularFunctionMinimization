using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class ContractOracle : OracleWithOperation
    {
        Dictionary<int,List<int>> remainder;
        List<int> reducedVertices;
        List<int> deletedVertices;
        int addedReduce;    //for calculation
        int addedDelete;    //for calculation
        List<int> notInRemainder;

        public ContractOracle(SubmodularOracle oracle)
        {
            Initialize(oracle);
            reducedVertices = new List<int>();
            deletedVertices = new List<int>();
            notInRemainder = new List<int>();
            remainder = new Dictionary<int, List<int>>();
            addedReduce = addedDelete = 0;
            ReducedFOfEmpty = fOfEmpty;
            for (int i = 0; i < BaseN; i++)
            {
                remainder[i] = new List<int>();
                remainder[i].Add(i);
            }//for i
        }

        protected override int CountDeleted
        {
            get { return deletedVertices.Count; }
        }

        protected override int CountReduced
        {
            get { return reducedVertices.Count; }
        }

        public  double ReducedFOfEmpty
        {
            get;
           protected set;
        }

        public List<int> this[int index]
        {
            get { return remainder[index]; }
        }

        public void Delete(int v)
        {
            foreach (var cur in remainder[v])
            {
                deletedVertices.Add(cur);
            }//foreach cur
            CountDeleted += remainder[v].Count;
            CountContracted -= remainder[v].Count - 1;
            addedDelete += remainder[v].Count;
            remainder.Remove(v);
        }

        public void Reduce(int v)
        {
            foreach (var cur in remainder[v])
            {
                reducedVertices.Add(cur);
            }//foreach cur
            CountReduced += remainder[v].Count;
            CountContracted -= remainder[v].Count - 1;
            addedReduce += remainder[v].Count;
            remainder.Remove(v);
        }

        public void Reduce(HashSet<int> reachable,out int  memoCountContracted,out double memoReducedFOfEmpty)
        {
            RemoveFromReachable(reachable);
            memoCountContracted = CountContracted;
            memoReducedFOfEmpty = ReducedFOfEmpty;
            SetReducedAndDeletedOrder();
            foreach (var v in reachable)
            {
                foreach (var cur in remainder[v])
                {
                    reducedVertices.Add(cur);
                }//foreach cur
                CountReduced += remainder[v].Count;
                CountContracted -= remainder[v].Count - 1;
                addedReduce += remainder[v].Count;
            }//foreach v
        }

        private void RemoveFromReachable(HashSet<int> reachable)
        {
            notInRemainder.Clear();
            foreach (var v in reachable)
            {
                if (!remainder.ContainsKey(v))
                {
                    notInRemainder.Add(v);
                }//if
            }//foreach v
            foreach (var v in notInRemainder)
            {
                reachable.Remove(v);
            }//foreach v
        }

        public void ReviveBeforeReducing(HashSet<int> reachable, int memoCountContracted, double memoReducedFOfEmpty)
        {
            CountContracted = memoCountContracted;
            addedReduce = 0;
            ReducedFOfEmpty = memoReducedFOfEmpty;
            foreach (var v in reachable)
            {
                foreach (var cur in remainder[v])
                {
                    reducedVertices.Remove(cur);
                }//foreach cur
            }//foreach v
        }

        //private void SwapAndRemove(int v)
        //{
        //    var tmpVariable = remainder[remainder.Count-1];
        //    remainder[remainder.Count-1] = remainder[v];
        //    remainder[v] = tmpVariable;
        //}

        public void Contract(HashSet<int> contractHash,int head)
        {
            foreach(int cur in contractHash)
            {
                if (cur==head)
                {
                    continue;
                }//if
                foreach (var v in remainder[cur])
                {
                    remainder[head].Add(v);
                }//foreach v
                remainder.Remove(cur);
            }//for i
            CountContracted += contractHash.Count - 1;
        }

        protected override void CopyB(int[] order, double[] b)
        {
            for (int i = 0; i < N; i++)
            {
                int cur = order[i];
                double sum = 0;
                foreach (var v in remainder[cur])
                {
                    sum += this.b[v];
                }//foreach v
                b[cur] = sum;
            }//for i
        }

        protected override int SetExtendedOrder(int[] order,int cardinality = -1)
        {
            SetReducedAndDeletedOrder();
            int pos = CountReduced;
            for(int i=0;i<N;i++)
            {
                if (pos < cardinality+CountReduced)
                {
                    cardinality += remainder[order[i]].Count - 1;
                }//if
                foreach (var v in remainder[order[i]])
                {
                    this.order[pos++] = v;    
                }//foreach v
            }//for i
            return cardinality;
        }

        private void SetReducedAndDeletedOrder()
        {
            if (addedReduce > 0)
            {
                for (int i = CountReduced - addedReduce; i < CountReduced; i++)
                {
                    this.order[i] = reducedVertices[i];
                    ReducedFOfEmpty += b[this.order[i]];
                }//for i                
                addedReduce = 0;
            }//if
            if (addedDelete > 0)
            {
                int end = this.order.Length - 1 - (CountDeleted - addedDelete);
                for (int i = CountDeleted - addedDelete; i < CountDeleted; i++)
                {
                    this.order[end--] = deletedVertices[i];
                }//for i
                addedDelete = 0;
            }//if
        }

    }
}
