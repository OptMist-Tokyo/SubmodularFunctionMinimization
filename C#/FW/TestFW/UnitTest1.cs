using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Onigiri.FW;
using System.Reflection;
using System.Text;

namespace Onigiri.TestFW
{



    [TestClass]
    public class UnitTest1
    {

        internal enum Oracles
        {
            //Modular,
            //UndirectedCut,
            //DirectedCut,
            //ConnectedDetachment,
            //FacilityLocation,
            //GraphicMatroid,
            //SetCover,
            SetCoverConcave,
            //NonPositiveSymmetricMatrixSummation,
            //BinaryMatrixRank,
        };

        internal enum Algorithms
        {
            FW,
            //FWContract,
            //FWOriginal,
        }

        //const int nMin = 1;
        //const int nMax = 20;
        const int nMin = 1;
        const int nMax =8192;
        const int kMin = 0;
        const int kMax = 25;
        bool pow = 1 == 1;

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
                    string o = oracleName.ToString();
                    var path = directly + @"\" + o + "_" + algoName.ToString() + ".txt";
                    StreamWriter sw = new StreamWriter(path);
                    sw.WriteLine(Content());
                    sw.Close();
                    var range = (pow ? Pow(nMin, nMax) : Range(nMin, nMax));
                    foreach (int n in range)
                    {
                        for (int k = kMin; k < kMax; k++)
                        {
                            //if (n < 4 || k != 15)
                            //{
                            //    continue;
                            //}//if

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            //var oracle = new ReducedOracle(GetOracle(oracleName.ToString(), n, k));
                            //var oracle = new ReducedOracle(GetOracle(oracleName.ToString(), n, k), false);
                            var oracle = GetOracle(oracleName.ToString(), n, k, 0.9, 300);

                            if (algoName.ToString()=="FW")
                            {
                                var algo = new Onigiri.FW.FWBinary();
                                algo.Minimization(oracle);
                                //CheckResult(algo.MinimumValue, n, k, o,algo.Minimizer);
                                OutputResult(path, oracle, algo);
                            }
                            else if (algoName.ToString() == "FWContract")
                            {
                                var algo = new Onigiri.FW.FWContract();
                                algo.Minimization(oracle);
//                                CheckResult(algo.MinimumValue, n, k, o, algo.Minimizer);
                                OutputResult(path, oracle, algo);
                            }
                            else if (algoName.ToString()=="FWOriginal")
                            {
                                var algo = new Onigiri.FW.FWOriginal();
                                algo.Minimization(oracle);
//                                CheckResult(algo.MinimumValue, n, k, o, algo.Minimizer);
                                OutputResult(path, oracle, algo);           
                            }
                            else
                            {
                                throw new Exception();
                            }

                        }//for k
                    }//for n
                }//foreach algoName
            }//foreach item
        }

        private string Content()
        {
            var content = new object[]{
                "N",
                "ReducedN",
                "Iteration",
                "ExecutionTime",
                "OracleTime",
                "BaseCall",
                "MinimumValue",
                "Minimizer",
            };
            return MakeString(content);
        }

        private void OutputResult(string path, SubmodularOracle oracle, FW.FWBinary algo)
        {
            StreamWriter streamWriter = new StreamWriter(path, true);
            var res = new object[]{
                oracle.N,
                oracle.Count,
                algo.Iteration,
                algo.ExecutionTime,
                oracle.OracleTime,
                oracle.BaseCall,
                algo.MinimumValue,
                algo.Minimizer
            };
            streamWriter.WriteLine(MakeString(res));
            streamWriter.Close();
        }

        private void OutputResult(string path, SubmodularOracle oracle, FWContract algo)
        {
            StreamWriter streamWriter = new StreamWriter(path, true);
            var res = new object[]{
                oracle.N,
                oracle.Count,
                algo.Iteration,
                algo.ExecutionTime,
                oracle.OracleTime,
                oracle.BaseCall,
                algo.MinimumValue,
                algo.Minimizer
            };
            streamWriter.WriteLine(MakeString(res));
            streamWriter.Close();
        }

        private void OutputResult(string path, SubmodularOracle oracle, FWOriginal algo)
        {
            StreamWriter streamWriter = new StreamWriter(path, true);
            var res = new object[]{
                oracle.N,
                oracle.Count,
                algo.Iteration,
                algo.ExecutionTime,
                oracle.OracleTime,
                oracle.BaseCall,
                algo.MinimumValue,
                algo.Minimizer
            };
            streamWriter.WriteLine(MakeString(res));
            streamWriter.Close();
        }

        private string MakeString(object[] res)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < res.Length; i++)
            {
                if (i>0)
                {
                    sb.Append("\t");
                }
                sb.Append(res[i].ToString());
            }
            return sb.ToString();
        }

        private void CheckResult(double val , int n, int k, string oracle,string min)
        {
            if (n>20)
            {
                return;
            }
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
            bool ok = (minimumValue == val
                || Math.Abs(minimumValue - val) / Math.Max(Math.Abs(minimumValue), Math.Abs(val)) <= 1e-9);
            Assert.AreEqual(true, ok);
            var tmp = minimizer == min;
            sr.Close();
        }

        private static string GetPath(string oracle, int n, int k)
        {
            var path = Const.AnsPath + oracle + @"\" + n.ToString() + "_" + k.ToString();
            return path;
        }


        internal static SubmodularOracle GetOracle(string oracleName, int n, int k, double pow , double coeff )
        {
            string o = oracleName.ToString();
            string path = Const.DataPrefix + o + "\\" + n.ToString() + "_" + k.ToString();
            var oracle = (SubmodularOracle)Activator.CreateInstance(
                Assembly.LoadFrom("Onigiri.FW.dll").GetType("Onigiri.FW." + oracleName),
                (oracleName == "SetCoverConcave" ? new object[] { path, pow, coeff } : new object[] { path })
                );
            return oracle;
        }


        private IEnumerable<int> Pow(int nMin, int nMax)
        {
            for (int i = nMin; i <= nMax; i *= 2)
            {
                yield return i;
            }
        }

        private IEnumerable<int> Range(int nMin, int nMax)
        {
            for (int i = nMin; i <= nMax; i++)
            {
                yield return i;
            }
        }


        [TestMethod]
        public void FindWorstParameter()
        {
            var oracleName = "SetCoverConcave";
            int kMin = 0;
            int kMax = 5;


            string path = @"C:\Users\onigiri\Desktop\parameterBad.txt";
            StreamWriter sw = new StreamWriter(path);
            sw.Close();
            for (double p = 0.5; p < 1; p+=0.05)
            {
                for (int coeff = 100; coeff < 1000; coeff += 50)
                {
                    long sum = 0;
                    foreach (var algoName in Enum.GetValues(typeof(Algorithms)))
                    {
                        string o = oracleName.ToString();
                        int n = 4096;
                        for (int k = kMin; k < kMax; k++)
                        {

                           //k = 1;

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            var oracle = GetOracle(oracleName.ToString(), n, k, p, coeff);
                            var algo = new Onigiri.FW.FWBinary();
                            algo.Minimization(oracle);
                            sum += algo.ExecutionTime;
                            //sw = new StreamWriter(path, true);
                            //sw.WriteLine(k.ToString()+" "+p + " " + coeff + " " + algo.ExecutionTime);
                            //sw.Close();
                        }//for k
                    }//foreach algoName
                    sw = new StreamWriter(path, true);
                    sw.WriteLine(p + " " + coeff + " " + sum);
                    sw.Close();
                }
            }
        }

    }
}
