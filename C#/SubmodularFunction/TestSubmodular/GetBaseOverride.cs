using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Onigiri.TestSubmodular
{
    [TestClass]
    public class GetBaseOverride
    {
        const int nMin = 1;
        const int nMax = 70;
        const int kMin = 0;
        const int kMax = 24;
        const int numSubstring = 15;

        [TestMethod]
        public void GetBaseOverrideModular()
        {
            Action<int,int,int[]> action = (n, k, order) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix+ methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.Modular(path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        [TestMethod]
        public void GetBaseOverrideUndirectedCut()
        {
            Action<int, int, int[]> action = (n, k, order) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.UndirectedCut( path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        [TestMethod]
        public void GetBaseOverrideDirectedCut()
        {
            Action<int, int, int[]> action = (n, k, order) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.DirectedCut(path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        [TestMethod]
        public void GetBaseOverrideConnectedDetachment()
        {
            Action<int, int, int[]> action = (n, k, order) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.ConnectedDetachment(path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        [TestMethod]
        public void GetBaseOverrideFacilityLocation()
        {
            Action<int, int, int[]> action = (n, k, order) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.FacilityLocation(path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        [TestMethod]
        public void GetBaseOverrideGraphicMatroid()
        {
            Action<int, int, int[]> action = (n, k, order) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.GraphicMatroid(path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        [TestMethod]
        public void GetBaseOverrideBinaryMatrixRank()
        {
            Action<int, int, int[]> action = (n, k, order) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.BinaryMatroid(path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        [TestMethod]
        public void GetBaseOverrideNonPositiveSymmetricMatrixSummation()
        {
            Action<int, int, int[]> action = (n, k, order) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.NonPositiveSymmetricMatrixSummation(path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        [TestMethod]
        public void GetBaseOverrideSetCover()
        {
            Action<int, int, int[]> action = (n, k, order) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.SetCover(path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        [TestMethod]
        public void GetBaseOverrideSetCoverConcave()
        {
            Action<int, int, int[]> action = (n, k, order) =>
            {
                //var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + "SetCoverConcave" + @"\" + n + "_" + k;
                var func = new Submodular.SetCoverConcave(path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        [TestMethod]
        public void GetBaseOverrideKyFan()
        {
            Action<int, int, int[]> action = (n, k, order) =>
            {
                //var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + "KyFan" + @"\" + n + "_" + k;
                var func = new Submodular.KyFan(path);
                func.TestGetBase(order);
            };
            TestGetBase(action);
        }

        private static void TestGetBase(Action<int, int, int[]> action)
        {
            for (int n = nMin; n < nMax; n++)
            {
                var order0 = Enumerable.Range(0, n).ToArray();
                var order1 = Enumerable.Range(0, n).Reverse().ToArray();
                var order2 = Enumerable.Range(0, (n + 1) / 2).Select(x => 2 * x).Concat(
                    Enumerable.Range(0, n / 2).Select(x => 2 * x + 1)).ToArray();
                for (int k = kMin; k <= kMax; k++)
                {
                    action.Invoke(n, k, order0);
                    action.Invoke(n, k, order1);
                    action.Invoke(n, k, order2);
                }//for k
            }//for n        
        }


    }
}
