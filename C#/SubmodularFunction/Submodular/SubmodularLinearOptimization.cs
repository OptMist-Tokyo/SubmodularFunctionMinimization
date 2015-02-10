using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onigiri.Algorithm;

namespace Onigiri.Submodular
{
    public  class SubmodularLinearOptimization:LinearOptimization
    {
        SubmodularOracle oracle;
        int[] order;
        double[] copy;

        public SubmodularLinearOptimization(SubmodularOracle oracle):base(oracle.N)
        {
            this.oracle = oracle;
            order = new int[oracle.N];
            copy = new double[oracle.N];
        }

        public SubmodularOracle Oracle
        {
            get { return oracle; }
        }

       public override object CalcLinearMinimizer(double[] x, double[] extremeBase)
       {
           for (int i = 0; i < oracle.N; i++)
           {
               order[i] = i;
               copy[i] = x[i];
           }//for i
           Array.Sort(copy, order, 0, oracle.N);
           oracle.CalcBase(order, extremeBase);
           return order.ToArray();
       }

       public override object GetInitialBase(double[] b)
       {
           var path = @"E:\seed.txt";
           var sr = new System.IO.StreamReader(path);
           var tmp = int.Parse(sr.ReadLine());
           sr.Close();
           var sw = new System.IO.StreamWriter(path);
           sw.WriteLine((tmp + 1) % 25);
           sw.Close();
           var seq = RandomSequence(b.Length, tmp);
           for (int i = 0; i < seq.Length; i++)
           {
               b[i] = seq[i];
           }

           return CalcLinearMinimizer(copy, b);
       }

       /// <summary>
       /// 長さ n の乱数列の生成
       /// 配列の中身は 0 ～ n-1 までの自然数
       /// O( n )
       /// </summary>
       /// <param name="n">配列の長さ</param>
       /// <param name="seed">乱数のシード</param>
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

        public void Resize(int n)
        {
            this.N = n;
            Array.Resize(ref order,n);
            Array.Resize(ref copy,n);
        }



    }
}
