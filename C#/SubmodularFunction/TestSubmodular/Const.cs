using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.TestSubmodular
{
    class Const
    {
        //public const string DataPrefix = @"E:\Submodular\DataInteger\";
        public const string DataPrefix = @"E:\Submodular\DataDouble\";
        public const string AnsPath = @"E:\Submodular\Answers";
#if DEBUG
        public const string ResultPrefix = @"E:\Submodular\Results\Debug_C#_";
#else
        public const string ResultPrefix = @"E:\Submodular\Results\Release_C#_";
#endif
    }
}
