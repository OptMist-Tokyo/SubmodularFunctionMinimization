using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public abstract class IO : SubmodularFunctionMinimization
    {
        double[] vals;   //for calculation
        int[] nextOrder;   //for calculation
        int[] nextLevels;   //for calculation
        double[] nextBase;  //for calculation


        protected void InitializeIO(SubmodularOracle oracle,double absEps,double relativeEps)
        {
            Initialize(oracle, absEps, relativeEps);
            InitializeForLevel();
            dataForContract = new DataForContract(N);
            contractOracle = new ContractOracle(oracle);
            this.oracle = contractOracle;
            vals = new double[N];
            nextOrder = new int[N];
            nextLevels = new int[N];
            nextBase = new double[N];

            var firstOrders = GetFirstOrders();
            SetFirstComponents(firstOrders);
            SetX();
        }

        private List<int[]> GetFirstOrders()
        {
            return GetFirstTrivialOrders();
        }

        protected int[] NextOrder
        {
            get { return nextOrder; }
        }

        //protected HashSet<int> Remainder
        //{
        //    get { return remainder; }
        //}

        protected double GetMaxValue()
        {
            double res = double.MinValue;
            foreach (var v in dataForContract.Remainder)
            {
                res = Math.Max(res, x[v]);
            }//foreach v
            return res;
        }

        protected double GetMinValue()
        {
            double res = double.MaxValue;
            foreach (var v in dataForContract.Remainder)
            {
                res = Math.Min(res, x[v]);
            }//foreach v
            return res;
        }

        protected void Wave(double eta)
        {
            int minLevelVertex; int minLevelComponentIndex;
            int cntChanged;int cntOffset;
            double delta = eta / (4 * N);
            double mu = delta;
            bool increased = false;
            while (mu <= eta && !increased)
            {
                mu = UpdateMu(mu, delta);
                if (!GetMinimizers(mu, out minLevelVertex, out minLevelComponentIndex))
                {
                    break;
                }//if
                NewPermutation(mu, minLevelVertex, minLevelComponentIndex,out cntOffset,out cntChanged);
                Push(mu, cntOffset,cntChanged, minLevelComponentIndex);
                increased = DoesLevelUp(dataForContract.Remainder);
            }//while
            var deletedComponents = components.Reduce(N);
            AdjustLevel(deletedComponents);
            DoesLevelUp(dataForContract.Remainder);
            RemoveGap(nextOrder);
            N = oracle.N;
        }

        private void AdjustLevel(List<ConvexComponent> deletedComponents)
        {
            foreach (var component in deletedComponents)
            {
                AdjustLevel(((ConvexComponentWithLevel)component).Levels,-1,dataForContract.Remainder);
            }//foreach component
        }

            //private void Reduce()
        //{
        //    int prevCount = remainder.Count;
        //    foreach (var v in reducedVertices)
        //    {
        //        remainder.Remove(v);
        //    }//foreach v
        //    RemoveOrder(prevCount, nextOrder, reducedVertices);
        //    SetBaseAndX();
        //}

        private void Push(double mu, int cntOffset,int cntChanged, int minLevelComponentIndex)
        {
            var component = components[minLevelComponentIndex];
            var prevBase = component.B;
            oracle.CalcBase(nextOrder, nextBase);
            double exchangeValue = CalcExchangeValue(mu,cntOffset, cntChanged, component, prevBase);
            CopyComponent(component, exchangeValue);
        }

        private void CopyComponent(ConvexComponent component, double exchangeValue)
        {
            ConvexComponentWithLevel nextComponent;
            if (exchangeValue != component.Lambda)
            {
                component.Lambda -= exchangeValue;
                nextComponent = components.GetNewComponentWithLevel();
            }//if
            else
            {
                var castedComponent = (ConvexComponentWithLevel)component;
                AdjustLevel(castedComponent.Levels, -1, dataForContract.Remainder);
                nextComponent = castedComponent;
            }//else
            nextComponent.Lambda = exchangeValue;
            for (int i = 0; i < N; i++)
            {
                nextComponent.Order[i] = nextOrder[i];
                int cur = nextOrder[i];
                nextComponent.B[cur] = nextBase[cur];
                nextComponent.Levels[cur] = nextLevels[cur];
            }//for i
            AdjustLevel(nextComponent.Levels, +1, dataForContract.Remainder);
            SetX();
        }

        private double CalcExchangeValue(double mu,int cntOffset, int cntChanged, ConvexComponent component, double[] prevBase)
        {
            double exchangeValue = component.Lambda;
            for (int i = cntOffset; i < cntOffset+ cntChanged; i++)
            {
                int v = nextOrder[i];
                if (RoundingValue(nextBase[v], -prevBase[v]) != 0)
                {
                    exchangeValue = Math.Min(exchangeValue, (x[v] - mu) / (prevBase[v] - nextBase[v]));
                }//if
            }//for i
            return exchangeValue;
        }

        private void NewPermutation(double mu,int minLevelVertex,int minLevelComponentIndex,out int cntOffset ,out int cntChanged)
        {
            var component = (ConvexComponentWithLevel)components[minLevelComponentIndex];
            var levels = component.Levels;
            var order = component.Order;
            int minLevel = minLevels[minLevelVertex];
            int cntComeAhead;
            CountComeAhead(mu, levels, order, minLevel, out cntComeAhead, out cntOffset);
            cntChanged =  SetNextOrderAndLevel(mu, levels, order, minLevel, cntComeAhead,cntOffset);
        }

        private void CountComeAhead(double mu, int[] levels, int[] order, int minLevel, out int cntComeAhead, out int cntOffset)
        {
            cntComeAhead = 0;
            cntOffset = 0;
            bool done = false;
            for (int i = 0; i < N; i++)
            {
                int v = order[i];
                if (levels[v] != minLevel)
                {
                    if (done)
                    {
                        break;                        
                    }//if
                    cntOffset++;
                }//if
                else
                {
                    done = true;
                    if (x[v] < mu)
                    {
                        cntComeAhead++;
                    }//if
                }//else
            }//for i
        }

        private int SetNextOrderAndLevel(double mu, int[] levels, int[] order, int minLevel, int cntComeAhead,int cntOffset)
        {
            int cntChanged = 0;
            int posStart = cntOffset;
            int posEnd = cntOffset + cntComeAhead;
            for (int i = 0; i < N; i++)
            {
                int cur = order[i];
                nextLevels[cur] = levels[cur];

                if (levels[cur] != minLevel)
                {
                    nextOrder[i] = cur;
                }//if
                else
                {
                    cntChanged++;
                    if (x[cur] < mu)
                    {
                        nextOrder[posStart++] = cur;
                    }//if
                    else
                    {
                        nextOrder[posEnd++] = cur;
                        nextLevels[cur]++;
                    }//else
                }//else
            }//for i
            return cntChanged;
        }

        private bool GetMinimizers(double mu,out int minLevelVertex,out int minLevelComponentIndex)
        {
            minLevelVertex = -1;
            minLevelComponentIndex = -1;
            foreach (var v in dataForContract.Remainder)
            {
                if (x[v]>mu&&(minLevelVertex==-1||minLevels[minLevelVertex]>minLevels[v]))
                {
                    minLevelVertex = v;
                }//if
            }//foreach v
            if (minLevelVertex==-1)
            {
                return false;
            }//if
            for (int i = 0; i < components.Count; i++)
            {
                var level = ((ConvexComponentWithLevel)components[i]).Levels;
                if (minLevels[minLevelVertex]==level[minLevelVertex])
                {
                    minLevelComponentIndex = i;
                    break;
                }//if
            }//for i
            return true;
        }

        private double UpdateMu(double mu, double delta)
        {
            int pos = 0;
            foreach (var v in dataForContract.Remainder)
            {
                if (x[v]+delta>=mu)
                {
                    vals[pos++] = x[v];                    
                }//if
            }//foreach v
            Array.Sort(vals, 0, pos);
            for (int i = 0; i < pos; i++)
            {
                if (vals[i]-delta>=mu)
                {
                    break;
                }//if
                mu = vals[i] + delta;
            }//for i
            return mu;
        }

        protected void RemoveGap(int[] nextOrder)
        {
            int gap = CalcGapPos();
            if (gap >= 0)
            {
                RemoveGap(gap, nextOrder);
            }//if
            N = oracle.N;
        }

        private void RemoveGap(int gap, int[] nextOrder)
        {
            dataForContract.DeletedVertices.Clear();
            foreach (var v in dataForContract.Remainder)
            {
                if (minLevels[v] > gap)
                {
                    contractOracle.Delete(v);
                    dataForContract.DeletedVertices.Add(v);
                }//if
            }//foreach v
            Delete(dataForContract.DeletedVertices, dataForContract.Remainder, nextOrder);
        }

        private void Delete(HashSet<int> deletedVertices, HashSet<int> remainder, int[] nextOrder)
        {
            int prevCount = remainder.Count;
            foreach (var v in deletedVertices)
            {
                remainder.Remove(v);
            }//foreach v
            RemoveOrder(prevCount, nextOrder, deletedVertices);
            SetBaseAndX();
            N = oracle.N;
        }

        protected override ConvexComponent GetNewComponent()
        {
            return components.GetNewComponentWithLevel();
        }

    }
}
