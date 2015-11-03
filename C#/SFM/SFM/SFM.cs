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
            var index = GetIndex(args, "-a");
            double error = GetValue(args, "-e",1e-10);
            var path = GetPath(args, "-r");
            if (index < AlgorithmSeparator)
            {
                ExecuteCombinatorialAlgorithm(args, path,error);
            }
            else if (index >= AlgorithmSeparator)
            {
                ExecuteFWAlgorithm(args, path);
            }
            else
            {
                Console.WriteLine("Please set the index of algorithm correctly.");
                return;
            }
        }

        private void ExecuteFWAlgorithm(string[] args, string path)
        {
            var fw = GetFWAlgorithm(args);
            var oracle = GetFWOracle(args);
            if (fw == null || oracle == null)
            {
                return;
            }
             fw.Minimization(oracle);
             fw.Output(path,oracle, false);
        }

        private void ExecuteCombinatorialAlgorithm(string[] args, string path,double error)
        {
            var sfm = GetAlgorithm(args);
            var oracle = GetOracle(args);
            if (sfm == null || oracle == null)
            {
                return;
            }
            var res = sfm.Minimization(oracle,error,error);
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
            if (index==(int)Oracles.UndirectedCut)
            {
                return new UndirectedCut(path);
            }
            else if (index==(int)Oracles.DirectedCut)
            {
                return new DirectedCut(path);
            }
            else if (index==(int)Oracles.ConnectedDetachment)
            {
                return new ConnectedDetachment(path);
            }
            else if (index==(int)Oracles.FacilityLocation)
            {
                return new FacilityLocation(path);
            }
            else if (index==(int)Oracles.GraphicMatroid)
            {
                return new GraphicMatroid(path);
            }
            else if (index==(int)Oracles.SetCover)
            {
                return new SetCover(path);
            }
            else if (index==(int)Oracles.NonPositiveSymmetricMatrixSummation)
            {
                return new NonPositiveSymmetricMatrixSummation(path);
            }
            else if (index==(int)Oracles.BinaryMatroid)
            {
                return new BinaryMatroid(path);
            }
            else if (index==(int)Oracles.SetCoverConcave)
            {
                double alpha = GetValue( args,  "-p", 0.75);
                double c = GetValue(args, "-c", 10.0);
                return new SetCoverConcave(path,alpha,c);
            }
            else
            {
                Console.WriteLine("Please set the index of oracle correctly.");
                return null;
            }

        }


        private Onigiri.FW.SubmodularOracle GetFWOracle(string[] args)
        {
            var index = GetIndex(args, "-o");
            var path = GetPath(args, "-f");
            if (path == null)
            {
                Console.WriteLine("Please set the file path correctly.");
                return null;
            }
            if (index == (int)Oracles.UndirectedCut)
            {
                return new Onigiri.FW.UndirectedCut(path);
            }
            else if (index == (int)Oracles.DirectedCut)
            {
                return new Onigiri.FW.DirectedCut(path);
            }
            else if (index == (int)Oracles.SetCover)
            {
                return new Onigiri.FW.SetCover(path);
            }
            else if (index==(int)Oracles.SetCoverConcave)
            {
                double alpha = GetValue(args, "-p", 0.75);
                double coeff = GetValue(args, "-c", 10.0);
                return new Onigiri.FW.SetCoverConcave(path, alpha, coeff);
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

        private static double GetValue(string[] args, string key,double defalutValue)
        {
            var value = defalutValue;
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
            if (index==(int)Algorithms.IFFWeakly)
            {
                return new IFFWeakly();
            }
            else if (index==(int)Algorithms.IFFStrongly)
            {
                return new IFFStrongly();
            }
            else if (index==(int)Algorithms.Schrijver)
            {
                return new Schrijver();
            }
            else if (index==(int)Algorithms.HybridWeakly)
            {
                return new HybridWeakly();
            }
            else if (index==(int)Algorithms.HybridStrongly)
            {
                return new HybridStrongly();
            }
            else if (index==(int)Algorithms.Orlin)
            {
                return new Orlin();
            }
            else if (index==(int)Algorithms.IOWeakly)
            {
                return new IOWeakly();
            }
            else if (index==(int)Algorithms.IOStrongly)
            {
                return new IOStrongly();
            }
            else if (index == (int)Algorithms.FW)
            {
                return new FW();
            }
            else
            {
                Console.WriteLine("Please set the index of algorithm correctly.");
                return null;
            }

        }


        private Onigiri.FW.FW GetFWAlgorithm(string[] args)
        {
            var index = GetIndex(args, "-a");
            if (index == (int)FWAlgorithms.FWOriginal)
            {
                return new Onigiri.FW.FWOriginal();
            }
            else if (index == (int)FWAlgorithms.FWContract)
            {
                return new Onigiri.FW.FWContract();
            }
            else if (index == (int)FWAlgorithms.FWBinary)
            {
                return new Onigiri.FW.FWBinary();
            }
            else
            {
                Console.WriteLine("Please set the index of algorithm correctly.");
                return null;
            }

        }


        public static int AlgorithmSeparator = 100;
        enum Algorithms { IFFWeakly = 0, IFFStrongly, Schrijver, HybridWeakly, HybridStrongly, Orlin, IOWeakly, IOStrongly, FW };
        enum FWAlgorithms { FWOriginal = AlgorithmSeparator, FWContract, FWBinary };
        enum Oracles { UndirectedCut = 0, DirectedCut,ConnectedDetachment, FacilityLocation, GraphicMatroid,SetCover,NonPositiveSymmetricMatrixSummation,BinaryMatroid, SetCoverConcave};

    }
}
