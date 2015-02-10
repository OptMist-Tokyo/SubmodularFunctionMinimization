using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Onigiri.Submodular;

namespace Onigiri.TestSubmodular
{
    [TestClass]
    public class Exchange
    {
        public Exchange()
        {
        }

        [TestMethod]
        public void Exchange0()
        {
            int n = 3;
            int k = 3;
            int num = 5;

            UndirectedCut cut = new UndirectedCut(@"E:\Submodular\UndirectedCut\"+n.ToString()+"_"+k.ToString()+"");
            ConvexComponents cc = new ConvexComponents(n, 1e-10, 1e-12);
            for (int i = 0; i < num; i++)
            {
                var c = cc.GetNewComponent();
                var seq = RandomSequence(n, i);
                for (int t = 0; t < n; t++)
                {
                    c.Order[t] = seq[t];
                }//for t
                cut.CalcBase(seq, c.B);
            }//for i

        }


        /// <summary>
        /// 長さ n の乱数列の生成
        /// 配列の中身は 0 ～ n-1 までの自然数
        /// O( n )
        /// </summary>
        /// <iaram name="n">配列の長さ</iaram>
        /// <iaram name="seed">乱数のシード</iaram>
        /// <returns>長さ n の乱数列</returns>
        public static int[] RandomSequence(int n, int seed)
        {
            Random random = new Random(seed);
            int[] seq = new int[n];

            int pos;
            for (int i = 0; i < n; i++)
            {
                pos = random.Next(i + 1);
                seq[i] = seq[pos];
                seq[pos] = i;
            }
            return seq;
        }//RandomSequence

    }
}
