using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class SFMResult
    {
        const int OutputLength = 20;
        SubmodularOracle oracle;
        //Stopwatch timer;
        //long memoOracleCallTime; //for calculation
        //long memoBaseCallTime;  //for calculation
        //long memoReductionTime; //for calculation
        double[] x;

        internal SFMResult(SubmodularOracle oracle)
        {
            this.oracle = oracle;
            //this.N = n;
            //timer = new Stopwatch();
            //timer.Start();
        }

        public void Output(string path,bool withX = true)
        {
            StreamWriter streamWriter;
            while (true)
            {
                try { streamWriter = new StreamWriter(path,true); }
                catch
                {
                    continue;
                }
                break;
            }//
            OutputVariable("N", BaseN, streamWriter);
            OutputVariable("reduced N", N, streamWriter);
            OutputVariable("Iteration", Iteration, streamWriter);
            OutputVariable("Execurtion Time", ExecutionTime, streamWriter);
            OutputVariable("Oracle Time", OracleTime, streamWriter);
            OutputVariable("Reduction Time", ReductionTime, streamWriter);
            OutputVariable("Oracle Call", OracleCall, streamWriter);
            OutputVariable("Base Call", BaseCall, streamWriter);
            OutputVariable("Reduction Call", ReductionTime, streamWriter);
            OutputVariable("Minimum Value", MinimumValue, streamWriter);
            OutputVariable("Minimizer", Minimizer, streamWriter);
            OutputVariable("Dual Value", DualValue, streamWriter,false);
            if (withX)
            {
                OutputArray("x", X, streamWriter);                
            }//if
            streamWriter.Close();
        }

        private void OutputArray<T>(string name, T[] array, StreamWriter streamWriter)
        {
            streamWriter.WriteLine(name.PadRight(OutputLength) + " : " );
            if (array==null)
            {
                return;
            }//if
            foreach (var element in array)
            {
                streamWriter.WriteLine(element.ToString());
            }//foreach element
        }

        private void OutputVariable<T>(string name, T value,StreamWriter streamWriter,bool flg = true)
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

        internal void SetResult(double[]x,string minimizer,double minimumValue,long iteration,long executionTime,
            ConvexComponents component,SubmodularOracle oracle)
        {
            //timer.Stop();
            //ExecutionTime = timer.ElapsedMilliseconds;
            this.x = (x == null ? null: (double[])x.Clone());
            this.Minimizer = minimizer;
            this.MinimumValue = minimumValue;
            this.DualValue = (x == null ? 0 : x.Where(val => val <= 0).Sum());
            this.Iteration = iteration;
            this.ExecutionTime = executionTime;

            this.OracleCall = oracle.OracleCall;
            this.BaseCall = oracle.BaseCall;
            this.OracleTime = oracle.OracleTime;
        }
        
        //internal void StartOracle()
        //{
        //    memoOracleCallTime = timer.ElapsedMilliseconds;
        //    OracleCall++;
        //}

        //internal void StopOracle()
        //{
        //    OracleTime += timer.ElapsedMilliseconds - memoOracleCallTime;
        //}

        //internal void StartBase()
        //{
        //    memoBaseCallTime = timer.ElapsedMilliseconds;
        //    BaseCall++;
        //}

        //internal void StopBase()
        //{
        //    OracleTime += timer.ElapsedMilliseconds - memoBaseCallTime;
        //}

        //internal void StartReduction()
        //{
        //    memoReductionTime = timer.ElapsedMilliseconds;
        //    ReductionCall++;
        //}

        //internal void StopReduction()
        //{
        //    ReductionTime += timer.ElapsedMilliseconds - memoReductionTime;
        //}

        public string Minimizer
        {
            get;
            private set;
        }

        public double MinimumValue
        {
            get;
            private set;
        }

        public double[] X
        {
            get { return (x == null ? null : (double[])x.Clone()); }
        }

        public long Iteration
        {
            get;
            private set;
        }

        public long OracleCall
        {
            get;
            private set;
        }

        public long BaseCall
        {
            get;
            private set;
        }

        public long ReductionCall
        {
            get;
            private set;
        }

        public long ExecutionTime
        {
            get;
            set;
        }

        public long ReductionTime
        {
            get;
            set;
        }

        public long OracleTime
        {
            get;
            set;
        }

        public double DualValue
        {
            get;
            set;
        }

        public int BaseN
        {
            get { return oracle.BaseN; }
        }

        public int N
        {
            get { return oracle.N; }
        }


    }
}
