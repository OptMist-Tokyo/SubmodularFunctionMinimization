using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
   public  class IOWeakly:IO
    {

       private void InitializeIOWeakly(SubmodularOracle oracle, double absEps ,double relativeEps)
       {
           InitializeIO(oracle, absEps, relativeEps); 
       }

       public override SFMResult Minimization(SubmodularOracle oracle, double absEps = DefaultAbsEps, double relativeEps = DefaultRelativeEps)
       {
           InitializeIOWeakly(oracle, absEps, relativeEps);
           while (N>0)
           {
               double eta = GetMaxValue();
               if (eta<DefaultPrecision/N-absEps||eta<=0)
               {
                   break;
               }//if
               Wave(eta);
           }//while
           SetMinimizerForWeakly(dataForContract.Remainder);
           return SetResults(oracle);
       }

    }
}
