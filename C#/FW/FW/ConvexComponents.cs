using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.FW
{
    public abstract class ConvexComponents
    {
        double[] x;
        //protected ConvexComponent[] components;
        //protected double[][] Q;
        //protected double[][] matrix;  //R
        protected List<ConvexComponent> components;
        protected List<double[]> Q;
        protected List<double[]> matrix;  //R
        protected double[] bestLambdas;   //the coefficients of the minimum norm ioint in th affine hull
        protected double[] r; //for calculation
        protected double[] vector;    //for calculation
        double[] vals;
        double[] kernelVector;
        double[][] kernelMatrix;
        double[] twistedVector;

        public int[][] GetData()
        {
            var res = new int[Count][];
            for (int i = 0; i < Count; i++)
            {
                res[i] = components[i].Data;
            }//for i
            return res;
        }

        public ConvexComponents(int n, double[][] kernelMatrix, double absEps, double relativeEps)
        {
            this.BaseN  = this.N = n;
            this.AbsEps = absEps;
            this.RelativeEps = relativeEps;
            this.kernelMatrix = kernelMatrix;
            x = new double[n];
            //components = new ConvexComponent[N+1];
            //matrix =new double[N+1][];
            components = new List<ConvexComponent>();
            matrix = new List<double[]>();
            bestLambdas = new double[n + 1];
            vals = new double[n + 1];
            kernelVector = new double[n];
            twistedVector = new double[n];
            //for (int i = 0; i < n + 1; i++)
            //{
            //    matrix[i] = new double[n + 1];
            //    components[i] = new ConvexComponent(n);
            //}
            r = new double[n + 1];
            vector = new double[n + 1];
            Count = 0;
            MinorCycle = 0;
        }

        public void Clear()
        {
            Count = 0;
            MinorCycle = 0;
        }

        private int BaseN
        { get; set; }

        protected int N
        {
            get;
            set;
        }

        bool IsKernel
        {
            get { return kernelMatrix != null; }
        }

        internal long MinorCycle
        { get; set; }

        double AbsEps
        {
            get;
            set;
        }

        protected double RelativeEps
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public double[] X
        {
            get { return x; }
        }

        public double[] Lambdas
        {
            get
            {
                var lambdas = new double[Count];
                for (int i = 0; i < Count; i++)
                {
                    lambdas[i] = components[i].Lambda;
                }
                return lambdas;
            }
        }

        protected abstract bool AddToMatrix(double[] extremeBase);
        protected abstract void Elimination(int pivotRow, int eraseRow, int endCol);
        protected abstract void CalcBestLambdas();
        protected abstract void Delete(int index);

        protected void SetInitialQ(int len)
        {
            Q = new List<double[]>();
            //Q = new double[len][];
            //for (int i = 0; i < len; i++)
            //{
            //    Q[i] = new double[len];
            //    Q[i][i] = 1;
            //}
        }


        public bool Add(double[] extremeBase, int[] data)
        {
            if (matrix.Count == Count)
            {
                matrix.Add(new double[BaseN + 1]);
                components.Add(new ConvexComponent(BaseN));
                Q.Add(new double[BaseN + 1]);
                Q[Count][Count] = 1;
            }
            if (!AddComponent(extremeBase, data))
            {
                return false;
            }
            ExecuteAlgorithm();
            return true;
        }

        private void ExecuteAlgorithm()
        {
            while (Count > 1)
            {
                MinorCycle++;
                CalcBestLambdas();
                if (IsInRelativeInterior())
                {
                    for (int i = 0; i < Count; i++)
                    {
                        components[i].Lambda = bestLambdas[i];
                    }//for i
                    break;
                }//if
                FindIntersection();
            }//while
            if (Count == 1)
            {
                components[0].Lambda = 1.0;
                //for (int i = 1; i < components.Length; i++)
                for (int i = 1; i < components.Count; i++)
                {
                    components[i].Lambda = 0;
                }
            }//if
        }

        private void FindIntersection()
        {
            double portion = 0;
            for (int i = 0; i < Count; i++)
            {
                if (components[i].Lambda - bestLambdas[i] > AbsEps)
                {
                    portion = Math.Max(portion, -bestLambdas[i] / (components[i].Lambda - bestLambdas[i]));
                }//if
            }//for i
            for (int i = Count - 1; i >= 0; i--)
            {
                components[i].Lambda = portion * components[i].Lambda + (1 - portion) * bestLambdas[i];
                if (components[i].Lambda < AbsEps)
                {
                    components[i].Lambda = 0;
                    Delete(i);
                }//if
            }//forrev i
            if (portion >= 1 - AbsEps)
            {
                int minIndex = 0;
                for (int i = 0; i < Count; i++)
                {
                    if (components[i].Lambda < components[minIndex].Lambda)
                    {
                        minIndex = i;
                    }
                }
                components[minIndex].Lambda = 0;
                Delete(minIndex);
            }
        }


        protected void Swap(int index0, int index1)
        {
            var tmpVariable = components[index0];
            components[index0] = components[index1];
            components[index1] = tmpVariable;
            var tmp = matrix[index0];
            matrix[index0] = matrix[index1];
            matrix[index1] = tmp;
        }

        protected void Elimination(int index, int endCol)
        {
            for (int i = index; i < endCol; i++)
            {
                Elimination(i, i + 1, endCol);
            }//for i
        }


        protected void SolveLinearEquation(double[] c, double[] r, int len)
        {
            for (int i = len - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < len; j++)
                {
                    sum += matrix[j][i] * r[j];
                }//for j
                double current = c[i] - sum;
                r[i] = current / matrix[i][i];
                //r[i] = Math.Max(r[i], 0);
            }//forrev i
        }

        public void SetX()
        {
            for (int i = 0; i < N; i++)
            {
                x[i] = 0;
            }//for iz
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    x[j] += components[i].Lambda * components[i].B[j];
                }//for j
            }//for i
        }

        internal void SetXWithoutError()
        {
            for (int i = 0; i < N; i++)
            {
                double sumPlus = 0;
                double sumMinus = 0;
                for (int j = 0; j < Count; j++)
                {
                    vals[j] = components[j].Lambda * components[j].B[i];
                }
                GetSum(vals, out sumPlus, out sumMinus);
                double sum = sumPlus + sumMinus;
                double maxAbsVal = Math.Max(sumPlus, sumMinus);
                if (Math.Abs(sum) <= RelativeEps * maxAbsVal)
                {
                    x[i] = 0;
                }//if
                else
                {
                    x[i] = sum;
                }//else
            }//for i
        }

        private void GetSum(double[] vals, out double sumPlus, out double sumMinus)
        {
            Array.Sort(vals, 0, Count);
            int seiarateIndex = Count;
            for (int i = 0; i < Count; i++)
            {
                if (vals[i] >= 0)
                {
                    seiarateIndex = i;
                    break;
                }
            }
            sumPlus = sumMinus = 0;
            for (int i = seiarateIndex - 1; i >= 0; i--)
            {
                sumMinus += vals[i];
            }
            for (int i = seiarateIndex; i < Count; i++)
            {
                sumPlus += vals[i];
            }
        }

        private bool AddComponent(double[] extremeBase, int[] data)
        {
            AddToComponents(extremeBase, data);
            bool res = AddToMatrix(extremeBase);
            Count++;
            return res;
        }


        protected void MultiplyByTransposeAndAddOne(double[] extremeBase, double[] vector)
        {
            if (kernelMatrix == null)
            {
                twistedVector = extremeBase;
            }
            else
            {
                for (int i = 0; i < N; i++)
                {
                    twistedVector[i] = 0;
                    for (int j = 0; j < N; j++)
                    {
                        twistedVector[i] += kernelMatrix[i][j] * extremeBase[j];
                    }
                }
            }
            for (int i = 0; i < Count; i++)
            {
                vector[i] = 1;
                for (int j = 0; j < N; j++)
                {
                    vector[i] += twistedVector[j] * components[i].B[j];
                }//for j
            }//for i
        }

        internal double CalcInnerProductKernel(double[] a, double[] b)
        {
            if (!IsKernel)
            {
                return CalcInnerProduct(a, b);
            }
            for (int i = 0; i < a.Length; i++)
            {
                double val = 0;
                for (int j = 0; j < N; j++)
                {
                    val += a[j] * kernelMatrix[i][j];
                }
                kernelVector[i] = val;
            }
            double res = 0;
            for (int i = 0; i < b.Length; i++)
            {
                res += kernelVector[i] * b[i];
            }
            return res;
        }

        internal double CalcSquareKernel(double[] x)
        {
            return CalcInnerProductKernel(x, x);
        }

        protected double CalcInnerProduct(double[] a, double[] b, int length = -1)
        {
            if (length == -1)
            {
                length = a.Length;
            }
            double res = 0;
            for (int i = 0; i < length; i++)
            {
                res += a[i] * b[i];
            }
            return res;
        }

        private void AddToComponents(double[] extremeBase, int[] data)
        {
            //if (components.Count==Count)
            //{
            //    components.Add(new ConvexComponent(N));
            //}
            components[Count].Set(0, extremeBase, data);
        }

        public bool IsInRelativeInterior()
        {
            bool res = true;
            for (int i = 0; i < Count; i++)
            {
                if (bestLambdas[i] < AbsEps)
                {
                    res = false;
                    if (bestLambdas[i] > -AbsEps)
                    {
                        bestLambdas[i] = 0;
                    }//if
                }//if
            }//for i
            return res;
        }

        public void Resize(int n)
        {
            this.N = n;
            Array.Resize(ref x, n);
        }

        public void Shuffle(int[] reorder)
        {
            ShuffleBaseAndOrder(reorder);
            ConstructMatrix();
            SetLambdas();
            ExecuteAlgorithm();
        }

        private void SetLambdas()
        {
            double coeff = 1.0 / Count;
            for (int i = 0; i < Count; i++)
            {
                components[i].Lambda = coeff;
            }
        }

        private void ConstructMatrix()
        {
            int memoCount = Count;
            int lastIndex = memoCount - 1;
            Count = 0;
            for (int i = 0; i < memoCount; i++)
            {
                if (AddToMatrix(components[Count].B))
                {
                    Count++;
                }
                else
                {
                    for (int j = Count; j < lastIndex; j++)
                    {
                        var tmpVariable = components[j];
                        components[j] = components[j + 1];
                        components[j + 1] = tmpVariable;
                    }
                    lastIndex--;
                }
            }
        }

        private void ShuffleBaseAndOrder(int[] reorder)
        {
            int prevN = reorder.Length;
            for (int i = 0; i < Count; i++)
            {
                var component = components[i];
                var order = (int[])components[i].Data;
                var b = components[i].B;
                for (int j = 0; j < prevN; j++)
                {
                    vector[j] = b[j];
                }
                int cnt = 0;
                for (int j = 0; j < prevN; j++)
                {
                    int cur = reorder[j];
                    if (cur != -1)
                    {
                        b[cur] = vector[j];
                    }
                    int replace = reorder[order[j]];
                    if (replace != -1)
                    {
                        order[cnt++] = replace;
                    }
                }
            }
        }

        private void Reset(int index, int[] order, double[] b)
        {
            components[index].Data = order;
            components[index].B = b;
        }

        public void ReplaceX(double[] other)
        {
            x = other;
            N = other.Length;
        }

        public void Delete(List<int> list)
        {
            double sum = 1;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                sum-=components[i].Lambda;
                Delete(list[i]);
            }
            foreach (var component in components)
            {
                component.Lambda /= sum;
            }
            SetX();
        }

    }
}
