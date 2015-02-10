using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Onigiri.FW
{
    public class SetCoverConcave : SetCover
    {
        double pow;
        double coeff;

        public SetCoverConcave(int N, int[] remainder, int countReduce, int countDelete, int count, double[] modular, double[] weight, int[][] edges, int[] emptyUsed, int[] fullUsed, int M, double SumOfEmptyModular, double SumOfEmptyWeight, double SumOfFullModular, double SumOfFullWeight, Stopwatch sw,double pow,double coeff)
            : base(N, remainder, countReduce, countDelete, count, modular, weight, edges, emptyUsed, fullUsed, M, SumOfEmptyModular, SumOfEmptyWeight, SumOfFullModular, SumOfFullWeight, sw)
        {
            SetVariable(pow,coeff);
        }
        
        public SetCoverConcave(int n, int m, double[] modular, double[] weight, int[][] edges,double pow,double coeff)
            : base(n, m, modular, weight, edges)
        {
            SetVariable(pow,coeff);
        }


        public SetCoverConcave(string path,double pow,double coeff)
            : base(path)
        {
            SetVariable(pow,coeff);
        }

        private void SetVariable(double pow, double coeff)
        {
            this.pow = pow;
            this.coeff = coeff;
        }

        protected override double CalcValue(double modular, double weight)
        {
            return modular + coeff * Math.Pow(weight, pow);
        }

        public override SubmodularOracle Copy()
        {
            return new SetCoverConcave(N, remainder, CountContract, CountDelete, Count, modular, weight, edges, emptyUsed, fullUsed, M, SumOfEmptyModular, SumOfEmptyWeight, SumOfFullModular, SumOfFullWeight, sw,pow,coeff);
        }

    }
}
