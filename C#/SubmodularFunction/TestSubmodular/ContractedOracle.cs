//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Onigiri.Submodular;

//namespace Onigiri.TestSubmodular
//{
//    [TestClass]
//    public class ContractedOracle
//    {

//        const int nMin = 1;
//        const int nMax = 10;
//        const int kMin = 0;
//        const int kMax = 25;
//        const int rei = 100;

//        [TestMethod]
//        public void ContractedOracle0()
//        {
//            foreach (var oracleName in Enum.GetValues(typeof(Execution.Oracles)))
//            {
//                for (int n = nMin; n <= nMax; n++)
//                {
//                    for (int k = kMin; k < kMax; k++)
//                    {
//                        for (int t = 0; t < rei; t++)
//                        {
//                            var oracle = Execution.GetOracle(oracleName.ToString(), n, k);
//                            var seq = RandomSequence(n, t);
//                            double[]b = new double[n];
//                            oracle.CalcBase(seq,b);
//                            TestReduce(oracle,seq,b);
//                            TestDelete(oracle, seq, b);
//                            TestContract(oracle, seq, b);
//                        }//for t
//                    }//for k
//                }//for n
//            }//foreach item
//        }

//        /// <summary>
//        /// 長さ n の乱数列の生成
//        /// 配列の中身は 0 ～ n-1 までの自然数
//        /// O( n )
//        /// </summary>
//        /// <iaram name="n">配列の長さ</iaram>
//        /// <iaram name="seed">乱数のシード</iaram>
//        /// <returns>長さ n の乱数列</returns>
//        public static int[] RandomSequence(int n, int seed)
//        {
//            Random random = new Random(seed);
//            int[] seq = new int[n];

//            int pos;
//            for (int i = 0; i < n; i++)
//            {
//                pos = random.Next(i + 1);
//                seq[i] = seq[pos];
//                seq[pos] = i;
//            }
//            return seq;
//        }//RandomSequence


//    }
//}
