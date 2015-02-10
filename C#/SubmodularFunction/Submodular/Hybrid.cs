using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public abstract class Hybrid:ScalingAlgorithm
    {
        int[] label;
        int[] previousLeftVertices; //R
        int[] previousRightVertices;    //Q
        HashSet<int> minimizersOfLabel;    //Z
        double[][] flow;
        int[] nextOrder;
        double[] nextBase;
        List<int>[] graphByOrder;  //A_I
        ConvexComponents copyComponent;


        protected override void InitializeAlgorithm(SubmodularOracle oracle, double absEps, double relativeEps)
        {
            InitializeFlowScaling(oracle, absEps, relativeEps);
            copyComponent = new ConvexComponents(N, AbsEps, RelativeEps);
            label = new int[N];
            minimizersOfLabel = new HashSet<int>();
            previousLeftVertices = new int[N];
            previousRightVertices = new int[N];
            nextBase = new double[N];
            nextOrder = new int[N];
            flow = new double[N][];
            graphByOrder = new List<int>[N];
            for (int i = 0; i < N; i++)
            {
                flow[i] = new double[N];
                graphByOrder[i] = new List<int>();
            }//for i
        }
        
        protected override void DoScalingPhase(ref double delta, double endValue)
        {
            int componentIndex; int orderStartIndex; int orderLastIndex;
            int terminalOfAugmentingPath;
            SetCurrentVerticse();
            SetVariable();
            while (RoundingValue(delta,-endValue)>=0)
            {
                Iteration++;
                ModifyPhi(ref delta);
                InitializeLabel();
                while (true)
                {
                    terminalOfAugmentingPath = SetVertices(delta);
                    if (terminalOfAugmentingPath >= 0)
                    {
                        Augment(terminalOfAugmentingPath, delta);
                    }//if
                    else
                    {
                        int minimumLabel = GetMinimizer();
                        if (minimumLabel==N)
                        {
                            break;
                        }//if
                        if (ExistActiveTriile(minimumLabel,out componentIndex, out orderStartIndex,out orderLastIndex))
                        {
                            //CheckZAndX();
                            MultipleExchange(delta, componentIndex, orderStartIndex, orderLastIndex);
                            //CheckZAndX();
                        }//if
                        else
                        {
                            Relabel();
                        }//else
                    }//else
                    components.Reduce(N);
                }//while
            }//while
            SetAnswer(delta);
        }

        //[System.Diagnostics.Conditional("DEBUG")]
        //private void ChcekLambda()
        //{
        //    foreach (var component in components)
        //    {
        //        if (double.IsNaN( component.Lambda))
        //        {
        //            System.Diagnostics.Debugger.Break();
        //        }
        //    }
        //}

        private void SetAnswer(double delta)
        {
            SetGraih();
            SetReachable(delta);
        }

        private void SetReachable(double delta)
        {
            que.Clear();
            reachableFromNegative.Clear();
            for (int i = 0; i < N; i++)
            {
                if (RoundingValue(z[i],delta) <= 0)
                {
                    que.Enqueue(i);
                    reachableFromNegative.Add(i);
                }//if
            }//for i
            while (que.Count > 0)
            {
                int cur = que.Dequeue();
                foreach (var next in graphByOrder[cur])
                {
                    if (!reachableFromNegative.Contains(next))
                    {
                        que.Enqueue(next);
                        reachableFromNegative.Add(next);
                    }//if
                }//foreach next
            }//while
        }

        private void SetGraih()
        {
            for (int i = 0; i < N; i++)
            {
                graphByOrder[i].Clear();
            }//for i
            foreach (var component in components)
            {
                var order = component.Order;
                for (int i = 0; i < N - 1; i++)
                {
                    graphByOrder[hash[order[i + 1]]].Add(hash[order[i]]);
                }//for i
            }//foreach component
        }

        private void MultipleExchange(double delta,int componentIndex, int orderStartIndex, int orderLastIndex)
        {
            var component = components[componentIndex];
            var order = component.Order;
            var b = component.B;
            var lambda = component.Lambda;
            int cntLeft; int cntRight;

            MakeLeftAndRight(orderStartIndex, orderLastIndex, order, out cntLeft, out cntRight);
            FillOrderAndBase(orderStartIndex, orderLastIndex, order, cntLeft, cntRight);
            double maximumFlow = FillFlow(b, cntLeft, cntRight);
            double alpha = Math.Min(lambda, delta / maximumFlow);
            AdjustXandPhi(b, cntLeft, cntRight, alpha);
            UpdateComponent(component, order, b, lambda, alpha);
        }

        private void UpdateComponent(ConvexComponent component, int[] order, double[] b, double lambda, double alpha)
        {
            if (alpha != lambda)
            {
                var copyComponent = components.GetNewComponent();
                copyComponent.Lambda = component.Lambda - alpha;
                for (int i = 0; i < N; i++)
                {
                    copyComponent.Order[i] = order[i];
                    copyComponent.B[baseOrder[i]] = b[baseOrder[i]];
                }//for i
            }//if
            component.Lambda = alpha;
            for (int i = 0; i < N; i++)
            {
                order[i] = nextOrder[i];
                b[baseOrder[i]] = nextBase[baseOrder[i]];
            }//for i
        }

        private void AdjustXandPhi(double[] b, int cntLeft, int cntReigt, double alpha)
        {
            for (int i = 0; i < N; i++)
            {
                x[baseOrder[i]] += alpha * (nextBase[baseOrder[i]] - b[baseOrder[i]]);
            }//for i
            for (int i = 0; i < cntReigt; i++)
            {
                for (int j = 0; j < cntLeft; j++)
                {
                    int right = hash[previousRightVertices[i]];
                    int left = hash[previousLeftVertices[j]];
                    double val =alpha*flow[i][j];
                    if (phi[left][right]>val)
                    {
                        phi[left][right] -= val;
                    }//if
                    else
                    {
                        phi[right][left] = val - phi[left][right];
                        phi[left][right] = 0;
                    }//else
                }//for j
            }//for i
        }

        private double FillFlow(double[] b, int cntLeft, int cntReigt)
        {
            int oppositePos = -1;
            double oppositeRemFlow = 0;
            double maximumFlow = 0;
            for (int i = 0; i < cntReigt; i++)
            {
                for (int j = 0; j < cntLeft; j++)
                {
                    flow[i][j] = 0;
                }//for j
                double remFlow = b[previousRightVertices[i]] - nextBase[previousRightVertices[i]];
                while (remFlow > 0)
                {
                    if (oppositeRemFlow == 0)
                    {
                        oppositePos++;
                        if (oppositePos >= cntLeft)
                        {
                            //CheckVal(oppositeRemFlow);
                            break;
                        }//if
                        oppositeRemFlow = nextBase[previousLeftVertices[oppositePos]] - b[previousLeftVertices[oppositePos]];
                    }//if
                    double currentFlow = Math.Min(remFlow, oppositeRemFlow);
                    maximumFlow = Math.Max(maximumFlow, currentFlow);
                    flow[i][oppositePos] = currentFlow;
                    if (remFlow == currentFlow)
                    {
                        remFlow = 0;
                        oppositeRemFlow -= currentFlow;
                    }//if
                    else
                    {
                        remFlow -= currentFlow;
                        oppositeRemFlow = 0;
                    }//else
                }//while
            }//for i
            return maximumFlow;
        }

        private void FillOrderAndBase(int orderStartIndex, int orderLastIndex, int[] order, int cntLeft, int cntReigt)
        {
            for (int i = 0; i < orderStartIndex; i++)
            {
                nextOrder[i] = order[i];
            }//for i
            for (int i = 0; i < cntLeft; i++)
            {
                nextOrder[i + orderStartIndex] = previousLeftVertices[i];
            }//for i
            for (int i = 0; i < cntReigt; i++)
            {
                nextOrder[i +orderStartIndex+ cntLeft] = previousRightVertices[i];
            }//for i
            for (int i = orderLastIndex + 1; i < N; i++)
            {
                nextOrder[i] = order[i];
            }//for i

            oracle.CalcBase(nextOrder, nextBase);
        }

        private void MakeLeftAndRight(int orderStartIndex, int orderLastIndex, int[] order, out int cntLeft, out int cntReigt)
        {
            cntLeft = 0;
            cntReigt = 0;
            for (int i = orderStartIndex; i <= orderLastIndex; i++)
            {
                int cur = hash[order[i]];
                if (reachableFromNegative.Contains(cur))
                {
                    previousLeftVertices[cntLeft++] = baseOrder[cur];
                }//if
                else
                {
                    previousRightVertices[cntReigt++] = baseOrder[cur];
                }//else
            }//for i
        }

        //[System.Diagnostics.Conditional("DEBUG")]
        //private void CheckVal(double remFlow)
        //{
        //    if (remFlow>1e-3)
        //    {
        //        System.Diagnostics.Debugger.Break();
        //    }//if
        //}
        
        private bool ExistActiveTriile(int minimumLable,out int componentIndex, out int orderStartIndex,out int orderLastIndex)
        {
            for (componentIndex = 0; componentIndex < components.Count; componentIndex++)
            {
                orderStartIndex = int.MaxValue;
                orderLastIndex = int.MinValue;
                var order = components[componentIndex].Order;
                for (int i = 0; i < N; i++)
                {
                    int v = hash[order[i]];
                    int currentLabel = label[v];
                    if (currentLabel==minimumLable&&orderStartIndex==int.MaxValue&&minimizersOfLabel.Contains(v))
                    {
                        orderStartIndex = i;                      
                    }//if
                    if (currentLabel == minimumLable-1&&reachableFromNegative.Contains(v))
                    {
                        orderLastIndex = i;
                    }//if
                }//for i
                if (orderStartIndex<orderLastIndex)
                {
                    return true;
                }//if
            }//for componentIndex
            orderStartIndex = int.MaxValue;
            orderLastIndex = int.MinValue;
            return false;
        }

        private void Relabel()
        {
            foreach (var v in minimizersOfLabel)
            {
                label[v]++;
            }//foreach v
        }

        private int GetMinimizer()
        {
            int minValue = N;
            for (int i = 0; i < N; i++)
            {
                if (reachableFromNegative.Contains(i))
                {
                    continue;
                }//if
                if (label[i]<minValue)
                {
                    minValue = label[i];
                    minimizersOfLabel.Clear();
                    minimizersOfLabel.Add(i);
                }//if
                else if (label[i]==minValue)
                {
                    minimizersOfLabel.Add(i);
                }//if
            }//for i
            return minValue;
        }

        private void InitializeLabel()
        {
            for (int i = 0; i < N; i++)
            {
                label[i] = 0;
            }//for i
        }

    }
}
