using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SubmodularDataSet
{
    internal partial class DataCreatorForm : Form
    {
        public static DataCreatorForm mainForm;

        [STAThread]
        static void Main()
        {
            mainForm = new DataCreatorForm();
            mainForm.Initialize();
            Application.Run(mainForm);
        }

        public DataCreatorForm()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            InitializeDataCreator();
            foreach (var name in Enum.GetValues(typeof(Parameters.FunctionType)))
            {
                var type = mainForm.GetType();
                var method = type.GetMethod("Initialize" + name.ToString());
                method.Invoke(mainForm, new object[0] {});
            }//foreach name
        }


        private void InitializeDataCreator()
        {
            var type =mainForm.GetType();
            var components = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var currentComponent in components)
            {
                foreach (var name in Enum.GetValues(typeof(Parameters.FunctionType)))
                {
                    if (currentComponent.Name=="dataCreator"+name.ToString())
                    {
                        var currentInstance = (DataCreator)currentComponent.GetValue(this);
                        currentInstance.FunctionType = (int)name;
                    }//if
                }//foreach name
            }//foreach item            
        }

        private void MakeData(int n, bool first, string methodName,string path,DataCreator dataCreator,Action< string,CheckParseClass> action)
        {
            var prefix = path + "\\" + methodName;
            Directory.CreateDirectory(prefix);
            if (first)
            {
                dataCreator.ErrorMessage = "";
            }//if

            var data = new CheckParseClass(dataCreatorAll, dataCreator, methodName);
            action.Invoke(prefix,data);
            dataCreator.ErrorMessage += methodName + " " + n.ToString() + " has ended.\r\n";
            dataCreatorAll.ErrorMessage += methodName + " " + n.ToString() + " has ended.\r\n";
        }

        #region All

        public void InitializeAll()
        {
            foreach (var name in Enum.GetValues(typeof(Parameters.FunctionType)))
            {
                var cur = name.ToString();
                if (cur.ToString()=="All")
                {
                    continue;
                }//if
                this.checkedListBoxCandidate.Items.Add(cur);
                this.checkedListBoxCandidate.SetItemChecked(
                    checkedListBoxCandidate.Items.Count - 1, true);
            }//foreach name
        }

        public void MakeDataAll(string path,int n, int K,bool first)
        {
            var isDouble = dataCreatorAll.IsDouble;
            if (first)
            {
                dataCreatorAll.ErrorMessage = "";                
            }//if
            var type = mainForm.GetType();
            foreach (var item in checkedListBoxCandidate.CheckedItems)
            {
                var methodName = "MakeData" + item.ToString();
                var method = type.GetMethod(methodName);
                method.Invoke(mainForm, new object[] {path, n, K,first ,isDouble});
            }//foreach item
        }

        #endregion
        
        #region Modular

        public void InitializeModular()
        {
            rangeFormModularModular.InitializeValues("modular", Parameters.minIntValue, Parameters.maxIntValue);
        }

        public void MakeDataModular(string path, int n, int K,bool first,bool? _isDouble = null)
        {
            var isDouble = (_isDouble != null ? (bool)_isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataModularDouble(n, K) : MakeDataModularInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorModular, action);
        }

        private Action<string, CheckParseClass> MakeDataModularInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormModularModular, "modular", out modularMin, out modularMax)
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private Action<string, CheckParseClass> MakeDataModularDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormModularModular, "modular", out modularMin, out modularMax)
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }
        
        #endregion

        #region Undirected Cut

        public void InitializeUndirectedCut()
        {
            rangeFormUndirectedCutModular.InitializeValues("modular", Parameters.minIntValue, Parameters.maxIntValue);
            rangeFormUndirectedCutEdgeWeight.InitializeValues("edge weight", 0, Parameters.maxIntValue + " / (#n)");
        }

        public void MakeDataUndirectedCut(string path, int n, int K, bool first, bool? _isDouble = null)
        {
           var isDouble = (_isDouble != null ? (bool)_isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataUndirectedCutDouble(n, K) : MakeDataUndirectedCutInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorUndirectedCut, action);
        }

        private Action<string, CheckParseClass> MakeDataUndirectedCutInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                int edgeWeightMin; int edgeWeightMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormUndirectedCutModular, "modular", out modularMin, out modularMax)
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormUndirectedCutEdgeWeight, "edge weight", out edgeWeightMin, out edgeWeightMax)
                    || edgeWeightMin < 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var edgeWeight = Creator.MakeRandomSymmetricMatrix(n, edgeWeightMin, edgeWeightMax, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, edgeWeight);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private Action<string, CheckParseClass> MakeDataUndirectedCutDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                double edgeWeightMin; double edgeWeightMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormUndirectedCutModular, "modular", out modularMin, out modularMax)
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormUndirectedCutEdgeWeight, "edge weight", out edgeWeightMin, out edgeWeightMax)
                    || edgeWeightMin < 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var edgeWeight = Creator.MakeRandomSymmetricMatrix(n, edgeWeightMin, edgeWeightMax, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, edgeWeight);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        #endregion

        #region Directed Cut

        public void InitializeDirectedCut()
        {
            rangeFormDirectedCutModular.InitializeValues("modular", Parameters.minIntValue, Parameters.maxIntValue);
            rangeFormDirectedCutEdgeWeight.InitializeValues("edge weight", 0, Parameters.maxIntValue+" / (#n)");
        }

        public void MakeDataDirectedCut(string path, int n, int K, bool first, bool? _isDouble = null)
        {
            var isDouble = (_isDouble != null ? (bool)_isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataDirectedCutDouble(n, K) : MakeDataDirectedCutInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorDirectedCut, action);
        }

        private Action<string, CheckParseClass> MakeDataDirectedCutInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                int edgeWeightMin; int edgeWeightMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormDirectedCutModular, "modular", out modularMin, out modularMax)
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormDirectedCutEdgeWeight, "edge weight", out edgeWeightMin, out edgeWeightMax)
                    || edgeWeightMin < 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var edgeWeight = Creator.MakeRandomMatrix(n, edgeWeightMin, edgeWeightMax, rand, true);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, edgeWeight);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private Action<string, CheckParseClass> MakeDataDirectedCutDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                double edgeWeightMin; double edgeWeightMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormDirectedCutModular, "modular", out modularMin, out modularMax)
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormDirectedCutEdgeWeight, "edge weight", out edgeWeightMin, out edgeWeightMax)
                    || edgeWeightMin < 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var edgeWeight = Creator.MakeRandomMatrix(n, edgeWeightMin, edgeWeightMax, rand, true);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, edgeWeight);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        #endregion

        #region Connected Detachment

        public void InitializeConnectedDetachment()
        {
            rangeFormConnectedDetachmentModular.InitializeValues("modular", 0, "(#n+1)/2");
            valueFormConnectedDetachmentProbEdge.InitializeValues("probability of edge", "1/(#n)");
        }

        public void MakeDataConnectedDetachment(string path, int n, int K, bool first, bool? _isDouble = null)
        {
            var isDouble = (_isDouble != null ?(bool) _isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataConnectedDetachmentDouble(n, K) : MakeDataConnectedDetachmentInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorConnectedDetachment, action);
        }

        private Action<string, CheckParseClass> MakeDataConnectedDetachmentInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                double probOfEdge;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormConnectedDetachmentModular, "modular", out modularMin, out modularMax)
                           || !Checker.ParseProbAndCheck(n, data, valueFormConnectedDetachmentProbEdge, "probability of edge", out probOfEdge)
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    //var edges = Creator.MakeRandomSymmetricMatrixEdge(n, probOfEdge, rand);
                    var edges = Creator.MakeRandomConnectedGraph(n, probOfEdge, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, edges, true);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private Action<string, CheckParseClass> MakeDataConnectedDetachmentDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                double probOfEdge;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormConnectedDetachmentModular, "modular", out modularMin, out modularMax)
                           || !Checker.ParseProbAndCheck(n, data, valueFormConnectedDetachmentProbEdge, "probability of edge", out probOfEdge)
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    //var edges = Creator.MakeRandomSymmetricMatrixEdge(n, probOfEdge, rand);
                    var edges = Creator.MakeRandomConnectedGraph(n, probOfEdge, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, edges, true);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        #endregion

        #region Facility Location

        public void InitializeFacilityLocation()
        {
            rangeFormFacilityLocationModular.InitializeValues("modular", Parameters.minIntValue, Parameters.maxIntValue);
            rangeFormFacilityLocationMatrixElement.InitializeValues("matrix element", 0, Parameters.maxIntValue+" / (#n)");
        }

        public void MakeDataFacilityLocation(string path, int n, int K, bool first, bool? _isDouble = null)
        {
            var isDouble = (_isDouble != null ? (bool)_isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataFacilityLocationDouble(n, K) : MakeDataFacilityLocationInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorFacilityLocation, action);
        }

        private Action<string, CheckParseClass> MakeDataFacilityLocationInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                int matrixElementMin; int matrixElementMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormFacilityLocationModular, "modular", out modularMin, out modularMax)
                || !Checker.ParseAndCheckOverflow(n, data, rangeFormFacilityLocationMatrixElement, "matrix element", out matrixElementMin, out matrixElementMax)
                )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var matrix = Creator.MakeRandomMatrix(n, matrixElementMin, matrixElementMax, rand, false);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, matrix);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private Action<string, CheckParseClass> MakeDataFacilityLocationDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                double matrixElementMin; double matrixElementMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormFacilityLocationModular, "modular", out modularMin, out modularMax)
                || !Checker.ParseAndCheckOverflow(n, data, rangeFormFacilityLocationMatrixElement, "matrix element", out matrixElementMin, out matrixElementMax)
                )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var matrix = Creator.MakeRandomMatrix(n, matrixElementMin, matrixElementMax, rand, false);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, matrix);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }
        
        #endregion

        #region Graphic Matroid

        public void InitializeGraphicMatroid()
        {
            rangeFormGraphicMatroidModular.InitializeValues("modular", -2, 1);
            rangeFormGraphicMatroidNumberOfEdge.InitializeValues("number of vertices", "(#n)", "(#n)");
        }

        public void MakeDataGraphicMatroid(string path, int n, int K, bool first, bool? _isDouble = null)
        {
            var isDouble = (_isDouble != null ?(bool) _isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataGraphicMatroidDouble(n, K) : MakeDataGraphicMatroidInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorGraphicMatroid, action);
        }

        private Action<string, CheckParseClass> MakeDataGraphicMatroidInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                int numberOfVerticesMin; int numberOfVirticesMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormGraphicMatroidModular, "modular", out modularMin, out modularMax)
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormGraphicMatroidNumberOfEdge, "number of edge", out numberOfVerticesMin, out numberOfVirticesMax)
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var numberOfVirtices = Creator.MakeValue(numberOfVerticesMin, numberOfVirticesMax, rand);
                    var edges = Creator.MakeRandomGraphWithSomeEdges(n, numberOfVirtices, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    streamWriter.WriteLine(numberOfVirtices.ToString());
                    WriteMatrix(streamWriter, edges);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private Action<string, CheckParseClass> MakeDataGraphicMatroidDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                int numberOfVerticesMin; int numberOfVirticesMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormGraphicMatroidModular, "modular", out modularMin, out modularMax)
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormGraphicMatroidNumberOfEdge, "number of edge", out numberOfVerticesMin, out numberOfVirticesMax)
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var numberOfVirtices = Creator.MakeValue(numberOfVerticesMin, numberOfVirticesMax, rand);
                    var edges = Creator.MakeRandomGraphWithSomeEdges(n, numberOfVirtices, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    streamWriter.WriteLine(numberOfVirtices.ToString());
                    WriteMatrix(streamWriter, edges);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }


        #endregion

        #region Binary Matrix Rank

        public void InitializeBinaryMatrixRank()
        {
            rangeFormBinaryMatrixRankModular.InitializeValues("modular", -2, 1);
            valueFormBinaryMatrixRankProbElement.InitializeValues("probability of element", "(1/#n)");
            valueFormBinaryMatrixRankRowLen.InitializeValues("column length", "(#n)");
        }

        public void MakeDataBinaryMatrixRank(string path, int n, int K, bool first, bool? _isDouble = null)
        {
           var isDouble = (_isDouble != null ?(bool) _isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataBinaryMatrixRankDouble(n, K) : MakeDataBinaryMatrixRankInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorBinaryMatrixRank, action);
        }

        private Action<string, CheckParseClass> MakeDataBinaryMatrixRankInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                double probOfElement;
                int rowLength;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormBinaryMatrixRankModular, "modular", out modularMin, out modularMax)
                   || !Checker.ParseProbAndCheck(n, data, valueFormBinaryMatrixRankProbElement, "empty value", out probOfElement)
                    || !Checker.ParseAndCheckOverflow(n, data, valueFormBinaryMatrixRankRowLen, "number of edge", out rowLength)
                    || rowLength <= 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var binaryMatrix = Creator.MakeValueRondomBinaryMatrix(n, rowLength, probOfElement, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    streamWriter.WriteLine(rowLength.ToString());
                    WriteArray(streamWriter, binaryMatrix);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private Action<string, CheckParseClass> MakeDataBinaryMatrixRankDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                double probOfElement;
                int rowLength;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormBinaryMatrixRankModular, "modular", out modularMin, out modularMax)
                   || !Checker.ParseProbAndCheck(n, data, valueFormBinaryMatrixRankProbElement, "empty value", out probOfElement)
                    || !Checker.ParseAndCheckOverflow(n, data, valueFormBinaryMatrixRankRowLen, "number of edge", out rowLength)
                    || rowLength <= 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var binaryMatrix = Creator.MakeValueRondomBinaryMatrix(n, rowLength, probOfElement, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    streamWriter.WriteLine(rowLength.ToString());
                    WriteArray(streamWriter, binaryMatrix);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }


        #endregion

        #region NonPositive Symmetric Matrix Summation

        public void InitializeNonPositiveSymmetricMatrixSummation()
        {
            rangeFormNonPositiveSymmetricMatrixSummationModular.InitializeValues("modular", Parameters.minIntValue+" / (#n)", Parameters.maxIntValue);
            rangeFormNonPositiveSymmetricMatrixSummationMatrixElement.InitializeValues("matrix element", Parameters.minIntValue.ToString() + "/#n", 0);
        }

        public void MakeDataNonPositiveSymmetricMatrixSummation(string path, int n, int K, bool first, bool? _isDouble = null)
        {
            bool isDouble = (_isDouble != null ? (bool)_isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataNonPositiveSymmetricMatrixSummationDouble(n, K) : MakeDataNonPositiveSymmetricMatrixSummationInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorNonPositiveSymmetricMatrixSummation, action);
        }

        private Action<string, CheckParseClass> MakeDataNonPositiveSymmetricMatrixSummationInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                int matrixElementMin; int matrixElementMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormNonPositiveSymmetricMatrixSummationModular, "modular", out modularMin, out modularMax)
                   || !Checker.ParseAndCheckOverflow(n, data, rangeFormNonPositiveSymmetricMatrixSummationMatrixElement, "matrix element", out matrixElementMin, out  matrixElementMax)
                    || matrixElementMax > 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var matrixElement = Creator.MakeRandomSymmetricMatrix(n, matrixElementMin, matrixElementMax, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, matrixElement);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private Action<string, CheckParseClass> MakeDataNonPositiveSymmetricMatrixSummationDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                double matrixElementMin; double matrixElementMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormNonPositiveSymmetricMatrixSummationModular, "modular", out modularMin, out modularMax)
                   || !Checker.ParseAndCheckOverflow(n, data, rangeFormNonPositiveSymmetricMatrixSummationMatrixElement, "matrix element", out matrixElementMin, out  matrixElementMax)
                    || matrixElementMax > 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var matrixElement = Creator.MakeRandomSymmetricMatrix(n, matrixElementMin, matrixElementMax, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, matrixElement);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }


        #endregion

        #region SetCover

        public void InitializeSetCover()
        {
            rangeFormSetCoverModular.InitializeValues("modular", Parameters.minIntValue, 0);
            rangeFormSetCoverElementWeight.InitializeValues("weight of element", 0, Parameters.maxIntValue);
            rangeFormSetCoverElementWeightNumOfVertices.InitializeValues("the cardinality of vertices","#n","#n");
            valueFormSetCoverProbOfEdge.InitializeValues("probability of edges", "2/#n");
        }

        public void MakeDataSetCover(string path, int n, int K, bool first, bool? _isDouble = null)
        {
            var isDouble = (_isDouble != null ? (bool)_isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataSetCoverDouble(n, K) : MakeDataSetCoverInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorSetCover, action);
        }

        private Action<string, CheckParseClass> MakeDataSetCoverInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                int numOfVerticesMin; int numOfVerticesMax;
                int weightElementMin; int weightElementMax;
                double probOfEdge;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverModular, "modular", out modularMin, out modularMax)
                    ||modularMax>0
                   || !Checker.ParseProbAndCheck(n, data, valueFormSetCoverProbOfEdge, "probability of edge", out probOfEdge)
                   || !Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverElementWeightNumOfVertices, "the cardinality of vertices", out numOfVerticesMin, out numOfVerticesMax)
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverElementWeight, "weight of element", out weightElementMin, out weightElementMax)
                    || weightElementMin < 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var numOfVertices = Creator.MakeValue(numOfVerticesMin, numOfVerticesMax, rand);
                    var weightOfElement = Creator.MakeRandomArray(numOfVerticesMax, weightElementMin, weightElementMax, rand);
                    var edges = Creator.MakeRandomGraph(n, numOfVertices, probOfEdge, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    streamWriter.WriteLine(numOfVertices);
                    WriteArray(streamWriter, weightOfElement);
                    WriteMatrix(streamWriter, edges, true);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private Action<string, CheckParseClass> MakeDataSetCoverDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                int numOfVerticesMin; int numOfVerticesMax;
                double weightElementMin; double weightElementMax;
                double probOfEdge;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverModular, "modular", out modularMin, out modularMax)
                    ||modularMax>0
                   || !Checker.ParseProbAndCheck(n, data, valueFormSetCoverProbOfEdge, "probability of edge", out probOfEdge)
                   || !Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverElementWeightNumOfVertices, "the cardinality of vertices", out numOfVerticesMin, out numOfVerticesMax)
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverElementWeight, "weight of element", out weightElementMin, out weightElementMax)
                    || weightElementMin < 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var numOfVertices = Creator.MakeValue(numOfVerticesMin, numOfVerticesMax, rand);
                    var weightOfElement = Creator.MakeRandomArray(numOfVerticesMax, weightElementMin, weightElementMax, rand);
                    var edges = Creator.MakeRandomGraph(n, numOfVertices, probOfEdge, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    streamWriter.WriteLine(numOfVertices);
                    WriteArray(streamWriter, weightOfElement);
                    WriteMatrix(streamWriter, edges, true);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }


        #endregion

        #region SetCoverConcave

        public void InitializeSetCoverConcave()
        {
            rangeFormSetCoverConcaveModular.InitializeValues("modular", Parameters.minMinus, 0);
            rangeFormSetCoverConcaveElementWeight.InitializeValues("weight of element", 0, Parameters.minPlus);
            rangeFormSetCoverConcaveElementWeightNumOfVertices.InitializeValues("the cardinality of vertices", "#n", "#n");
            valueFormSetCoverConcaveProbOfEdge.InitializeValues("probability of edges", "2/#n");
        }

        public void MakeDataSetCoverConcave(string path, int n, int K, bool first, bool? _isDouble = null)
        {
            var isDouble = (_isDouble != null ? (bool)_isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataSetCoverConcaveDouble(n, K) : MakeDataSetCoverConcaveInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorSetCoverConcave, action);
        }

        private Action<string, CheckParseClass> MakeDataSetCoverConcaveInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                int numOfVerticesMin; int numOfVerticesMax;
                int weightElementMin; int weightElementMax;
                double probOfEdge;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverConcaveModular, "modular", out modularMin, out modularMax)
                    ||modularMax>0
                   || !Checker.ParseProbAndCheck(n, data, valueFormSetCoverConcaveProbOfEdge, "probability of edge", out probOfEdge)
                   || !Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverConcaveElementWeightNumOfVertices, "the cardinality of vertices", out numOfVerticesMin, out numOfVerticesMax)
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverConcaveElementWeight, "weight of element", out weightElementMin, out weightElementMax)
                    || weightElementMin < 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var numOfVertices = Creator.MakeValue(numOfVerticesMin, numOfVerticesMax, rand);
                    var weightOfElement = Creator.MakeRandomArray(numOfVerticesMax, weightElementMin, weightElementMax, rand);
                    var edges = Creator.MakeRandomGraph(n, numOfVertices, probOfEdge, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    streamWriter.WriteLine(numOfVertices);
                    WriteArray(streamWriter, weightOfElement);
                    WriteMatrix(streamWriter, edges, true);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private Action<string, CheckParseClass> MakeDataSetCoverConcaveDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                int numOfVerticesMin; int numOfVerticesMax;
                double weightElementMin; double weightElementMax;
                double probOfEdge;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverConcaveModular, "modular", out modularMin, out modularMax)
                    ||modularMax>0
                   || !Checker.ParseProbAndCheck(n, data, valueFormSetCoverConcaveProbOfEdge, "probability of edge", out probOfEdge)
                   || !Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverConcaveElementWeightNumOfVertices, "the cardinality of vertices", out numOfVerticesMin, out numOfVerticesMax)
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormSetCoverConcaveElementWeight, "weight of element", out weightElementMin, out weightElementMax)
                    || weightElementMin < 0
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var numOfVertices = Creator.MakeValue(numOfVerticesMin, numOfVerticesMax, rand);
                    var weightOfElement = Creator.MakeRandomArray(numOfVerticesMax, weightElementMin, weightElementMax, rand);
                    var edges = Creator.MakeRandomGraph(n, numOfVertices, probOfEdge, rand);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    streamWriter.WriteLine(numOfVertices);
                    WriteArray(streamWriter, weightOfElement);
                    WriteMatrix(streamWriter, edges, true);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }


        #endregion

        #region KyFan

        public void InitializeKyFan()
        {
            rangeFormKyFanModular.InitializeValues("modular", Parameters.minMinus, Parameters.minPlus);
            rangeFormKyFanEigen.InitializeValues("eigenvalue", 1, Parameters.minPlus);
            rangeFormKyFanElement.InitializeValues("the value of element", Parameters.minMinus, Parameters.minPlus);
        }

        public void MakeDataKyFan(string path, int n, int K, bool first, bool? _isDouble = null)
        {
            var isDouble = (_isDouble != null ? (bool)_isDouble : dataCreatorModular.IsDouble);
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.Substring(Parameters.removedLen);

            Action<string, CheckParseClass> action = (isDouble ? MakeDataKyFanDouble(n, K) : MakeDataKyFanInt(n, K));
            MakeData(n, first, methodName, path, dataCreatorKyFan, action);
        }

        private Action<string, CheckParseClass> MakeDataKyFanInt(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                int modularMin; int modularMax;
                int eigenvalueMin; int eigenvalueMax;
                int weightElementMin; int weightElementMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormKyFanModular, "modular", out modularMin, out modularMax)
                   || !Checker.ParseAndCheckOverflow(n, data, rangeFormKyFanEigen, "eigenvalue", out eigenvalueMin, out eigenvalueMax)||eigenvalueMin<=0
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormKyFanElement, "weight of element", out weightElementMin, out weightElementMax)
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var eigenvalue = Creator.MakeRandomArray(n,eigenvalueMin, eigenvalueMax, rand);
                    var triangle = Creator.MakeTriangleMatrix(n, weightElementMin, weightElementMax, rand);
                    var positiveDefinite = MakePositiveDefinite(n,eigenvalue, triangle);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, positiveDefinite, false);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private long[][] MakePositiveDefinite(int n,int[] eigenvalue, int[][] triangle)
        {
            for (int i = 0; i < n; i++)
            {
                triangle[i][i] = eigenvalue[i];
            }
            var res = new long[n][];
            for (int i = 0; i < n; i++)
            {
                res[i] = new long[n];
                for (int j = 0; j < n; j++)
                {
                    int min = Math.Min(i, j) + 1;
                    for (int k = 0; k < min; k++)
                    {
                        res[i][j] +=(long) triangle[i][k] * triangle[j][k];
                    }
                }
            }
            return res;
        }

        private Action<string, CheckParseClass> MakeDataKyFanDouble(int n, int K)
        {
            Action<string, CheckParseClass> action = (prefix, data) =>
            {
                double modularMin; double modularMax;
                double eigenvalueMin; double eigenvalueMax;
                double weightElementMin; double weightElementMax;
                if (!Checker.ParseAndCheckOverflow(n, data, rangeFormKyFanModular, "modular", out modularMin, out modularMax)
                   || !Checker.ParseAndCheckOverflow(n, data, rangeFormKyFanEigen, "eigenvalue", out eigenvalueMin, out eigenvalueMax) || eigenvalueMin <= 0
                    || !Checker.ParseAndCheckOverflow(n, data, rangeFormKyFanElement, "weight of element", out weightElementMin, out weightElementMax)
                    )
                {
                    return;
                }

                for (int k = 0; k < K; k++)
                {
                    var streamWriter = new StreamWriter(prefix + @"\" + n.ToString() + "_" + k.ToString());
                    var rand = GetRandomGenerator(n, k);
                    var modular = Creator.MakeRandomArray(n, modularMin, modularMax, rand);
                    var eigenvalue = Creator.MakeRandomArray(n, eigenvalueMin, eigenvalueMax, rand);
                    var triangle = Creator.MakeTriangleMatrix(n, weightElementMin, weightElementMax, rand);
                    var positiveDefinite = MakePositiveDefinite(n, eigenvalue, triangle);
                    streamWriter.WriteLine(n.ToString());
                    WriteArray(streamWriter, modular);
                    WriteMatrix(streamWriter, positiveDefinite, true);
                    streamWriter.Close();
                }//for k
            };
            return action;
        }

        private double[][] MakePositiveDefinite(int n, double[] eigenvalue, double[][] triangle)
        {
            for (int i = 0; i < n; i++)
            {
                triangle[i][i] = eigenvalue[i];
            }
            var res = new double[n][];
            for (int i = 0; i < n; i++)
            {
                res[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    int min = Math.Min(i, j) + 1;
                    for (int k = 0; k < min; k++)
                    {
                        res[i][j] += triangle[i][k] * triangle[j][k];
                    }
                }
            }
            return res;
        }


        #endregion
        
        private PseudoRandom.MersenneTwister GetRandomGenerator(int n, int k)
        {
            return new PseudoRandom.MersenneTwister((ulong)n * Const.Offset +(ulong) k);
        }

        private static void WriteMatrix<T>(StreamWriter streamWriter,IEnumerable<IEnumerable<T>> edges,bool withLength = false)
        {
            foreach (var array in edges)
            {
                var stringBuilder = new StringBuilder();
                if (withLength)
                {
                    stringBuilder.Append(array.Count() + " ");
                }//if
                int cnt = 0;
                foreach (var elem in array)
                {
                    if (cnt != 0)
                    {
                        stringBuilder.Append(" ");
                    }//if
                    stringBuilder.Append(elem);
                    cnt++;
                }//foreach elem
                streamWriter.WriteLine(stringBuilder.ToString());
            }//foreach m
        }

        //private static void WriteMatrix<T>(StreamWriter streamWriter, T[][] matrix)
        //{
        //    foreach (var array in matrix)
        //    {
        //        var stringBuilder = new StringBuilder();
        //        int cnt = 0;
        //        foreach (var elem in array)
        //        {
        //            if (cnt != 0)
        //            {
        //                stringBuilder.Append(" ");
        //            }//if
        //            stringBuilder.Append(elem);
        //            cnt++;
        //        }//foreach elem
        //        streamWriter.WriteLine(stringBuilder.ToString());
        //    }//foreach m
        //}

        private static void WriteArray<T>(StreamWriter streamWriter, T[] array)
        {
            foreach (var m in array)
            {
                streamWriter.WriteLine(m.ToString());
            }//foreach m
        }

        public class CheckParseClass
        {
            public DataCreator dataCreatorAll;
            public DataCreator dataCreator;
            public string name;

            public CheckParseClass(DataCreator dataCreatorAll, DataCreator dataCreator, string name)
            {
                this.dataCreatorAll = dataCreatorAll;
                this.dataCreator = dataCreator;
                this.name = name;
            }

        }

        private void DataCreator_Load(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click_1(object sender, EventArgs e)
        {

        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void parameterSetter1_Load(object sender, EventArgs e)
        {

        }

        private void tabPageUndirectedCut_Click(object sender, EventArgs e)
        {

        }

        private void dataCreatorUndirectedCut_Load(object sender, EventArgs e)
        {
            
        }

        private void rangeFormDirectedCutEmptyValue_Load(object sender, EventArgs e)
        {

        }

        private void rangeFormDirectedCutModular_Load(object sender, EventArgs e)
        {

        }

        private void tabPageBinaryMatrixRank_Click(object sender, EventArgs e)
        {

        }

        private void tabPageSetCover_Click(object sender, EventArgs e)
        {

        }

        private void tabPageModular_Click(object sender, EventArgs e)
        {

        }

        private void dataCreatorModular_Load(object sender, EventArgs e)
        {

        }

        private void dataCreatorAll_Load(object sender, EventArgs e)
        {

        }

        private void tabPageSetCovereConcave_Click(object sender, EventArgs e)
        {

        }

    }
}
