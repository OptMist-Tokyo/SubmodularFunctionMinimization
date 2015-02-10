#if DEBUG
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Onigiri.TestSubmodular
{
    /// <summary>
    /// CheckKyFan の概要の説明
    /// </summary>
    [TestClass]
    public class CheckKyFan
    {
        public CheckKyFan()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanui を使用してコードを実行してください
        // [ClassCleanui()]
        // public static void MyClassCleanui() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanui を使用してコードを実行してください
        // [TestCleanui()]
        // public void MyTestCleanui() { }
        //
        #endregion

        [TestMethod]
        public void MakeTriangle()
        {
            const int n = 100;
            const int rei = 10;
            const string path = @"C:\Users\onigiri\Desktop\KyFan\";
            var rand = new Random(0);
            var order = Enumerable.Range(0, n).ToArray();
            var modular = new double[n];

            for (int k = 0; k < rei; k++)
            {
                var triangle = MakeMatrix(n, rand);
                OutputTriangle(n, path, k, triangle);
                var b0 = OutputBase(n, path, k, triangle);
                var mat = OutputMatrix(n, path, k, triangle);
                var oracle = new Onigiri.Submodular.KyFan(n, modular, mat);
                var b1 = new double[n];
                oracle.CalcBase(order, b1);
                var b2 = new double[n];
                for (int i = 0; i < n; i++)
                {
                    b2[i] = oracle.CalcValue(order, i + 1) - (i == 0 ? 0 : b2[i - 1]);
                }
                double error1 = GetAverageError(n, b0, b1);
                double error2 = GetAverageError(n, b0, b2);
                var swError = new StreamWriter(path + "Error\\" + k.ToString() + ".txt");
                swError.WriteLine(error1);
                swError.WriteLine(error2);
                swError.Close();
            }

        }

        private static double GetAverageError(int n, double[] b0, double[] b1)
        {
            double error1 = 0;
            for (int i = 0; i < n; i++)
            {
                if (b0[i]==0)
                {
                    continue;
                }
                error1 += Math.Abs(b0[i] - b1[i]) / Math.Abs(b0[i]);
            }
            error1 /= n;
            return error1;
        }

        private static double[][] OutputMatrix(int n, string path, int k, double[][] triangle)
        {
            var mat = new double[n][];
            for (int i = 0; i < n; i++)
            {
                mat[i] = new double[n];
            }
            for (int i = 0; i < n; i++)
            {
                for (int q = 0; q < n; q++)
                {
                    for (int r = 0; r < n; r++)
                    {
                        mat[i][q] += triangle[i][r] * triangle[q][r];
                    }
                }
            }
            var swMatrix = new StreamWriter(path + "Matrix\\" + k.ToString() + ".txt");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    swMatrix.Write((j == 0 ? "" : " ") + mat[i][j]); ;
                }
                swMatrix.WriteLine();
            }
            swMatrix.Close();
            return mat;
        }

        private static double[] OutputBase(int n, string path, int k, double[][] matrix)
        {
            var res = new double[n];
            var swBase = new StreamWriter(path + "Base\\" + k.ToString() + ".txt");
            for (int i = 0; i < n; i++)
            {
                double cur = matrix[i][i];
                cur*= cur;
                cur = Math.Log(cur);
                swBase.WriteLine(cur);
                res[i] = cur;
            }
            swBase.Close();
            return res;
        }

        private static void OutputTriangle(int n, string path, int k, double[][] matrix)
        {
            var swTriangle = new StreamWriter(path + "Triangle\\" + k.ToString() + ".txt");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    swTriangle.Write((j == 0 ? "" : " ") + matrix[i][j]);
                }
                swTriangle.WriteLine();
            }
            swTriangle.Close();
        }

        private static double[][] MakeMatrix(int n, Random rand)
        {
            var matrix = new double[n][];
            for (int i = 0; i < n; i++)
            {
                matrix[i] = new double[n];
                for (int j = 0; j < i; j++)
                {
                    matrix[i][j] = rand.NextDouble();
                }
                matrix[i][i] = rand.Next(n) + 1;
            }
            return matrix;
        }


    }
}
#endif