using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class IOStrongly : IO
    {

        private void InitializeIOStrongly(SubmodularOracle oracle,double absEps,double relativeEps)
        {
            InitializeIO(oracle, absEps, relativeEps);
        }

        public override SFMResult Minimization(SubmodularOracle oracle, double absEps = DefaultAbsEps, double relativeEps = DefaultRelativeEps)
        {
            InitializeIOStrongly(oracle, absEps, relativeEps);
            int baseN = N;
            while (N > 0)
            {
                double eta = GetMaxValue();
                if (eta <= 0)
                {
                    break;
                }//if
                if (Reduce(eta)||Contract(eta))
                {
                    RemoveGap(NextOrder);
                    continue;
                }//if
                Wave(eta);
            }//while
            SetMinimizerForStrongly(dataForContract.Remainder, contractOracle);
            return SetResults(oracle);
        }

        private bool Contract(double eta)
        {
            double threshold = eta * N * N;
            foreach (var v in dataForContract.Remainder)
            {
                FillOrderOfReachable(v);
                double val = oracle.CalcValue(NextOrder, N);
                if (RoundingValue(val,-threshold)>0 )
                {                    
                   Contract(v, eta, SetContractVertices);
                   foreach (var component in components)
                   {
                       var levels = ((ConvexComponentWithLevel)component).Levels;
                       foreach (var cur in dataForContract.Remainder)
                       {
                           levels[cur] = 0;  
                       }//foreach cur
                   }//foreach component
                   ResetLevels();
                   N = oracle.N;
                   return true;
                }//if
            }//foreach v
            return false;
        }

        private void SetContractVertices(HashSet<int> hash,double eta,bool dummy)
        {
            hash.Clear();
            double threshold = -eta * N;
            foreach (var v in dataForContract.Remainder)
            {
                if (x[v]<threshold)
                {
                    hash.Add(v);
                }//if
            }//foreach v
        }

        private void FillOrderOfReachable(int v)
        {
            int posStart = 0;
            int posEnd = N - 1;
            foreach (var cur in dataForContract.Remainder)
            {
                if (dataForContract.Reachable[v].Contains(cur))
                {
                    NextOrder[posStart++] = cur;
                }//if
                else
                {
                    NextOrder[posEnd--] = cur;
                }//else
            }//foreach cur
        }

        private bool Reduce(double eta)
        {
            dataForContract.ReducedVertices.Clear();
            double threshold = -N * eta;
            foreach (var v in dataForContract.Remainder)
            {
                if (x[v]<threshold)
                {
                    dataForContract.ReducedVertices.Add(v);
                }//if
            }//foreach v
            bool res = dataForContract.ReducedVertices.Count > 0;
            ReduceForStrongly();
            N = oracle.N;
            if (res&&N>0)
            {
                AdjustComponents();
            }//if
            return res;
        }

        private void AdjustComponents()
        {
            for (int i = components.Count - 1; i > 0; i--)  //be careful for the range
            {
                components.Delete(i);
            }//forrev i
            var component = (ConvexComponentWithLevel)components[0];
            component.Lambda = 1;
            foreach (var v in dataForContract.Remainder)
            {
                x[v] = component.B[v];
                component.Levels[v] = 0;
            }//foreach v
            ResetLevels();
        }


    }
}
