using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onigiri.Submodular;
using System.Reflection;

namespace Onigiri.TestSubmodular
{
    [TestClass]
    public class Execution
    {
        internal enum Oracles
        {
//            Modular,
            UndirectedCut,
            //DirectedCut,
            //ConnectedDetachment,
            //FacilityLocation,
            //GraphicMatroid,
            //SetCover,
            //SetCoverConcave,
            //NonPositiveSymmetricMatrixSummation,
            BinaryMatrixRank,
        };

        internal enum Algorithms
        {
            //BruteForce,


            //IFFWeakly,
            //IFFStrongly,
            FW,
            //IOWeakly,
            //IOStrongly,
            //Orlin,
            //HybridWeakly,
            //HybridStrongly,
            //Schrijver,


        };

        string[] labels = new string[]{"N", 
            "reduced N", 
            "Iteration", 
            "Execurtion Time", 
            "Oracle Time",
            "Reduction Time", 
            "Oracle Call", 
            "Base Call", 
            "Reduction Call", 
            "Minimum Value", 
            "Minimizer", 
            "Dual Value", 
            };

        //const int nMin =128;
        //const int nMax = 200;

        const int nMin = 16;
        const int nMax = 32;
        const int kMin = 0;
        const int kMax = 25;
        bool pow = 1==1;

        [TestMethod]
        public void ExecutionAll()
        {
            var date = DateTime.Now;
            var time = date.ToString().Replace(' ', '_').Replace(':', '_').Replace('/', '_');
            var directly = @"" + Const.ResultPrefix + time;
            Directory.CreateDirectory(directly);
            foreach (var oracleName in Enum.GetValues(typeof(Oracles)))
            {
                foreach (var algoName in Enum.GetValues(typeof(Algorithms)))
                {
                    string o =  oracleName.ToString();
                    var path = directly + @"\" + o + "_" + algoName.ToString() + ".txt";
                    var sw = new StreamWriter(path);
                    for (int i = 0; i < labels.Length; i++)
                    {
                        if (i!=0)
                        {
                            sw.Write('\t');
                        }
                        sw.Write(labels[i]);
                    }
                    sw.WriteLine();
                    sw.Close();
                    var range = (pow ? Pow(nMin, nMax) : Range(nMin, nMax));
                    foreach (int n in range)
                    {
                        for (int k = kMin; k < kMax; k++)
                        {
                            //if (n < 64 || k < 13)
                            //{
                            //    continue;
                            //}//if

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            //var oracle = new ReducedOracle(GetOracle(oracleName.ToString(), n, k));
                            //var oracle = new ReducedOracle(GetOracle(oracleName.ToString(), n, k),false);
                            var oracle = new ReducedOracle(GetOracle(oracleName.ToString(), n, 0), false);
                            //var oracle = GetOracle(oracleName.ToString(), n, k, 0.75, 10);
                            var algo = GetAlgo(algoName.ToString());
                            var result = algo.Minimization(oracle);
                            result.Output(path,false);
                            //CheckResult(result, n, k, oracleName.ToString());
                        }//for k
                    }//for n
                }//foreach algoName
            }//foreach item
        }

        private IEnumerable<int> Pow(int nMin, int nMax)
        {
            for (int i = nMin; i <= nMax; i*=2)
            {
                yield return i;
            }
        }

        private IEnumerable<int> Range(int nMin,int nMax)
        {
            for (int i = nMin; i <= nMax; i++)
            {
                yield return i;
            }
        }

        private void CheckResult(SFMResult result, int n, int k, string oracle)
        {
            string path = GetPath(oracle, n, k);
            StreamReader sr;
            try
            {
                sr = new StreamReader(path);
            }
            catch
            {
                return;
            }
            string minimizer = sr.ReadLine();
            var minimumValue = double.Parse(sr.ReadLine());
            bool ok = (minimumValue==result.MinimumValue
                ||Math.Abs(minimumValue - result.MinimumValue) / Math.Max(Math.Abs(minimumValue), Math.Abs(result.MinimumValue)) <= 1e-9 
                || (minimizer == result.Minimizer));
            Assert.AreEqual(true, ok);
            sr.Close();
        }

        [TestMethod]
        public void MakeTest()
        {
            const int nMin = 1;
            const int nMax = 20;
            foreach (var oracleName in Enum.GetValues(typeof(Oracles)))
            {
                string directly = Const.AnsPath + oracleName.ToString();
                Directory.CreateDirectory(directly);
                for (int n = nMin; n <= nMax; n++)
                {
                    for (int k = kMin; k < kMax; k++)
                    {
                        var path = GetPath(oracleName.ToString(), n, k);
                        var oracle = GetOracle(oracleName.ToString(), n, k);
                        var algo = new BruteForce();
                        var result = algo.Minimization(oracle);
                        //result.Output(path);
                        var sw = new StreamWriter(path);
                        sw.WriteLine(result.Minimizer);
                        sw.WriteLine(result.MinimumValue);
                        sw.Close();
                    }//for k
                }//for n
            }//foreach item
        }

        private static string GetPath(string oracle, int n, int k)
        {
            var path =Const.AnsPath+ oracle + @"\" + n.ToString() + "_" + k.ToString();
            return path;
        }
        


            //int n = 3;
            //string path0; string path1;
            //double[] modular;
            //double[][] matrix;

            //var oracle0 = new UndirectedCut(path0);
            //var oracle1 = new SetCover(path1);
            //var oracle2 = new FacilityLocation(n, modular, matrix);

            //var oracle0 = new SetCover("");
            //var algo0 = new BruteForce();
            //var sfmResult0 = algo0.Minimization(oracle0);



        //private void OutputResult(string path, Action<StreamWriter> action)
        //{
        //    StreamWriter sw = new StreamWriter(path,true);
        //    action.Invoke(sw);
        //    sw.Close();
        //}

        internal static SubmodularFunctionMinimization GetAlgo(string algoName)
        {
            return (SubmodularFunctionMinimization)Activator.CreateInstance(
                Assembly.LoadFrom("Onigiri.Submodular.dll").GetType("Onigiri.Submodular." + algoName));
        }

        internal static SubmodularOracle GetOracle(string oracleName, int n, int k,double pow = 0.5,double coeff = 100)
        {
            string o = oracleName.ToString();
            string path = Const.DataPrefix + o+ "\\" + n.ToString() + "_" + k.ToString();
            var oracle = (SubmodularOracle)Activator.CreateInstance(
                Assembly.LoadFrom("Onigiri.Submodular.dll").GetType("Onigiri.Submodular." + oracleName),
                (oracleName == "SetCoverConcave" ? new object[] { path,pow,coeff } : new object[] { path })
                );
            return oracle;
        }


        [TestMethod]
        public void FindWorstParameter()
        {
            const int n = 512;
            var oracleName = "SetCoverConcave";
            var stoiwatch = new System.Diagnostics.Stopwatch();
            var sw = new StreamWriter(@"C:\Users\onigiri\Desktop\res.txt", false);
            sw.Close();
            for (double pow = 0.2; pow <= 0.5; pow += 0.05)
            {
                for (double coeff = 100; coeff < 1000; coeff += 50)
                {
                    sw = new StreamWriter(@"C:\Users\onigiri\Desktop\res.txt", true);
                    stoiwatch.Restart();
                    for (int k = kMin; k < kMax; k++)
                    {
                        var oracle = GetOracle(oracleName, n, k, pow, coeff);
                        var algo = new Onigiri.Submodular.FW();
                        var result = algo.Minimization(oracle);
                        //sw.WriteLine(result.Minimizer);
                        //result.Output(i2ath);
                        //Console.WriteLine(result.ToString());
                       // sw.WriteLine(result.ExecutionTime);
                    }//for k
                    stoiwatch.Stop();
                    sw.WriteLine(pow.ToString() + " " +coeff+" "+ stoiwatch.ElapsedMilliseconds);
                    sw.Close();
                }
            }
        }


    }
}