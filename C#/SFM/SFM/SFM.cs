using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Onigiri.Submodular;

namespace SFM
{
    class SFM
    {
        public static void Main(string[]args)
        {
            new SFM().Solve(args);            
        }

        public void Solve(string[] args)
        {
            var sfm = GetAlgorithm(args);
            var oracle = GetOracle(args);
            if (sfm==null||oracle==null)
            {
                return;
            }
            double error = GetValue(args, "-e");
            var res = sfm.Minimization(oracle);
            var path = GetPath(args,"-r");
            res.Output(path, false);
        }

        private SubmodularOracle GetOracle(string[] args)
        {
            var index = GetIndex(args, "-o");
            var path = GetPath(args,"-f");
            if (path == null)
            {
                Console.WriteLine("Please set the file path correctly.");
                return null;
            }
            if (index==0)
            {
                return new UndirectedCut(path);
            }
            else if (index==1)
            {
                return new DirectedCut(path);
            }
            else if (index==2)
            {
                return new ConnectedDetachment(path);
            }
            else if (index==3)
            {
                return new FacilityLocation(path);
            }
            else if (index==4)
            {
                return new GraphicMatroid(path);
            }
            else if (index==5)
            {
                return new SetCover(path);
            }
            else if (index==6)
            {
                return new NonPositiveSymmetricMatrixSummation(path);
            }
            else if (index==7)
            {
                return new BinaryMatrixRank(path);
            }
            else
            {
                Console.WriteLine("Please set the index of oracle correctly.");
                return null;
            }

        }

        private static int GetIndex(string[] args,string key)
        {
            var index = -1;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == key && i + 1 < args.Length && int.TryParse(args[i + 1], out index))
                {
                    break;
                }
            }
            return index;
        }

        private static double GetValue(string[] args, string key)
        {
            var value = 1e-9;
            for (int i = 0; i < args.Length; i++)
            {
                double cur = 0;
                if (args[i] == key && i + 1 < args.Length && double.TryParse(args[i + 1], out cur))
                {
                    value = cur;
                    break;
                }
            }
            return value;
        }
        
        private static string GetPath(string[] args,string key)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == key && i + 1 < args.Length )
                {
                    return args[i + 1];
                }
            }
            return null;
        }


        private SubmodularFunctionMinimization GetAlgorithm(string[] args)
        {
            var index = GetIndex(args, "-a");
            if (index==0)
            {
                return new IFFWeakly();
            }
            else if (index==1)
            {
                return new IFFStrongly();
            }
            else if (index==2)
            {
                return new Schrijver();
            }
            else if (index==3)
            {
                return new HybridWeakly();
            }
            else if (index==4)
            {
                return new HybridStrongly();
            }
            else if (index==5)
            {
                return new Orlin();
            }
            else if (index==6)
            {
                return new IOWeakly();
            }
            else if (index==7)
            {
                return new IOStrongly();
            }
            else if (index==8)
            {
                return new FW();
            }
            else
            {
                Console.WriteLine("Please set the index of algorithm correctly.");
                return null;
            }

        }


    }
}
