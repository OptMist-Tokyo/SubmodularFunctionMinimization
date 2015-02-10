using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class HybridStrongly:Hybrid
    {
        public override SFMResult Minimization(SubmodularOracle oracle, double absEps = DefaultAbsEps, double relativeEps = DefaultRelativeEps)
        {
            return MinimizationStrongly(oracle, absEps, relativeEps);
        }

    }
}
