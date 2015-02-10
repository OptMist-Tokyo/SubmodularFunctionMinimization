using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public abstract class IFF:ScalingAlgorithm
    {


        protected override void InitializeAlgorithm(SubmodularOracle oracle, double absEps, double relativeEps)
        {
            InitializeFlowScaling(oracle, absEps, relativeEps);
        }

        
        protected override void DoScalingPhase(ref double delta,double endValue)
        {
            int componentIndex; int orderIndex; int terminalOfAugmentingPath;
            SetCurrentVerticse();
            SetVariable();
            while (RoundingValue( 2*delta,-endValue)>0)
            {
                Iteration++;
                ModifyPhi(ref delta);
                terminalOfAugmentingPath = SetVertices(delta);
                while (terminalOfAugmentingPath>=0||ExistActiveTriile(out componentIndex,out orderIndex))
                {
                    while (terminalOfAugmentingPath < 0 && ExistActiveTriile(out componentIndex, out orderIndex))
                    {
                        DoubleExchange(componentIndex, orderIndex);
                        terminalOfAugmentingPath = SetReachableFromNegative();
                    }//while
                    if (terminalOfAugmentingPath>=0)
                    {
                        Augment(terminalOfAugmentingPath, delta);
                        terminalOfAugmentingPath = SetVertices(delta);
                    }//if
                    components.Reduce(N);
                }//while
            }//while
        }

        //[System.Diagnostics.Conditional("DEBUG")]
        //private void CheckPhi(double delta)
        //{
        //    this.delta = delta;
        //    for (int i = 0; i < N; i++)
        //    {
        //        for (int j = 0; j < N; j++)
        //        {
        //            if (phi[i][j]<0||Math.Abs(phi[i][j])-1e-6>delta)
        //            {
        //                System.Diagnostics.Debugger.Break();
        //            }//if
        //        }//for j
        //    }//for i
        //}

        //[System.Diagnostics.Conditional("DEBUG")]
        //private void CheckLambda()
        //{
        //    double sum = 0;
        //    foreach (var component in components)
        //    {
        //        if (component.Lambda<=0)
        //        {
        //            System.Diagnostics.Debugger.Break();
        //        }//if
        //        sum += component.Lambda;
        //    }//foreach item
        //    if (sum>1.01)
        //    {
        //        System.Diagnostics.Debugger.Break();
        //    }//if
        //}

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckPhi(int cur, int prev)
        {
            if (phi[prev][cur] < 0)
            {
                System.Diagnostics.Debugger.Break();
            }//if
        }

        private void DoubleExchange(int componentIndex,int orderIndex)
        {
            var oldComponent = components[componentIndex];
            var order =oldComponent.Order;
            int preceed = order[orderIndex]; //v
            int succeed = order[orderIndex+1];    //u
            Swap(orderIndex,orderIndex+ 1, order);
            double exchangeCaiacity = 0;
            exchangeCaiacity += oracle.CalcValue(order, orderIndex + 1);
            Swap(orderIndex, orderIndex + 1, order);
            exchangeCaiacity -= oracle.CalcValue(order, orderIndex + 2);
            exchangeCaiacity += oldComponent.B[preceed];
            exchangeCaiacity = Math.Max(0, exchangeCaiacity);
            double multExchangeCaiacity = oldComponent.Lambda * exchangeCaiacity;
            double alpha = Math.Min(phi[hash[succeed]][hash[preceed]], multExchangeCaiacity);
            x[succeed] += alpha;
            x[preceed] -= alpha;
            phi[hash[succeed]][hash[preceed]] -= alpha;
            if (alpha!=multExchangeCaiacity)
            {
                var newComponent = components.GetNewComponent();
                newComponent.Lambda = oldComponent.Lambda - alpha / exchangeCaiacity;
                oldComponent.Lambda = alpha / exchangeCaiacity;
                for (int i = 0; i < N; i++)
                {
                    newComponent.B[baseOrder[i]] = oldComponent.B[baseOrder[i]];
                    newComponent.Order[i] = oldComponent.Order[i];
                }//for i
            }//if
            oldComponent.B[succeed] += exchangeCaiacity;
            oldComponent.B[preceed] -= exchangeCaiacity;
            Swap(orderIndex, orderIndex + 1, oldComponent.Order);
        }

        //[System.Diagnostics.Conditional("DEBUG")]
        //private static void CheckLambda(ConvexComponent oldComponent, double exchangeCaiacity, double alpha)
        //{
        //    if (oldComponent.Lambda - alpha / exchangeCaiacity <= 0 || alpha / exchangeCaiacity <= 0)
        //    {
        //        System.Diagnostics.Debugger.Break();
        //    }//if
        //}

        //[System.Diagnostics.Conditional("DEBUG")]
        //private void CheckBaseValue(ConvexComponent oldComponent, int succeed, double exchangeCaiacity)
        //{
        //    if (RoundingValue(oldComponent.B[succeed], -exchangeCaiacity) != 0)
        //    {
        //       // System.Diagnostics.Debugger.Break();
        //    }//if
        //}

        private void Swap(int index0, int index1, int[] array)
        {
            var tmpVariable = array[index0];
            array[index0] = array[index1];
            array[index1] = tmpVariable;
        }

        private bool ExistActiveTriile(out int componentIndex, out int orderIndex)
        {
            int end = oracle.N - 1;
            for(int k=0;k<components.Count;k++)
            {
                var order = components[k].Order;
                for (int i = 0; i < end; i++)
                {
                    if (!reachableFromNegative.Contains(hash[order[i]])&&reachableFromNegative.Contains(hash[order[i+1]]))
                    {
                        orderIndex = i;
                        componentIndex = k;
                        return true;
                    }//if
                }//for i
            }//for k
            orderIndex = componentIndex = -1;
            return false;
        }

   

    }
}
