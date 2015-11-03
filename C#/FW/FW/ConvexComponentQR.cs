using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.FW
{
    class ConvexComponentsQR : ConvexComponents
    {
        double[] calcX;

        public ConvexComponentsQR(int n, double[][] kernelMatrix, double absEps, double relativeEps)
            : base(n, kernelMatrix, absEps, relativeEps)
        {
            SetInitialQ(N + 1);
            calcX = new double[N];
        }

        protected override bool AddToMatrix(double[] extremeBase)
        {
            if (Count == 0)
            {
                for (int i = 0; i < N; i++)
                {
                    Q[0][i] = extremeBase[i];
                }
                Q[0][N] = 1;
            }
            else
            {
                MultiplyByQTransposeWithOne(extremeBase, matrix[Count]);
                MultiplyByQ(matrix[Count], Q[Count]);
                MinusByBWithOne(extremeBase, Q[Count]);
            }
            matrix[Count][Count] = Normalize(Q[Count]);
            //for (int i = 0; i < Count - 1; i++)
            //{
            //    if (matrix[i][i] < matrix[i + 1][i + 1])
            //    {
            //        Console.WriteLine();
            //    }
            //}
            //if (Count>=170)
            //{
            //    Console.WriteLine();
            //}

            return matrix[Count][Count] >= Math.Abs(matrix[0][0]) * RelativeEps;
        }

        private double Normalize(double[] vector)
        {
            double res = 0;
            for (int i = 0; i <= N; i++)
            {
                res += vector[i] * vector[i];
            }
            res = Math.Sqrt(res);
            for (int i = 0; i <= N; i++)
            {
                vector[i] /= res;
            }
            return res;
        }

        private void MinusByBWithOne(double[] extremeBase, double[] q)
        {
            for (int i = 0; i < N; i++)
            {
                q[i] = extremeBase[i] - q[i];
            }
            q[N] = 1 - q[N];
        }

        private void MultiplyByQ(double[] cur, double[] res)
        {
            for (int i = 0; i <= N; i++)
            {
                res[i] = 0;
                for (int j = 0; j < Count; j++)
                {
                    res[i] += Q[j][i] * cur[j];
                }
            }
        }

        private void MultiplyByQTransposeWithOne(double[] cur, double[] res)
        {
            for (int i = 0; i < Count; i++)
            {
                res[i] = Q[i][N];
                for (int j = 0; j < N; j++)
                {
                    res[i] += cur[j] * Q[i][j];
                }
            }
        }

        protected override void CalcBestLambdas()
        {
            for (int i = 0; i < Count; i++)
            {
                vector[i] = Q[i][N];
            }
            SolveLinearEquation(vector, r, Count);
            double sum = 0;
            for (int i = 0; i < Count; i++)
            {
                sum += r[i];
            }
            for (int i = 0; i < Count; i++)
            {
                bestLambdas[i] = r[i] / sum;
            }
        }

        protected override void Delete(int index)
        {
            for (int i = index; i < Count - 1; i++)
            {
                var tmpVariable = matrix[i];
                matrix[i] = matrix[i + 1];
                matrix[i + 1] = tmpVariable;
                var tmp = components[i];
                components[i] = components[i + 1];
                components[i + 1] = tmp;
            }
            for (int i = index; i < Count - 1; i++)
            {
                Elimination(i, i + 1, Count - 1);
            }
            Count--;
        }

        protected override void Elimination(int pivotRow, int eraseRow, int endCol)
        {
            double a = matrix[pivotRow][pivotRow];
            double b = matrix[pivotRow][eraseRow];
            double sqrt = Math.Sqrt((a * a + b * b));
            if (sqrt == 0)
            {
                return;
            }
            double cos = a / sqrt;
            double sin = b / sqrt;
            for (int j = pivotRow; j < endCol; j++)
            {
                double pivot = matrix[j][pivotRow];
                double erase = matrix[j][eraseRow];
                //TODO: set tolerance
                matrix[j][pivotRow] = pivot * cos + erase * sin;
                matrix[j][eraseRow] = -pivot * sin + erase * cos;
            }//for j
            matrix[pivotRow][eraseRow] = 0;

            for (int j = 0; j <= N; j++)
            {
                double pivot = Q[pivotRow][j];
                double erase = Q[eraseRow][j];
                Q[pivotRow][j] = pivot * cos + erase * sin;
                Q[eraseRow][j] = -pivot * sin + erase * cos;
            }//for j
        }

        internal bool CalcRemoveX(List<int> sortedList, HashSet<int> hash, Func<double, bool> func)
        {
            double lambda = 0;
            foreach (var item in sortedList)
            {
                lambda -= components[item].Lambda;
            }

            var array = hash.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                calcX[i] = 0;
            }

            int sortedListIndex = 0;
            for (int i = 0; i < Count; i++)
            {
                if (sortedListIndex<sortedList.Count&&sortedList[sortedListIndex]==i)
                {
                    sortedListIndex++;
                    continue;
                }
                double coeff = 1.0 / lambda * components[i].Lambda;
                double[] b = components[i].B;
                for (int k = 0; k < array.Length; k++)
                {
                    calcX[k] += b[array[k]];
                }
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (!func.Invoke(array[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
