using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.FW
{
    public abstract class FW
    {
        const int OutputLength = 20;
        string[] labels = new string[]{"N", 
            "reduced N", 
//            "Iteration", 
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


        protected Stopwatch sw;

        public long ExecutionTime
        {
            get { return sw.ElapsedMilliseconds; }
        }

        public string Minimizer
        {
            get;
            protected set;
        }

        public double MinimumValue
        {
            get;
            protected set;
        }




        public abstract void Minimization(SubmodularOracle oracle,double absEps = 1e-10,double relativeEps = 1e-10);

        public void OutputResult()
        {
        }


        public void Output(string path,SubmodularOracle oracle, bool withX = true)
        {
            StreamWriter streamWriter;
            WriteLable(path);
            while (true)
            {
                try { streamWriter = new StreamWriter(path, true); }
                catch
                {
                    continue;
                }
                break;
            }//
            OutputVariable("N", oracle.N, streamWriter);
            OutputVariable("reduced N", oracle.Count, streamWriter);
            //OutputVariable("Iteration", Iteration, streamWriter);
            OutputVariable("Execurtion Time", ExecutionTime, streamWriter);
            OutputVariable("Oracle Time",oracle.OracleTime, streamWriter);
            OutputVariable("Reduction Time", -1, streamWriter);
            OutputVariable("Oracle Call", 0, streamWriter);
            OutputVariable("Base Call", oracle.BaseCall, streamWriter);
            OutputVariable("Reduction Call", -1, streamWriter);
            OutputVariable("Minimum Value", MinimumValue, streamWriter);
            OutputVariable("Minimizer",Minimizer, streamWriter);
            OutputVariable("Dual Value", -1, streamWriter, false);
            //if (withX)
            //{
            //    OutputArray("x", X, streamWriter);
            //}//if
            streamWriter.Close();
        }


        private void OutputArray<T>(string name, T[] array, StreamWriter streamWriter)
        {
            streamWriter.WriteLine(name.PadRight(OutputLength) + " : ");
            if (array == null)
            {
                return;
            }//if
            foreach (var element in array)
            {
                streamWriter.WriteLine(element.ToString());
            }//foreach element
        }

        private void OutputVariable<T>(string name, T value, StreamWriter streamWriter, bool flg = true)
        {
            //streamWriter.Write(name.PadRight(OutputLength) + " : " + value.ToString());
            streamWriter.Write(value.ToString());
            if (flg)
            {
                streamWriter.Write('\t');
            }
            else
            {
                streamWriter.WriteLine();
            }
        }

        private void WriteLable(string path)
        {
            if (File.Exists(path))
            {
                return;
            }
            var sw = new StreamWriter(path, false);
            for (int i = 0; i < labels.Length; i++)
            {
                if (i != 0)
                {
                    sw.Write('\t');
                }
                sw.Write(labels[i]);
            }
            sw.WriteLine();
            sw.Close();
        }

    }


}
