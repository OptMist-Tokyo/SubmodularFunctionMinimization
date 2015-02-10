using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.TestSubmodular
{
    [TestClass]
    public class IsSubmodularOverride
    {
        const int nMin = 1;
        const int nMax = 10;
        const int kMin = 0;
        const int kMax = 24;
        const int numSubstring = 20;

        [TestMethod]
        public void IsSubmodularOverrideModular()
        {
            Action<int, int> action = (n, k) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path =  Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.Modular(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        [TestMethod]
        public void IsSubmodularOverrideUndirectedCut()
        {
            Action<int, int> action = (n, k) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path =  Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.UndirectedCut(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        [TestMethod]
        public void IsSubmodularOverrideDirectedCut()
        {
            Action<int, int> action = (n, k) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path =  Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.DirectedCut(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        [TestMethod]
        public void IsSubmodularOverrideConnectedDetachment()
        {
            Action<int, int> action = (n, k) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path =  Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.ConnectedDetachment(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        [TestMethod]
        public void IsSubmodularOverrideFacilityLocation()
        {
            Action<int, int> action = (n, k) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path =  Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.FacilityLocation(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        [TestMethod]
        public void IsSubmodularOverrideGraphicMatroid()
        {
            Action<int, int> action = (n, k) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path =  Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.GraphicMatroid(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        [TestMethod]
        public void IsSubmodularOverrideBinaryMatrixRank()
        {
            Action<int, int> action = (n, k) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path =  Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.BinaryMatrixRank(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        [TestMethod]
        public void IsSubmodularOverrideNonPositiveSymmetricMatrixSummation()
        {
            Action<int, int> action = (n, k) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path =  Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.NonPositiveSymmetricMatrixSummation(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        [TestMethod]
        public void IsSubmodularOverrideSetCover()
        {
            Action<int, int> action = (n, k) =>
            {
                var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + methodName.Substring(numSubstring) + @"\" + n + "_" + k;
                var func = new Submodular.SetCover(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        [TestMethod]
        public void IsSubmodularOverrideSetCoverConcave()
        {
            Action<int, int> action = (n, k) =>
            {
                //var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + "SetCover" + @"\" + n + "_" + k;
                var func = new Submodular.SetCoverConcave(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        [TestMethod]
        public void IsSubmodularOverrideKyFan()
        {
            Action<int, int> action = (n, k) =>
            {
                //var methodName = MethodBase.GetCurrentMethod().Name.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = Const.DataPrefix + "KyFan" + @"\" + n + "_" + k;
                var func = new Submodular.KyFan(path);
                Assert.AreEqual(true, func.IsSubmodular());
            };
            CheckSubmodular(action);
        }

        private static void CheckSubmodular(Action<int, int> action)
        {
            for (int n = nMin; n < nMax; n++)
            {
                for (int k = kMin; k < kMax; k++)
                {
                    action(n, k);
                }//for k
            }//for n
        }
        
    }
}
