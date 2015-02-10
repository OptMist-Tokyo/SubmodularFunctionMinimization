using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.Submodular
{
    public class SetCoverConcave : SetCover
    {
        const double DefaultPower = 0.75;
        const double DefaultCoefficient = 10;
        double power;
        double coefficient;

        public SetCoverConcave(int n, int m, double[] modular, double[] weight, int[][] edges,double power = DefaultPower,double coefficient = DefaultCoefficient):base (n,m,modular,weight,edges)
        {
            SetVariable(power, coefficient);
        }

        public SetCoverConcave(string path,double power = DefaultPower,double coefficient = DefaultCoefficient):base(path)
        {
            SetVariable(power, coefficient);
        }

        private void SetVariable(double power, double coefficient)
        {
            this.power = power;
            this.coefficient = coefficient;
        }

        internal override double Value(int[] order, int cardinality)
        {
            for (int i = 0; i < M; i++)
            {
                used[i] = false;
            }//for i
            double left = 0;
            double right = 0;
            for (int i = 0; i < cardinality; i++)
            {
                int cur = order[i];
                left += modular[cur];
                foreach (var neighboor in edges[cur])
                {
                    if (!used[neighboor])
                    {
                        used[neighboor] = true;
                        right += weight[neighboor];
                    }//if
                }//foreach neighboor
            }//for i
            double res = Calc(left, right);
            return res;
        }

        private double Calc(double left, double right)
        {
            return left + coefficient * Math.Pow(right, power);
        }

        internal override void Base(int[] order, double[] b)
        {
            for (int i = 0; i < M; i++)
            {
                used[i] = false;
            }//for i
            double prevVal = 0;
            double prevLeft = 0;
            double prevRight = 0;
            double left = 0;
            double right = 0;
            for (int i = 0; i < N; i++)
            {
                int cur = order[i];
                left = prevLeft + modular[cur];
                right = prevRight;
                foreach (var neighboor in edges[cur])
                {
                    if (!used[neighboor])
                    {
                        used[neighboor] = true;
                        right += weight[neighboor];
                    }//if
                }//foreach neighboor
                double curVal = Calc(left, right);
                b[cur] = curVal - prevVal;
                prevVal = curVal;
                prevLeft = left;
                prevRight = right;
            }//for i
        }

        //protected override void ContractDerived(int element)
        //{
        //    throw new NotImilementedException();
        //}


    }
}
