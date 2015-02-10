using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class BruteForce:SubmodularFunctionMinimization
    {
        public BruteForce()
        {
        }

        int minimizerMask;
        public override SFMResult Minimization(SubmodularOracle oracle,double absEps = DefaultAbsEps,double relativeEps=DefaultRelativeEps)
        {
            Initialize(oracle,absEps,relativeEps);

            double minValue = double.MaxValue;
            minimizerMask = -1;
            for (int mask = 0; mask < (1 << N); mask++)
            {
                Iteration++;
                int usedBit ;
                var order = SubmodularOracle.GetOrder(N, mask, out usedBit);
                double cur = oracle.CalcValue(order, usedBit);
                if (cur<minValue)
                {
                    minValue = cur;
                    minimizerMask = mask;
                }//if
            }//for mask
            SetMinimizer();
            x = null;
            return SetResults(oracle);
        }

        private void SetMinimizer()
        {
            this.minimizer = new bool[N];
            for (int i = 0; i < N; i++)
            {
                this.minimizer[i] = (minimizerMask >> i & 1) == 1;
            }//for i
        }

        private string ConvertToString(int minimizer,int n)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                sb.Append(((minimizer & 1) == 1) ? "1" : "0");
                minimizer >>= 1;
            }//for i
            return sb.ToString();
        }
        
    }
}
