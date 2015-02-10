using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Onigiri.Submodular;

namespace Onigiri.TestSubmodular
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class QRDecomposition
    {
        const double AbsEps = 1e-10;
        const double RelativeEps = 1e-12;


        [TestMethod]
        public void QRDecomposition0()
        {
            int row = 5;
            int col = 5;
            Random rand = new Random(0);

            ExecuteTest(row, col, rand);
        }

        [TestMethod]
        public void QRDecomposition1()
        {
            int row = 5;
            int col = 10;
            Random rand = new Random(0);

            ExecuteTest(row, col, rand);
        }

        [TestMethod]
        public void QRDecomposition2()
        {
            int row = 5;
            int col = 20;
            Random rand = new Random(0);

            ExecuteTest(row, col, rand);
        }

        [TestMethod]
        public void QRDecomposition3()
        {
            int row = 15;
            int col = 20;
            Random rand = new Random(0);

            ExecuteTest(row, col, rand);
        }

        [TestMethod]
        public void QRDecomposition4()
        {
            int row = 50;
            int col = 20;
            Random rand = new Random(0);

            ExecuteTest(row, col, rand);
        }

        [TestMethod]
        public void QRDecomposition5()
        {
            int row = 50;
            int col = 200;
            Random rand = new Random(0);

            ExecuteTest(row, col, rand);
        }


        [TestMethod]
        public void QRDecomposition6()
        {
            int row = 50;
            int col = 200;
            Random rand = new Random(0);

            for (int i = 1; i < row; i++)
            {
                for (int j = 1; j < col; j++)
                {
                    ExecuteTest(i, j, rand);
                }//for j
            }//for i
        }

        private static void ExecuteTest(int row, int col, Random rand)
        {
            ConvexComponents cc = new ConvexComponents(row, AbsEps, RelativeEps);
            for (int i = 0; i < col; i++)
            {
                var c = cc.GetNewComponent();
                c.Lambda = 1.0 / col;
                for (int j = 0; j < c.B.Length; j++)
                {
                    c.B[j] = rand.NextDouble();
                }//for j
            }//for i
            cc.Reduce(col);
        }


    }
}
