using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmodularDataSet
{
    class Parameters
    {
        public const int minIntValue = (int)-1e6;
        public const int maxIntValue = (int)1e6;
        public const int Offset = (int)100000;   //the seed of random = n*Offset+k.
        public const int minPlus = 100;
        public const int minMinus = -100;
        public const int power = 2;
        public const int removedLen = 8;
        public const double Eps = 1e-12;


        public enum FunctionType
        {
            All, 
            Modular,
            UndirectedCut,
            DirectedCut,
            ConnectedDetachment,
            FacilityLocation,
            GraphicMatroid,
            BinaryMatrixRank,
            NonPositiveSymmetricMatrixSummation,
            SetCover,
            SetCoverConcave,
            KyFan
        }

    }
}
