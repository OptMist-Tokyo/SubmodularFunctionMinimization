using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public abstract class OracleWithOperation:SubmodularOracle
    {
        protected SubmodularOracle oracle;    //base oracle
        protected int[] order;
        protected double[] b;

        public override int N
        {
            get { return BaseN - CountReduced - CountDeleted -CountContracted; }
        }

        protected virtual int CountDeleted
        {
            get;
            set;
        }

        protected virtual int CountReduced
        {
            get;
            set;
        }

        protected int CountContracted
        {
            get;
            set;
        }

        protected void Initialize(SubmodularOracle oracle)
        {
            SetVariable(oracle.N, oracle.fOfEmpty);
            this.oracle = oracle;
            this.order = Enumerable.Range(0, base.N).ToArray();
            this.b = new double[base.N];
            CountReduced = 0;
            CountDeleted = 0;
            CountContracted = 0;
        }

        internal override double Value(int[] order, int cardinality)
        {
            cardinality = SetExtendedOrder(order, cardinality);
            return oracle.Value(this.order, CountReduced + cardinality);
        }

        internal override void Base(int[] order, double[] b)
        {
            SetExtendedOrder(order);
            oracle.Base(this.order, this.b);
            CopyB(order, b);
        }

        protected abstract void CopyB(int[] order, double[] b);

        protected abstract int SetExtendedOrder(int[] order,int cardinality = -1);



    }
}
