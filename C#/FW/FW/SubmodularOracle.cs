using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.FW
{
    public abstract class SubmodularOracle
    {
        public int[] remainder;
        bool[] used;
        bool[] minimizer;
        int[] inverse;
        protected Stopwatch sw;

        public int N { get; private set; }
        public int CountContract { get; private set; }
        public int CountDelete { get; private set; }
        public int Count { get; private set; }
        public bool[] Minimizer { get { return minimizer; } }

        public abstract double EmptyValue { get; }
        public abstract double FullValue { get; }
        public abstract void Contract(List<int> reduceList);
        public abstract void Delete(List<int> deleteList);
        protected abstract void CalcBaseInherent(int[] order, double[] b);
        protected abstract void CalcBaseHeadInherent(int[] order, double[] b);
        protected abstract void CalcBaseTailInherent(int[] order, double[] b);
        public abstract SubmodularOracle Copy();

        public long OracleTime
        {
            get { return sw.ElapsedMilliseconds; }
        }

        public long BaseCall
        {
            get;
            private set;
        }

        public void SetParentVariable(int n)
        {
            N = Count = n;
            CountContract = CountDelete = 0;
            remainder = Enumerable.Range(0, n).ToArray();
            used = new bool[n];
            minimizer = new bool[n];
            inverse = new int[n];
            sw = new Stopwatch();
            BaseCall = 0;
            //tmpB = new double[n];
            //tmpOrder = Enumerable.Range(0, n).ToArray();
            //rand = new Random(0);
        }


        public void CalcBase(int[] order, double[] b)
        {
            sw.Start();
            BaseCall++;
            CalcBaseInherent(order, b);
            sw.Stop();
        }

        //double[] tmpB;
        //int[] tmpOrder;
        //Random rand;

        public void CalcBaseHead(int[] order, double[] b)
        {
            sw.Start();
            BaseCall++;
            CalcBaseHeadInherent(order, b);
            sw.Stop();

            //GetLongBase();
        }
        
        public void CalcBaseTail(int[] order, double[] b)
        {
            sw.Start();
            BaseCall++;
            CalcBaseTailInherent(order, b);
            sw.Stop();

            //GetLongBase();
        }

        //private void GetLongBase()
        //{
        //    var s = rand.Next(N);
        //    var t = rand.Next(N);
        //    var tmpVariable = tmpOrder[s];
        //    tmpOrder[s] = tmpOrder[t];
        //    tmpOrder[t] = tmpVariable;
        //    CalcBaseInherent(tmpOrder, tmpB);
        //}
        
        public void SetRemainder(List<int> reduceList, List<int> deleteList,int[]reorder = null)
        {
            if (reorder!=null)
            {
                for (int i = 0; i < remainder.Length; i++)
                {
                    inverse[remainder[i]] = i;
                    reorder[i] = -1;
                }
            }
            for (int i = 0; i < remainder.Length; i++)
            {
                used[i] = false;
            }
            foreach (var item in reduceList)
            {
                used[item] = true;
                minimizer[remainder[item]] = true;
            }
            foreach (var item in deleteList)
            {
                used[item] = true;
            }
            int index = 0;
            if (reorder != null)
            {
                for (int i = 0; i < remainder.Length; i++)
                {
                    if (!used[i])
                    {
                        reorder[inverse[remainder[i]]] = index++;
                    }
                }
                index = 0;
            }
            for (int i = 0; i < remainder.Length; i++)
            {
                if (!used[i])
                {
                    remainder[index++] = remainder[i];
                }
            }
            CountContract += reduceList.Count;
            CountDelete += deleteList.Count;
            Count = N - CountContract - CountDelete;
            Array.Resize(ref remainder, Count);
        }

        protected void Copy(int N ,int[]remainder,int countReduce,int countDelete,int count,Stopwatch sw)
        {
            //this.BaseCall = 0;
            this.N = N;
            this.remainder = new int[remainder.Length];
            this.used = new bool[count];
            this.minimizer = new bool[N];
            this.CountContract = countReduce;
            this.CountDelete = countDelete;
            this.Count = count;
            for (int i = 0; i < remainder.Length; i++)
            {
                this.remainder[i] = remainder[i];
            }
            this.inverse = new int[N];
            this.sw = sw;
        }


        
        public void AddBaseCall(long subCall)
        {
            BaseCall += subCall;
        }

    }
}
