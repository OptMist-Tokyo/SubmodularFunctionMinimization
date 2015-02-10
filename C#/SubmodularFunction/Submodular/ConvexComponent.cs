
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class ConvexComponent
    {
        int[] order;
        double[] b;

        internal ConvexComponent(int n)
        {
            Lambda = 0;
            order = new int[n];
            b = new double[n];
        }

        public double Lambda
        {
            get;
            set;
        }

        public int[] Order
        {
            get { return order; }
        }

        public double[] B
        {
            get { return b; }
        }

        //internal void Set(double lambda, int[] order, double[] b,List<int> hash)
        //{
        //    this.Lambda = lambda;
        //    if (this.order == null)
        //    {
        //        this.order = new int[order.Length];
        //        this.b = new double[b.Length];
        //    }//if
        //    foreach (var index in hash)
        //    {
        //        this.order[index] = order[index];
        //        this.b[index] = b[index];
        //    }//foreach index
        //}

    }

    public class ConvexComponentWithID : ConvexComponentWithLevel
    {
        internal ConvexComponentWithID(int n)
            : base(n)
        {
        }

        public long ID
        {
            get;
            set;
        }


        internal void Set(long id,double lambda, int[] levels, double[]b,int[]order)
        {
            this.Lambda = lambda;
            this.ID = id;
            for (int i = 0; i < b.Length; i++)
            {
                this.B[i] = b[i];
                this.Levels[i] = levels[i];
                this.Order[i] = order[i];
            }//for i
        }

    }

    public class ConvexComponentWithInverseOrder : ConvexComponent
    {
        int[] inverseOrder;
        internal ConvexComponentWithInverseOrder(int n)
            : base(n)
        {
            inverseOrder = new int[n];
        }

        public int[] InverseOrder
        {
            get { return inverseOrder; }
        }

    }

    public class ConvexComponentWithLevel : ConvexComponent
    {
        int[] levels;
        internal ConvexComponentWithLevel(int n)
            : base(n)
        {
            levels = new int[n];
        }

        public int[] Levels
        {
            get { return levels; }
        }

    }

    public class ComponentOrlin
    {
        double[] b;
        int[] levels;

#if DEBUG
      public  int[] Order;
#endif

        internal ComponentOrlin(int n)
        {
            b = new double[n];
            levels = new int[n];
        }

        public long ID
        {
            get;
            set;
        }

        public long SumOfLevels
        {
            get;
            set;
        }


        public int[] Levels
        {
            get { return levels; }
        }

        public double[] B
        {
            get { return b; }
        }

        public int CountUsed
        {
            get;
            set;
        }


        internal int ComiareTo(int v, ComponentOrlin other)
        {
            int res = Math.Sign(this.levels[v] - other.levels[v]);
            if (res != 0)
            {
                return res;
            }//if
            res = Math.Sign(this.SumOfLevels - other.SumOfLevels);
            if (res != 0)
            {
                return res;
            }//if
            return Math.Sign(this.ID - other.ID);
        }
    }

}
