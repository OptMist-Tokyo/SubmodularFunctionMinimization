using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    /// <summary>
    /// This class maintains a convex combination of bases.
    /// In order to add new convex combination,
    /// ilease use GetNewComponent() and directly modify its iroierty.
    /// (It is unsafe but we can save memory.)
    /// </summary>
    public class ConvexComponents : IEnumerable
    {
        int n;  //the cardinality for the ground set
        List<ConvexComponent> components;   //current components for convex combination
        List<double[]> r;   //for qr decomposition
        List<ConvexComponent> deletedList;  //for reduce
        List<double> nullVector;    //for reduce
        Stopwatch timer;


        public ConvexComponents(int n,double absEps,double relativeEps)
        {
            Count = 0;
            this.n = n;
            this.AbsEps = absEps;
            this.RelativeEps = relativeEps;
            components = new List<ConvexComponent>();
            r = new List<double[]>();
            deletedList = new List<ConvexComponent>();
            nullVector = new List<double>();
            timer = new Stopwatch();
        }

        public void Clear()
        {
            Count = 0;
        }

        public int Count
        {
            get;
            private set;
        }

        public long ReductionCall
        {
            get;
            private set;
        }

        public long ReductionTime
        {
            get {return timer.ElapsedMilliseconds; }
        }

        private double AbsEps
        {
            get;
            set;
        }

        private double RelativeEps
        {
            get;
            set;
        }

        internal ConvexComponent this[int index]
        {
            get { return components[index]; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <iaram name="currentN">Current cardinality of the ground set</iaram>
        /// <returns></returns>
        public ConvexComponent GetNewComponent()
        {
            if (components.Count == Count)
            {
                components.Add(new ConvexComponent(n));
                r.Add(new double[n+1]);
            }//if
            components[Count].Lambda = 0;
            return components[Count++];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <iaram name="currentN">Current cardinality of the ground set</iaram>
        /// <returns></returns>
        internal ConvexComponentWithInverseOrder GetNewComponentWithInverseOrder()
        {
            if (components.Count == Count)
            {
                components.Add(new ConvexComponentWithInverseOrder(n));
                r.Add(new double[n+1]);
            }//if
            components[Count].Lambda = 0;
            return (ConvexComponentWithInverseOrder)components[Count++];
        }

        internal ConvexComponentWithLevel GetNewComponentWithLevel()
        {
            if (components.Count == Count)
            {
                components.Add(new ConvexComponentWithLevel(n));
                r.Add(new double[n + 1]);
            }//if
            components[Count].Lambda = 0;
            return (ConvexComponentWithLevel)components[Count++];
        }

        internal ConvexComponentWithID GetNewComponentWithID()
        {
            if (components.Count == Count)
            {
                components.Add(new ConvexComponentWithID(n));
                r.Add(new double[n + 1]);
            }//if
            components[Count].Lambda = 0;
            return (ConvexComponentWithID)components[Count++];
        }

        //internal ConvexComponent GetNewComponentOrlin()
        //{
        //    if (components.Count == Count)
        //    {
        //        components.Add(new ConvexComponentOrlin(n));
        //        r.Add(new double[n + 1]);
        //    }//if
        //    components[Count].Lambda = 0;
        //    return (ConvexComponentOrlin)components[Count++];
        //}

        internal void Delete(int index)
        {
            Count--;
            Swap(components,index);
        }

        private void Swap<T>(List<T> list, int index)
        {
            Swap(list, index, Count);
        }

        private void Swap<T>(List<T> list,int index0,int index1)
        {
            var tmpVariable = list[index0];
            list[index0] = list[index1];
            list[index1] = tmpVariable;
        }

        public List<ConvexComponent> Reduce(int usedLength)
        {
            timer.Start();
            if (usedLength==-1)
            {
                usedLength = n;
            }//if
            ReductionCall++;
            InitializeQR(usedLength);
            int rank = CalcQRDecomposition(0,usedLength+1,false);
            var deletedList =  Reduce(rank, usedLength + 1);
            timer.Stop();
            return deletedList;
        }

        int cnt = 0;
        private List<ConvexComponent> Reduce(int rank,int length)
        {
            while (nullVector.Count<rank+1)
            {
                nullVector.Add(0);
            }//while
            deletedList.Clear();
            while (rank!=Count)
            {
                cnt++;
                //CheckSum();
                CalcNullVector(rank);
                //CheckSum();
                //TestReduce(rank);
                ExecuteReduce(ref rank, length, deletedList);
                //CheckSum();
            }//while
            return deletedList;
        }

        //[System.Diagnostics.Conditional("DEBUG")]
        //private void TestReduce(int rank)
        //{
        //    for (int i = 0; i < rank; i++)
        //    {
        //        double absMaxVal = 0;
        //        double sum = 0;
        //        for (int j = 0; j < rank+1; j++)
        //        {
        //            sum += r[j][i] * nullVector[j];
        //            absMaxVal = Math.Max(absMaxVal, Math.Abs(r[j][i] * nullVector[j]));
        //        }//for j
        //        if (Math.Abs(sum)>absMaxVal*RelativeEps&&Math.Abs(sum)>AbsEps&&Math.Abs(sum)>1e-3)
        //        {
        //            System.Diagnostics.Debugger.Break();
        //            throw new Exception();
        //        }//if
        //    }//for i
        //    for (int i = 0; i < rank; i++)
        //    {
        //        double absMaxVal = 0;
        //        double sum = 0;
        //        for (int j = 0; j < rank+1; j++)
        //        {
        //            if (i==components[j].B.Length)
        //            {
        //                sum += nullVector[j];
        //                absMaxVal = Math.Max(absMaxVal, nullVector[j]);
        //            }//if
        //            else
        //            {
        //                sum += components[j].B[i] * nullVector[j];
        //                absMaxVal = Math.Max(absMaxVal, Math.Abs(components[j].B[i] * nullVector[j]));
        //            }//else
        //        }//for j
        //        if (Math.Abs(sum)>absMaxVal*RelativeEps)
        //        {
        //            System.Diagnostics.Debugger.Break();
        //            throw new Exception();                    
        //        }//if
        //    }//for i
        //}

        private void ExecuteReduce(ref int rank,int length,List<ConvexComponent> deletedList)
        {
            int minPos = -1;
            double coefficient = double.MaxValue;
            for (int i = 0; i <= rank; i++)
            {
                double cur = -components[i].Lambda / nullVector[i];
                if (nullVector[i] < 0&coefficient>cur)
                {
                    minPos = i;
                    coefficient = cur;
                }//if
            }//for i
            for (int i = 0; i <= rank; i++)
            {
                components[i].Lambda += coefficient * nullVector[i];
            }//for i

            components[minPos].Lambda = 0;
            //CheckSum();
            int rei = rank;
            int nextRank = rank;
            for (int i = 0; i <= rei; i++)
            {
                while (i<=rei&&components[i].Lambda < AbsEps)        //TODO : tolarence
                {
                    rei--;
                    deletedList.Add(components[i]);
                    Count--;
                    for (int j = i; j < Count; j++)
                    {
                        SwapColumn(j, j + 1);
                    }//for j
                    nextRank = CalcQRDecomposition(i, length,true);
                }//if
            }//for i
            rank = nextRank;
        }

        //[System.Diagnostics.Conditional("DEBUG")]
        //private void CheckSum()
        //{
        //    double sum = 0;
        //    for (int i = 0; i < Count   ; i++)
        //    {
        //        sum += components[i].Lambda;
        //        if (components[i].Lambda<-0.0001)
        //        {
        //            Debugger.Break();
        //        }//if
        //    }//for i
        //    if (sum>1.01)
        //    {
        //        Debugger.Break();
        //    }//if
        //}

        private void CalcNullVector(int rank)
        {
            nullVector[rank] = 1;
            double maxAbsValue = 1;
            for (int j = rank - 1; j >= 0; j--)
            {
                double sum = 0;
                for (int i = j + 1; i <= rank; i++)
                {
                    sum += r[i][j] * nullVector[i];
                }//for i
                nullVector[j] = -sum / r[j][j];
                maxAbsValue = Math.Max(maxAbsValue, Math.Abs(nullVector[j]));
            }//forrev j

            for (int i = 0; i <= rank; i++)
            {
                if (Math.Abs(nullVector[i]) < maxAbsValue*RelativeEps)    //TODO : tolarence for nullVector
                {
                    nullVector[i] = 0;
                }//if
            }//for i
        }

        private int CalcQRDecomposition(int rank,int length,bool onlyOneRowElimination)
        {
            int endCol = Count;
            for (; rank < Math.Min(length, endCol); rank++)
            {
                int endRow = (onlyOneRowElimination ? Math.Min(rank + 2, length) : length);
                while (rank < endCol && !CalcEliminationOfColumn(rank,endRow,endCol))
                {
                    endCol--;
                    SwapColumn(rank, endCol);
                }//while
            }//for i
            return Math.Min(rank, endCol);
        }

        private bool CalcEliminationOfColumn(int rank,int endRow,int endCol)
        {
            //int absMaxPos = rank;
            //for (int i = rank; i < endRow; i++)
            //{
            //    if (Math.Abs(curR[absMaxPos])<Math.Abs(curR[i]))
            //    {
            //        absMaxPos = i;
            //    }//if
            //}//for i
            //SwapRow(rank, absMaxPos,rank,endCol);
            double[] curR = r[rank];
            for (int i = rank+1; i < endRow; i++)
            {
                if (curR[i]!=0)
                {
                    CalcEliminationOfRow(rank, i, rank,endCol);
                }//if
            }//for i
            if (Math.Abs(curR[rank]) < Math.Abs(r[0][0]) * RelativeEps)    //TODO: tolerance for eigenvalue
            {
                curR[rank] = 0;
                return false;
            }//if
            return true;
        }

        //private void SwapRow(int index0, int index1, int startCol, int endCol)
        //{
        //    if (index0 != index1)
        //    {
        //        for (int i = startCol; i < endCol; i++)
        //        {
        //            var tmpVariable = r[i][index0];
        //            r[i][index0] = r[i][index1];
        //            r[i][index1] = tmpVariable;
        //        }//for i                
        //    }//if
        //}

        private void CalcEliminationOfRow(int pivotRow, int erasedRow, int startCol,int endCol)
        {
            double a = r[startCol][pivotRow];
            double b = r[startCol][erasedRow];
            double sqrt = Math.Sqrt((a * a + b * b));
            double cos = a / sqrt;
            double sin = b / sqrt;
            for (int j = startCol; j < endCol; j++)
            {
                double pivot = r[j][pivotRow];
                double erase = r[j][erasedRow];
                //TODO: set tolerance
                r[j][pivotRow] = pivot * cos + erase * sin;
                r[j][erasedRow] = -pivot * sin + erase * cos;
            }//for j
            r[startCol][erasedRow] = 0;
        }

        private void SwapColumn(int index0,int index1)
        {
            Swap(components, index0, index1);
            Swap(r, index0, index1);
        }
        
        private void InitializeQR(int usedLength)
        {
            if (Count==0)
            {
                return;
            }//if
            var order = components[0].Order;
            for (int i = 0; i < Count; i++)
            {
                var curB = components[i].B;
                var curR = r[i];
                for (int j = 0; j < usedLength; j++)
                {
                    curR[j] = curB[order[j]];
                }//for j
                curR[usedLength] = 1;
            }//for i
        }

        internal void Swap(int index0, int index1)
        {
            var tmpVariable = components[index0];
            components[index0] = components[index1];
            components[index1] = tmpVariable;
        }

        public IEnumerator<ConvexComponent> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return components[i];
            }//for i
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }        

    }




}
