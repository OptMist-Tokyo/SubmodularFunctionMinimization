using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class BinaryMatrixRank:SubmodularOracle
    {
        double[] modular;
        bool[][] columnVectors;
        bool[][] coefficientColumnVectors;  //for caluculation
        bool[] columnVector;    //or caluculation

        public BinaryMatrixRank(int n, int row, double[] modular, bool[][] columnVectors)
        {
            SetVariables(n, row, modular, columnVectors);
        }

        public BinaryMatrixRank(string path)
        {
            var streamReader = new StreamReader(path);
            int n = int.Parse(streamReader.ReadLine());
            var modular = new double[n];
            for (int i = 0; i < n; i++)
            {
                modular[i] = double.Parse(streamReader.ReadLine());
            }//for i
            var row = int.Parse(streamReader.ReadLine());
            var columnVectors = new bool[n][];
            for (int i = 0; i < n; i++)
            {
                columnVectors[i] = new bool[row];
            }//for i
            for (int i = 0; i < row; i++)
            {
                string rowVector = streamReader.ReadLine();
                for (int j = 0; j < n; j++)
                {
                    columnVectors[j][i] = (rowVector[j] == '1');
                }//for j
            }//for i
            streamReader.Close();
            SetVariables(n, row, modular, columnVectors);
        }

        private void SetVariables(int n, int row, double[] modular, bool[][] columnVectors)
        {
            SetVariable(n, 0);
            this.Row = row;
            this.modular = modular;
            this.columnVectors = columnVectors;

            columnVector = new bool[Row];
            coefficientColumnVectors = new bool[Row][];
            for (int i = 0; i < Row; i++)
            {
                coefficientColumnVectors [i] = new bool[Row];
            }//for i
        }

        public int Row
        {
            get;
            set;
        }

        internal override double Value(int[] order, int cardinality)
        {
            SetUnitMatrix();
            int rank = 0;
            for (int k = 0; k < cardinality; k++)
            {
                CheckAndDoRankArgmentation(order[k], coefficientColumnVectors, ref rank);
            }//for k
            double res = order.Take(cardinality).Sum(x => modular[x]);
            res += rank;
            return res;
        }

        internal override void Base(int[] order, double[] b)
        {
            SetUnitMatrix();
            int rank = 0;
            for (int i = 0; i < N; i++)
            {
                b[order[i]] = modular[order[i]];
                if (CheckAndDoRankArgmentation(order[i],coefficientColumnVectors,ref rank))
                {
                    b[order[i]]++;
                }//if
            }//for i
        }

        private bool CheckAndDoRankArgmentation(int pos , bool[][] coefficientColumnVectors, ref int rank)
        {
            SetMultipleyTranspose(pos, coefficientColumnVectors);
            int onePos = GetOnePosition(rank, columnVector);
            if (onePos == -1)
            {
                return false;
            }//if
            Swap(coefficientColumnVectors, rank, columnVector, onePos);
            Elimination(coefficientColumnVectors, rank, columnVector);
            rank++;
            return true;
        }
        
        private void Elimination(bool[][] coefficientColumnVectors, int rank, bool[] columnVector)
        {
            for (int i = rank + 1; i < Row; i++)
            {
                if (columnVector[i])
                {
                    for (int j = 0; j < Row; j++)
                    {
                        coefficientColumnVectors[i][j] ^= coefficientColumnVectors[rank][j];
                    }//for j
                }//if
            }//for i
        }

        private static void Swap(bool[][] coefficientColumnVectors, int rank, bool[] columnVector, int onePos)
        {
            var tmpVariable = columnVector[onePos];
            columnVector[onePos] = columnVector[rank];
            columnVector[rank] = tmpVariable;
            var tmpArray = coefficientColumnVectors[onePos];
            coefficientColumnVectors[onePos] = coefficientColumnVectors[rank];
            coefficientColumnVectors[rank] = tmpArray;
        }

        private int GetOnePosition(int rank, bool[] columnVector)
        {
            for (int i = rank; i < Row; i++)
            {
                if (columnVector[i])
                {
                    return i;
                }//if
            }//for i
            return -1;
        }

        private void SetMultipleyTranspose(int pos, bool[][] coefficientColumnVectors)
        {
            for (int i = 0; i < Row; i++)
            {
                columnVector[i] = false;
                for (int j = 0; j < Row; j++)
                {
                    columnVector[i] ^= (coefficientColumnVectors[i][j] & columnVectors[pos][j]);
                }//for j
            }//for i
        }

        private void SetUnitMatrix()
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    coefficientColumnVectors[i][j] = false;
                }//for j
                coefficientColumnVectors[i][i] = true;
            }//for i
        }
        

    }
}
