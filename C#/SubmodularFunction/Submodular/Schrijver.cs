using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class Schrijver:SubmodularFunctionMinimization
    {
        const int Inf = (int)1e9;
#if DEBUG
#endif

        int[] distFromPositive;    //for calculation
        List<double[]> bases;   //for calculation of elimination
        double[] exchangeCoefficient;   //for calculation
        Dictionary<int, int>[] edges;

        public Schrijver()
        {
            bases = new List<double[]>();
        }

        public override SFMResult Minimization(SubmodularOracle oracle, double absEps=DefaultAbsEps, double relativeEps=DefaultRelativeEps)
        {
            InitializeSchrijver(oracle, absEps, relativeEps);
            IteratingPhase();
            SetMinimizer();
            return SetResults(oracle);
        }

        private void SetMinimizer()
        {
            var que = new Queue<int>();
            for (int i = 0; i < minimizer.Length; i++)
            {
                if (x[i] < 0)
                {
                    que.Enqueue(i);
                    minimizer[i] = true;
                }//if
            }//for i
            while (que.Count != 0)
            {
                int deq = que.Dequeue();
                for (int i = 0; i < minimizer.Length; i++)
                {
                    if (!minimizer[i] && edges[i].ContainsKey(deq))
                    {
                        que.Enqueue(i);
                        minimizer[i] = true;
                    }//if
                }//for i
            }//while
        }

        private void IteratingPhase()
        {
            while (ExistPathFromPositiveToNegative())
            {
                Iteration++;
                int decreaseElement;
                int increaseElement;
                int componentIndex;
                FindMaxmizers(out decreaseElement,out increaseElement,out componentIndex);
                int prevNumComponent = components.Count;
                CheckLambdaSum();
                int cntAdded = Exchange(componentIndex, decreaseElement, increaseElement);
                CheckLambdaSum();
                ModifiyEdges(componentIndex, prevNumComponent);
                CheckLambdaSum();
                Reduce();
                CheckLambdaSum();
            }//while
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckLambdaSum()
        {
            double sum = 0;
            for (int i = 0; i < components.Count; i++)
            {
                sum += components[i].Lambda;
            }//for i
            if (sum >= 1.01)
            {
                System.Diagnostics.Debugger.Break();
            }//if
        }

        private void Reduce()
        {
            var deletedList = components.Reduce(N);
            foreach (var component in deletedList)
            {
                DeleteEdges(component.Order);
            }//foreach component
        }

        private void FindMaxmizers(out int decreaseElement, out int increaseElement, out int componentIndex)
        {
            increaseElement = -1;
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i]<0&&distFromPositive[i]!=Inf&&(increaseElement==-1||distFromPositive[increaseElement]<=distFromPositive[i]))
                {
                    increaseElement = i;
                }//if
            }//for i

            decreaseElement = -1;
            for (int i = distFromPositive.Length - 1; i >= 0; i--)
            {
                if (distFromPositive[i] == distFromPositive[increaseElement] - 1 && (decreaseElement == -1)&&edges[i].ContainsKey(increaseElement))
                {
                    decreaseElement = i;
                    break;
                }//if
            }//forrev i

            int memo = -1;
            componentIndex = -1;
            for (int i = 0; i < components.Count; i++)
            {
                int[] inverseOrder = ((ConvexComponentWithInverseOrder)components[i]).InverseOrder;
                int cur = inverseOrder[increaseElement]-inverseOrder[decreaseElement];
                if (memo<=cur)
                {
                    componentIndex = i;
                    memo = cur;
                }//if
            }//for i
        }

        private void ModifiyEdges(int componentIndex, int prevNumComponent)
        {
            for (int i = components.Count - 1; i >= prevNumComponent; i--)
            {
                AddEdges((ConvexComponentWithInverseOrder)components[i]);
            }//forrev i
            if (components[componentIndex].Lambda==0)
            {
                DeleteEdges(components[componentIndex].Order);
                components.Delete(componentIndex);
            }//if
        }

        private bool ExistPathFromPositiveToNegative()
        {
            var que = new Queue<int>();
            for (int i = 0; i < distFromPositive.Length; i++)
            {
                distFromPositive[i] = Inf;
                if (x[i]>0)
                {
                    distFromPositive[i] = 0;
                    que.Enqueue(i);
                }//if
            }//for i
            while (que.Count!=0)
            {
                int cur = que.Dequeue();
                int nextDist = distFromPositive[cur]+1;
                foreach (var next in edges[cur].Keys)
                {
                    if (distFromPositive[next]==Inf)
                    {
                        distFromPositive[next] = nextDist;
                        que.Enqueue(next);
                    }//if
                }//foreach next
            }//while
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i]<0&&distFromPositive[i]!=Inf)
                {
                    return true;
                }//if
            }//for i
            return false;
        }

        private void AddEdges(ConvexComponentWithInverseOrder component)
        {
            var order = component.Order;
            var inverseOrder = component.InverseOrder;
            for (int i = 0; i < order.Length; i++)
            {
                int from = order[i];
                inverseOrder[from] = i;
                for (int j = i+1; j < order.Length; j++)
                {
                    int to = order[j];
                    if (!edges[from].ContainsKey(to))
                    {
                        edges[from][to] = 1;
                    }//if
                    else
                    {
                        edges[from][to]++;
                    }//else
                }//for j
            }//for i
        }

        private void DeleteEdges(int[] order)
        {
            for (int i = 0; i < order.Length; i++)
            {
                int from = order[i];
                for (int j = i + 1; j < order.Length; j++)
                {
                    int to = order[j];
                    edges[from][to]--;
                    if (edges[from][to] == 0)
                    {
                        edges[from].Remove(to);
                    }//if
                }//for j
            }//for i
        }

        private void InitializeSchrijver(SubmodularOracle oracle, double absEps , double relativeEps )
        {
            Initialize(oracle, absEps, relativeEps);
            bases.Clear();
            exchangeCoefficient = new double[N+1];
            edges = new Dictionary<int, int>[N];
            for (int i = 0; i < edges.Length; i++)
            {
                edges[i] = new Dictionary<int, int>();
            }//for i
            distFromPositive = new int[N];

            var firstOrders = GetFirstTrivialOrders();
            SetFirstComponents(firstOrders);
            SetX();
        }

        protected override ConvexComponent GetNewComponent()
        {
            return components.GetNewComponentWithInverseOrder();
        }

        protected override void ActForNewConvexComponent(ConvexComponent component)
        {
            AddEdges((ConvexComponentWithInverseOrder)component);
        }
        
        //private List<int[]> GetFirstOrders()
        //{
        //    return GetFirstTrivialOrders();
        //}

        private int Exchange(int componentIndex, int decreaceElement, int increaseElement)
        {
            int firstCount = components.Count;
            int decreaseIndex; int increaseIndex;
            var component = components[componentIndex];
            GetIndexOfElement(decreaceElement, increaseElement, out decreaseIndex, out increaseIndex, component);
            CheckIndexRange(decreaseIndex, increaseIndex);
            IncreaseMemory(decreaseIndex, increaseIndex);
            int cntAdded;
            int zeroIndex = MakeMatrix(decreaseIndex, increaseIndex, component);
            if (zeroIndex >= 0)
            {
                 cntAdded= ExchangeTrivial(componentIndex, firstCount, zeroIndex);
            }//if
            else
            {
                cntAdded= ExchangeWithElimination(decreaceElement,increaseElement, componentIndex,firstCount);
            }//else
            return cntAdded;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private static void CheckIndexRange(int decreaseIndex, int increaseIndex)
        {
            if (decreaseIndex >= increaseIndex)
            {
                System.Diagnostics.Debugger.Break();
            }//if
        }

        private int ExchangeTrivial(int componentIndex, int firstCount, int zeroIndex)
        {
            components.Swap(firstCount, firstCount + zeroIndex);
            for (int i = components.Count - 1; i > firstCount; i--)
            {
                components.Delete(i);
            }//forrev i
            components[firstCount].Lambda = components[componentIndex].Lambda;
            components[componentIndex].Lambda = 0;
            return 1;
        }

        private int ExchangeWithElimination(int decreaseElement,int increaseElement,int componentIndex,int firstCount)
        {
            int len = components.Count-firstCount;
            double norm = FillExchangeCoefficientVector(len);
            double q = ArgmentX(decreaseElement, increaseElement, componentIndex, len, norm);
            int cntAdded = AdjustCoefficient(componentIndex,firstCount, len, norm, q);
            return cntAdded;
        }

        private int AdjustCoefficient(int componentIndex,int firstCount, int len, double norm, double q)
        {
            var component = components[componentIndex];
            int cntAdded = len;
            for (int i = components.Count - 1; i >= firstCount; i--)
            {
                components[i].Lambda = components[componentIndex].Lambda * (exchangeCoefficient[i - firstCount] / norm) * q;
                if (components[i].Lambda < AbsEps)
                {
                    cntAdded--;
                    components[i].Lambda = 0;
                    components.Delete(i);
                }//if                
            }//forrev i
            component.Lambda *= (1 - q);
            if (component.Lambda<AbsEps)
            {
                component.Lambda = 0;
            }//if
            return cntAdded;
        }

        private double ArgmentX(int decreaseIndex, int increaseIndex, int componentIndex, int len, double norm)
        {
            double delta = components[componentIndex].Lambda * (exchangeCoefficient[len - 1] / norm) * bases[len - 1][len];
            double q;
            if (delta + x[increaseIndex] > 0)
            {
                q = -x[increaseIndex] / delta;
                x[decreaseIndex] += x[increaseIndex];
                x[increaseIndex] = 0;
            }
            else
            {
                q = 1;
                x[decreaseIndex] -= delta;
                x[increaseIndex] += delta;
            }//else
            return q;
        }

        private double FillExchangeCoefficientVector(int len)
        {
            double norm = 1;
            exchangeCoefficient[len - 1] = 1;
            for (int i = len - 2; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < len; j++)
                {
                    sum += bases[j][i + 1] * exchangeCoefficient[j];
                }//for j
                exchangeCoefficient[i] = -sum / bases[i][i + 1];
                norm += exchangeCoefficient[i];
            }//forrev i
            return norm;
        }

        private int MakeMatrix( int decreaseIndex, int increaseIndex, ConvexComponent component)
        {
            var order = component.Order;
            for (int i = decreaseIndex + 1; i <= increaseIndex; i++)
            {
                var newComponent = components.GetNewComponentWithInverseOrder();
                var newOrder = GetExchangedOrder(decreaseIndex, order, i, newComponent);
                var b = newComponent.B;
                oracle.CalcBase(newOrder, b);
                int pos = i - (decreaseIndex + 1);
                for (int k = 0; k <= pos; k++)
                {
                    bases[pos][k] = b[order[decreaseIndex + k]] - component.B[order[decreaseIndex + k]];
                }//for k
                bases[pos][pos + 1] = RoundingValue(b[order[decreaseIndex + pos + 1]], - component.B[order[decreaseIndex + pos + 1]]);
                if (bases[pos][pos+1]==0)
                {
                    return pos;
                }//if
            }//for i
            return -1;
        }

        private int[] GetExchangedOrder(int decreaseIndex, int[] order, int i, ConvexComponent newComponent)
        {
            var newOrder = newComponent.Order;
            newOrder[decreaseIndex] = order[i];
            int index = 0;
            for (int k = 0; k < newOrder.Length; k++)
            {
                if (k != decreaseIndex)
                {
                    if (order[index] == newOrder[decreaseIndex])
                    {
                        index++;
                    }//if
                    newOrder[k] = order[index++];
                }//if
            }//for k
            return newOrder;
        }

        private void IncreaseMemory(int decreaseIndex, int increaseIndex)
        {
            int range = increaseIndex - decreaseIndex;
            for (int i = bases.Count; i < range; i++)
            {
                bases.Add(new double[i + 2]);
            }//for i
        }

        private static void GetIndexOfElement(int decreaceElement, int increaseElement, out int decreaseIndex, out int increaseIndex, ConvexComponent component)
        {
            var order = component.Order;
            decreaseIndex = increaseIndex = -1;
            for (int i = 0; i < order.Length; i++)
            {
                if (order[i] == decreaceElement)
                {
                    decreaseIndex = i;
                }//if
                else if (order[i] == increaseElement)
                {
                    increaseIndex = i;
                }//if
            }//for i
        }


    }
}
