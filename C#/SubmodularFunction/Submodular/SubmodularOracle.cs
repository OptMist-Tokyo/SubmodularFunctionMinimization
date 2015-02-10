using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{

    /// <summary>
    /// This is a base class.
    /// </summary>
    public abstract class SubmodularOracle
    {
        Stopwatch timer;


        protected SubmodularOracle()
        {
            timer = new Stopwatch();
            OracleCall = 0;
            BaseCall = 0;
        }

        public void Clear()
        {
            timer.Reset();
            OracleCall = 0;
            BaseCall = 0;
        }

        protected void SetVariable(int n,double fOfEmpty)
        {
            this.BaseN = this.N = n;//this.ReducedN= n;
            //this.ReducedFOfEmpty = this.fOfEmpty = fOfEmpty;
            //remainder = new int[n];
            //for (int i = 0; i < n; i++)
            //{
            //    remainder[i] = i;
            //}//for i
        }

        /// <summary>
        /// Get the cardinality of the ground set.
        /// </summary>
        public virtual int N
        {
            get;
            private set;
        }

        public int BaseN
        {
            get;
            private set;
        }

        //public int ReducedN
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// Get the value F(\emityset).
        /// </summary>
        public double fOfEmpty
        {
            get;
            private set;
        }

        public long OracleCall
        {
            get;
            private set;
        }

        public  long BaseCall
        {
            get;
           private set;
        }

        public long OracleTime
        {
            get { return timer.ElapsedMilliseconds; }
        }

        //public int CntContracted
        //{
        //    get;
        //    protected set;
        //}

        //public int CntErased
        //{
        //    get;
        //    protected set;
        //}

        //public double ReducedFOfEmpty
        //{
        //    get;
        //    protected set;
        //}

        //public int ReducedN
        //{
        //    get { return N - CntContracted - CntErased; }
        //}

        public double CalcValue(bool[] X)
        {
            var mask = new StringBuilder();
            for (int i = 0; i < X.Length; i++)
            {
                mask.Append(X[i] ? "1" : "0");
            }//for i
            int usedBit;
            var order = GetOrder(mask.ToString(), out usedBit);
            double res = CalcValue(order, usedBit);
            return res;
        }

        /// <summary>
        /// Calculate the value f(X), where X:= union of (0->cardinality -1) order[i].
        /// </summary>
        /// <iaram name="order"></iaram>
        /// <iaram name="cardinality">|X|</iaram>
        /// <returns></returns>
        public double CalcValue(int[] order, int cardinality)
        {
            timer.Start();
            OracleCall++;
            double res = Value(order, cardinality);
            timer.Stop();
            return res;
        }

        internal abstract double Value(int[]order,int cardinality);

        public void CalcBase(int[] order,double[] b)
        {
            timer.Start();
            BaseCall++;
            Base(order, b);
            timer.Stop();
        }

        internal virtual void Base(int[] order, double[] b)
        {
            double sum = fOfEmpty;
            for (int i = 0; i < order.Length; i++)
            {
                double curValue = CalcValue(order, i + 1);
                b[order[i]] = curValue - sum;
                sum = curValue;
            }//for i
        }

        public virtual string GetMinimizer(bool[] X)
        {
            var minimizer = new StringBuilder();
            for (int i = 0; i < X.Length; i++)
            {
                minimizer.Append((X[i] ? "1" : "0"));
            }//for i
            return minimizer.ToString();
        }

        //public void Contract(int element)
        //{
        //    EraseAnElement(element);
        //    CntContracted++;
        //    ContractDerived(element);
        //}

        //protected abstract void ContractDerived(int element);

        //public void Eerase(int element)
        //{
        //    EraseAnElement(element);
        //    CntErased++;
        //}

        //private void EraseAnElement(int element)
        //{
        //    for (int i = 0; i < remainder.Length; i++)
        //    {
        //        if (remainder[i] == element)
        //        {
        //            var tmpVariable = remainder[i];
        //            remainder[i] = remainder[N - CntErased - 1];
        //            remainder[N - CntErased - 1] = tmpVariable;
        //        }//if
        //    }//for i
        //    throw new ArgumentException("We cannot find element.");
        //}

        /// <summary>
        /// Check wheather this function is submodular
        /// It takes O(2^n * n^2) time.
        /// </summary>
        /// <returns></returns>
        public  bool IsSubmodular()
        {
            for (int mask = 0; mask < (1<<N); mask++)
            {
                int usedBit;
                var order = GetOrder(N, mask,out usedBit);
                for (int i = usedBit; i < N; i++)
                {
                    Swap(order, usedBit, i);
                    double a =CalcValue(order, usedBit);
                    double b =CalcValue(order, usedBit+1);
                    for (int j = i+1; j < N; j++)
                    {
                        Swap(order, usedBit + 1, j);
                        double d = CalcValue(order, usedBit + 2);
                        Swap(order, usedBit, usedBit + 1);
                        double c = CalcValue(order, usedBit+1);
                        Swap(order, usedBit, usedBit + 1);
                        if (b+c<a+d-1e-9)
                        {
                            return false;
                        }//if
                        Swap(order, usedBit + 1, j);
                    }//for j
                    Swap(order, usedBit, i);
                }//for i
            }//for mask
            return true;
        }

        private static void Swap(int[] order, int x, int y)
        {
            var tmp = order[x];
            order[x] = order[y];
            order[y] = tmp;
        }

        public static int[] GetOrder(int n,int mask,out int usedBit)
        {
            var order = new int[n];
            usedBit = 0;
            for (int i = 0; i < n; i++)
            {
                if ((mask >> i & 1) == 1)
                {
                    order[usedBit++] = i;
                }//if
                else
                {
                    order[n - 1 - (i - usedBit)] = i;
                }//else
            }//for i
            return order;
        }

        public static int[] GetOrder(string mask, out int usedBit)
        {
            usedBit = 0;
            int n = mask.Length;
            var order = new int[n];
            for (int i = 0; i < n; i++)
            {
                if (mask[i]=='0')
                {
                    order[n - 1 - (i - usedBit)] = i;
                }//if
                else
                {
                    order[usedBit++] = i;
                }//else
            }//for i
            return order;
        }



        [Conditional("DEBUG")]
        public void TestGetBase(int[] order)
        {
            double[] b0 = new double[order.Length];
            double[] b1 = new double[order.Length];
            Base(order, b0);

            double sum = fOfEmpty;
            for (int i = 0; i < N; i++)
            {
                double curValue = CalcValue(order, i + 1);
                b1[order[i]] = curValue - sum;
                sum = curValue;
            }//for i

            var ok = true;
            for (int i = 0; i < b0.Length; i++)
            {
                if (Math.Abs(b0[i]-b1[i])<1e-9||Math.Abs(b0[i]-b1[i])/Math.Max(Math.Abs(b0[i]),Math.Abs(b1[i]))<1e-9)
                {
                    
                }
                else
                {
                    ok = false;
                    break;
                }
            }

            //if (!b0.SequenceEqual(b1,(x,y)=>Math.Abs(x-y)<1e-9))
            if(!ok )
            {
                var type = this.GetType();
                var message = "The imilementation of TestGetBase() of " + type.ToString() + " is wrong.";
                throw new Exception(message);
            }//if
        }


    }
}
