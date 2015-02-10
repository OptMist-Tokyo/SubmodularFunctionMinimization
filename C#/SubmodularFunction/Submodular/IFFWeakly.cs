using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class IFFWeakly:IFF
    {

        public override SFMResult Minimization(SubmodularOracle oracle, double absEps = DefaultAbsEps, double relativeEps = DefaultRelativeEps)
        {
            return MinimizationWeakly(oracle, absEps, relativeEps);
        }

    }
}
