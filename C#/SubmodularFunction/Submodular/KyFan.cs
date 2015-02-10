using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class KyFan:SubmodularOracle
    {
        double[] modular;
        double[][] matrix;
        double[][] sub;
        double[][] q;

        public KyFan(int n, double[] modular, double[][] matrix)
        {
            SetVariables(n, modular, matrix);
        }

        public KyFan(string path)
        {
            var streamReader = new StreamReader(path);
             int n = int.Parse(streamReader.ReadLine());
            var modular = new double[n];
            for (int i = 0; i < n; i++)
            {
                modular[i] = double.Parse(streamReader.ReadLine());
            }//for i
           var matrix = new double[n][];
            for (int i = 0; i < n; i++)
            {
                matrix[i] = Array.ConvertAll<string, double>(streamReader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), double.Parse);
            }//for i
            streamReader.Close();
            SetVariables(n, modular, matrix);
        }

        private void SetVariables(int n,double []modular,double[][]matrix)
        {
            SetVariable(n, 0);
            this.modular = modular;
            this.matrix = matrix;
            sub = new double[n][];
            q = new double[n][];
            for (int i = 0; i < n; i++)
            {
                sub[i] = new double[n];
                q[i] = new double[n];
            }
        }

        internal override double Value(int[] order, int cardinality)
        {
            double res = 0;
            for (int i = 0; i < cardinality; i++)
            {
                res += modular[order[i]];
            }//for i
            SetSubMatrix(order, cardinality);
            double determinant = CalcLogDeterminant(cardinality);
            res += determinant;
            return res;
        }

        private void SetSubMatrix(int[] order, int cardinality)
        {
            for (int i = 0; i < cardinality; i++)
            {
                for (int j = 0; j < cardinality; j++)
                {
                    sub[i][j] = matrix[order[i]][order[j]];
                }
            }
        }

        private double CalcLogDeterminant(int cardinality)
        {
            for (int col = 0; col < cardinality; col++)
            {
                for (int row = col+1; row < cardinality; row++)
                {
                    double a = sub[col][col];
                    double b = sub[row][col];
                    double sqrt = Math.Sqrt(a * a + b * b);
                    double cos = a / sqrt;
                    double sin = b / sqrt;
                    for (int i = col; i < cardinality; i++)
                    {
                        double pivot = sub[col][i];
                        double erase = sub[row][i];
                        sub[col][i] = pivot * cos + erase * sin;
                        sub[row][i] = -pivot * sin + erase * cos;
                    }
                }
            }
            double res = 1;
            for (int i = 0; i < cardinality; i++)
            {
                res += Math.Log(sub[i][i]);
            }
            return res;
        }


        internal override void Base(int[] order, double[] b)
        {
            double prev = 0;
            double cur = 0;
            for (int i = 0; i < N; i++)
            {
                int pos = order[i];
                for (int j = 0; j < i; j++)
                {
                    q[j][i] = q[i][j] = 0;
                    sub[i][j] = matrix[pos][order[j]];
                    double val = 0;
                    for (int k = 0; k <= i; k++)
                    {
                        val += q[k][j] * matrix[order[k]][pos];
                    }
                    sub[j][i] = val;
                }
                q[i][i] = 1;
                sub[i][i] = matrix[pos][pos];
                //Check(i,matrix, q, sub);
                for (int j = 0; j < i; j++)
                {
                    double s = sub[j][j];
                    double t = sub[i][j];
                    double sqrt = Math.Sqrt(s * s + t * t);
                    double cos = s / sqrt;
                    double sin = t / sqrt;
                    for (int k = j; k <= i; k++)
                    {                        
                        double pivot = sub[j][k];
                        double erase = sub[i][k];
                        sub[j][k] = pivot * cos + erase * sin;
                        sub[i][k] = -pivot * sin + erase * cos;
                    }
                    for (int k = 0; k <= i; k++)
                    {
                        double pivot = q[k][j];
                        double erase = q[k][i];
                        q[k][j] = pivot * cos + erase * sin;
                        q[k][i] = -pivot * sin + erase * cos;                        
                    }
                    //Check(i, matrix, q, sub);
                }
                //Check(i,matrix, q, sub);
                cur = 1;
                for (int k = 0; k <= i; k++)
                {
                    cur += Math.Log(sub[k][k]);
                }
                b[pos] = cur - prev + modular[pos];
                prev = cur;
            }
            //Check(N-1, matrix, q, sub);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void Check(int cur,double[][] matrix, double[][] q, double[][] sub)
        {
            cur++;
            double[][] a = new double[cur][];
            for (int i = 0; i < cur; i++)
            {
                a[i] = new double[cur];
            }
            for (int i = 0; i < cur; i++)
            {
                for (int j = 0; j < cur; j++)
                {
                    for (int k = 0; k < cur; k++)
                    {
                        a[i][j] += q[i][k] * sub[k][j];
                    }
                }
            }
        }


    }
}
