using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class QRDecomposition
    {
        List<int> rowIndices;
        List<Pair<int,double[]>> R;
        List<double[]> Q;
        double[] baseForCalc; //for calc
        double[] vector;
        double[] transBase;

        private int N
        {
            get;
            set;
        }

        private int Count
        {
            get;
            set;
        }

        private int Row
        {
            get;
            set;
        }

        private double RelativeEps
        {
            get;
            set;
        }

        private double AbsEps
        {
            get;
            set;
        }

        public QRDecomposition(int n,double absEps,double relativeEps)
        {
            this.N = n;
            this.RelativeEps = relativeEps;
            this.AbsEps = absEps;
            rowIndices = new List<int>();
            baseForCalc = new double[N];
            transBase = new double[N];
            vector = new double[N];
            R = new List<Pair<int,double[]>>();
            Q = new List<double[]>();
        }


        internal void Update(int eraseElement, int addElement, double[] b)
        {
            int indexErase = -1;
            for (int i = 0; i < Count; i++)
            {
                if (R[i].first == eraseElement)
                {
                    indexErase = i;
                    R[i].first = addElement;
                    break;
                }//if
            }//for i

            Remove(indexErase);
            Eliminate(indexErase);
            MakeBase(b, baseForCalc);
            AddNewBase(addElement, baseForCalc);
        }

        private void AddNewBase(int addElement, double[] b)
        {
            int rank = GetRank();
            TransProd(b, transBase);
            if (R.Count == Count)
            {
                R.Add(new Pair<int, double[]>(-1, new double[N]));
            }//if
            R[Count].first = addElement;
            for (int i = 0; i < Row; i++)
            {
                R[Count].second[i] = transBase[i];
            }//for i
            Count++;

            double max = 0;
            for (int i = 0; i < Row; i++)
            {
                max = Math.Max(max, Math.Abs(R[Count - 1].second[i]));
            }
            for (int i = 0; i < Row; i++)
            {
                if (Math.Abs(R[Count - 1].second[i]) < AbsEps * max)
                {
                    R[Count - 1].second[i] = 0;
                }
            }

            for (int i = rank + 1; i < Row; i++)
            {
                if (R[Count - 1].second[i] != 0)
                {
                    CalcEliminationOfRow(rank, i, Count - 1, Count);
                }//if:
            }//for i
            SwapCol(rank, Count - 1);
        }

        public void TransProd(double[] b,double[]res)
        {
            double max = 1;
            for (int i = 0; i < Row; i++)
            {
                double positive = 0;
                double negative = 0;
                for (int j = 0; j < Row; j++)
                {
                    double current = Q[i][j] * b[j];
                    max = Math.Max(max, Math.Abs(b[j]));
                    if (current >= 0)
                    {
                        positive += current;
                    }//if
                    else
                    {
                        negative += current;
                    }//else
                }//for j
                if (positive<max*AbsEps)
                {
                    positive = 0;
                }
                if (-negative<max*AbsEps)
                {
                    negative = 0;
                }
                res[i] = RoundingValue(positive, negative);
            }//for i
        }

        private void Eliminate(int indexErase)
        {
            CalcQRDecomposition(indexErase, true);
        }

        private bool CalcEliminationOfColumn(int rank, int endRow, int endCol)
        {
            double[] curR = R[rank].second;
            for (int i = rank + 1; i < endRow; i++)
            {
                if (curR[i] != 0)
                {
                    CalcEliminationOfRow(rank, i, rank, endCol);
                }//if
            }//for i
            if (Math.Abs(curR[rank]) < Math.Abs(R[0].second[0]) * RelativeEps)    //TODO: tolerance for eigenvalue
            {
                curR[rank] = 0;
                return false;
            }//if
            return true;
        }

        private void CalcEliminationOfRow(int pivotRow, int erasedRow, int startCol, int endCol)
        {
            double a = R[startCol].second[pivotRow];
            double b = R[startCol].second[erasedRow];
            double sqrt = Math.Sqrt((a * a + b * b));
            double cos = a / sqrt;
            double sin = b / sqrt;
            for (int j = startCol; j < endCol; j++)
            {
                double pivot = R[j].second[pivotRow];
                double erase = R[j].second[erasedRow];
                //TODO: set tolerance
                R[j].second[pivotRow] = pivot * cos + erase * sin;
                R[j].second[erasedRow] = -pivot * sin + erase * cos;
            }//for j
            R[startCol].second[erasedRow] = 0;
            for (int j = 0; j < Row; j++)
            {
                double pivot = Q[pivotRow][j];
                double erase = Q[erasedRow][j];
                Q[pivotRow][j] = pivot * cos + erase * sin;
                Q[erasedRow][j] = -pivot * sin + erase * cos;
                //Q[pivotRow][j] = RoundingValue(pivot * cos, erase * sin);
                //Q[erasedRow][j] = RoundingValue(-pivot * sin, erase * cos);
            }//for j
        }


        private int CalcQRDecomposition(int rank,  bool onlyOneRowElimination)
        {
            int endCol = Count;
            for (; rank < Math.Min(Row, endCol); rank++)
            {
                int endRow = (onlyOneRowElimination ? Math.Min(rank + 2, Row) : Row);
                while (rank < endCol && !CalcEliminationOfColumn(rank, endRow, endCol))
                {
                    endCol--;
                    SwapCol(rank, endCol);
                }//while
            }//for i
            return Math.Min(rank, endCol);
        }
        
        private void Remove(int indexErase)
        {
            for (int i = indexErase; i < Count-1; i++)
            {
                SwapCol(i, i + 1);
            }//for i
            Count--;
        }

        private void SwapRow(int index0, int index1)
        {
            var tmpVariable = R[index0];
            R[index0] = R[index1];
            R[index1] = tmpVariable;
            var tmpArray = Q[index0];
            Q[index0] = Q[index1];
            Q[index1] = tmpArray;
        }

        private void SwapCol(int index0, int index1)
        {
            var tmpVariable = R[index0];
            R[index0] = R[index1];
            R[index1] = tmpVariable;
            //var tmpArray = Q[index0];
            //Q[index0] = Q[index1];
            //Q[index1] = tmpArray;
        }

        private void MakeBase(double[] b, double[] baseForCalc)
        {
            int cnt = 0;
            foreach (var row in rowIndices)
            {
                baseForCalc[cnt++] = b[row];
            }//foreach row
        }

        internal void Update(int x, double[] b)
        {
            Update(x, x, b);
        }
        
        internal void CalcNullVector(double[] nullVector)
        {
            int rank = GetRank();
            if (!ExistZeroVector(rank))
            {
                SolveEquation(rank);
            }//if

            for (int i = 0; i < N; i++)
            {
                nullVector[i] = 0;
            }//for i
            for (int i = 0; i <= rank; i++)
            {
                nullVector[R[i].first] = vector[i];
            }//for i
        }

        private bool ExistZeroVector(int rank)
        {
            for (int i = 0; i <= rank; i++)
            {
                bool existNonZero = false;
                for (int j = 0; j <= i; j++)
                {
                    if (R[i].second[j] != 0)
                    {
                        existNonZero = true;
                    }//if
                }//for j
                if (!existNonZero)
                {
                    for (int j = 0; j <= rank; j++)
                    {
                        vector[j] = 0;
                    }//for j
                    vector[i] = 1;
                    return true; ;
                }//if                
            }//for i
            return false;
        }

        private void SolveEquation(int rank)
        {
            vector[rank] = 1;
            for (int i = rank - 1; i >= 0; i--)
            {
                double positive = 0;
                double negative = 0;
                for (int j = i + 1; j <= rank; j++)
                {
                    double current = R[j].second[i] * vector[j];
                    positive += Math.Max(0, current);
                    negative += Math.Min(0, current);
                }//for j
                double val = R[i].second[i];
                vector[i] = -RoundingValue(positive / val, negative / val);
            }//forrev i
        }

        private int GetRank()
        {
            double max = 1;
            for (int i = 0; i < Math.Min(Count, Row); i++)
            {
                max = Math.Max(max, Math.Abs(R[i].second[i]));
                if (Math.Abs(R[i].second[i])<= RelativeEps*max)
                {
                    return i;
                }//if
            }//for i
            return Math.Min(Count, Row);
        }
        
        internal void FromScrach(HashSet<int> zero, int prevPositiveIndex, PriorityQueueMin[] que, double[][] bases)
        {
            Row = zero.Count - 1;
            rowIndices.Clear();
            foreach (var v in zero)
            {
                if (v==prevPositiveIndex)
                {
                    continue;
                }//if
                rowIndices.Add(v);
            }//foreach v

            SetQ();
            SetR(zero, que, bases);
        }

        private void SetR(HashSet<int> zero, PriorityQueueMin[] que, double[][] bases)
        {
            Count = 0;
            foreach (var v in zero)
            {
                int cnt = 0;
                var prev = que[v].Peek.B;
                var next = bases[v];
                foreach (var row in rowIndices)
                {
                    baseForCalc[cnt++] = RoundingValue(next[row], -prev[row]);
                }//foreach row
                AddNewBase(v, baseForCalc);
            }//foreach v
        }

        private void SetQ()
        {
            for (int i = Q.Count; i < Row; i++)
            {
                Q.Add(new double[N]);
            }//for i
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    Q[i][j] = 0;
                }//for j
                Q[i][i] = 1;
            }//for i
        }


        private double RoundingValue(double val0, double val1)
        {
            double sum = val0 + val1;
            double absSum = Math.Abs(sum);
            double maxAbsValue = Math.Max(Math.Abs(val0), Math.Abs(val1));
            if (absSum < AbsEps || absSum < maxAbsValue * RelativeEps)
            {
                return 0;
            }//if
            return sum;
        }


        [System.Diagnostics.Conditional("DEBUG")]
        public void CheckContain(int index,bool isFromScrach)
        {
            bool found = isFromScrach;
            for (int i = 0; i < Count; i++)
            {
                if (R[i].first==index)
                {
                    found = true;
                    break;
                }//if
            }//for i
            if (!found)
            {
                System.Diagnostics.Debugger.Break();
            }//if
        }

    }


    /// <summary>
    /// varified by SRM 337 DIV1 Medium BuildingAdvertise
    /// varified by SRM 529 DIV1 easy KingSort
    /// varified by SRM404 DIV1 Medium KSubstring
    /// Pair クラス
    /// </summary>
    /// <typeiaram name="typeFirst">０番目の要素の型</typeiaram>
    /// <typeiaram name="typeSecond">１番目の要素の型</typeiaram>
    public class Pair<typeFirst,typeSecond>
    {
        public typeFirst first;  //最初に比較される要素
        public typeSecond second;   //次に比較される要素

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <iaram name="typeFirst"></iaram>
        /// <iaram name="typeSecond"></iaram>
        public Pair(typeFirst first, typeSecond second)
        {
            this.first = first;
            this.second = second;
        }//Constractor

    }//Pair


}
