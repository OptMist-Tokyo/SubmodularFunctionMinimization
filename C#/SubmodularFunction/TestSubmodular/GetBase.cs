using Microsoft.VisualStudio.TestTools.UnitTesting;
using Onigiri.Submodular;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
namespace Onigiri.TestSubmodular
{
    [TestClass]
    public class GetBase
    {
        [TestMethod]
        public void GetBase0()
        {
            const int n = 3;
            var array= new double[n] {9,89,7};
            var order = new int[n]{0,1,2};

            var func = new Modular(n, array);
            var manual = ConvertModularToManual(n, func);
            double[] b0 = new double[order.Length];
            double[] b1 = new double[order.Length];
            func.CalcBase(order,b0);
            manual.CalcBase(order,b1);
            Assert.AreEqual(true, b0.SequenceEqual(b1));
        }

        [TestMethod]
        public void GetBase1()
        {
            const int nMin = 1;
            const int nMax = 10;
            const int max = 10000;
            var rand = new Random(0);

            for (int i = nMin; i < nMax; i++)
            {
                var order = Enumerable.Range(0, i).ToArray();
                var array = Enumerable.Range(0, i).Select(x => (double)rand.Next(max) - rand.Next(max)).ToArray();
                var func = new Modular(i, array);
                var manual = ConvertModularToManual(i,  func);
                double[] b0 = new double[order.Length];
                double[] b1 = new double[order.Length];
                 func.CalcBase(order,b0);
                manual.CalcBase(order,b1);
                Assert.AreEqual(true, b0.SequenceEqual(b1));
            }//for i

        }

        private static Manual ConvertModularToManual(int n, Modular func)
        {
            var values = new double[1 << n];
            for (int mask = 0; mask < (1 << n); mask++)
            {
                int usedBit;
                int []order = SubmodularOracle.GetOrder(n, mask, out usedBit);
                values[mask] = func.CalcValue(order, usedBit);
            }//for mask
            var manual = new Manual(n, values);
            return manual;
        }
    }
}

#endif