using Microsoft.VisualStudio.TestTools.UnitTesting;
using Onigiri.Submodular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
namespace Onigiri.TestSubmodular
{
    [TestClass]
    public class IsSubmodular
    {

        [TestMethod]
        public void IsSubmodular0()
        {
            const int n = 3;
            var array = new double[1 << n] { 0, 0, 0, 0, 0, 0, 0, 0 };

            var func = new Manual(n, array);
            Assert.AreEqual(IsSubmodular.IsSubmodularTrivially(func), func.IsSubmodular());
        }

        [TestMethod]
        public void IsSubmodular1()
        {
            const int n=3;
            var array = new double[1<<n] { 0, 0, 0, 0, 0, 0, 0, 3 };

            var func = new Manual(n, array);
            Assert.AreEqual(IsSubmodular.IsSubmodularTrivially(func), func.IsSubmodular());
        }

        [TestMethod]
        public void IsSubmodular2()
        {
            const int n = 3;
            var array = new double[1<<n] { 0, 2,3,4,6,-1,5,-10};

            var func = new Manual(n, array);
            Assert.AreEqual(IsSubmodular.IsSubmodularTrivially(func), func.IsSubmodular());
        }

        [TestMethod]
        public void IsSubmodular3()
        {
            const int nMin = 1;
            const int nMax = 9;
            const int rei = 10000;
            const int max = 10000;
            var rand = new Random(0);

            for (int i = nMin; i < nMax; i++)
            {
                var res = RepeatMany(i, rei, max, rand);
                Assert.AreEqual(true, res);                
            }//for i
        }        

        private static bool IsSubmodularTrivially(Manual func)
        {
            int max = 1 << func.N;
            for (int i = 0; i < max; i++)
            {
                for (int j = i + 1; j < max; j++)
                {
                    if (func.Value(i) + func.Value(j) < func.Value(i & j) + func.Value(i | j))
                    {
                        return false;
                    }//if
                }//for j
            }//for i
            return true;
        }

        private static bool RepeatMany(int n, int rei, int max, Random rand)
        {
            var res = true;
            for (int i = 0; i < rei; i++)
            {
                var array = new double[1 << n];
                for (int k = 0; k < array.Length; k++)
                {
                    array[k] = rand.Next(max) - rand.Next(max);
                }//for k
                var func = new Manual(n, array);
                var want = IsSubmodular.IsSubmodularTrivially(func);
                var ans = func.IsSubmodular();
                res &= (want == ans);
            }//for i
            return res;
        }
        

    }
}
        
#endif