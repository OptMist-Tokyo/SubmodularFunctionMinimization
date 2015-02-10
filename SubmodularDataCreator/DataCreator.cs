using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubmodularDataSet
{
    public partial class DataCreator : UserControl
    {
        public DataCreator()
        {
            InitializeComponent();
            this.buttonExecution.Text = "Execute and Save";
        }

        public int FunctionType
        {
            set;
            get;
        }

        public bool LoopOfN
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get
            {
                return textBoxMessage.Text;
            }
            set
            {
                textBoxMessage.Text = "";
                textBoxMessage.AppendText(value);
            }
        }

        public bool IsDouble
        {
            get { return radioButtonDouble.Checked; }
        }

        public bool IsInt
        {
            get { return radioButtonInt.Checked; }
        }

        
        private void button_Click(object sender, EventArgs e)
        {
            string path = textBoxPath.Text;
            int nMin = (int)numericUpDownNMin.Value;
            int nMax = (int)numericUpDownNMax.Value;
            int K = (int)numericUpDownK.Value;
            var range = (radioButtonPow.Checked ? enumPow(nMin, nMax) : enumRange(nMin, nMax));
            LoopOfN = true;
            bool first = true;
            foreach (var i in range)
            {
                if (!LoopOfN)
                {
                    break;
                }//if
                MakeData(path,i, K, FunctionType,first);
                first = false;
            }//foreach i
        }

        private void MakeData(string path,int n, int K, int FunctionType,bool first)
        {
            foreach (var name in Enum.GetValues(typeof(Parameters.FunctionType)))
            {
                if (FunctionType==(int)name)
                {
                    var type = Type.GetType("SubmodularDataSet.DataCreatorForm");
                    var method = type.GetMethod("MakeData" + name.ToString());
                    if (name.ToString() == "All")
                    {
                        method.Invoke(SubmodularDataSet.DataCreatorForm.mainForm,
                           new object[] { path, n, K, first });
                    }
                    else
                    {
                        method.Invoke(SubmodularDataSet.DataCreatorForm.mainForm,
                            new object[] { path, n, K, first, null });
                    }
                }//if
            }//foreach name
        }

        private IEnumerable<int> enumRange(int min,int max)
        {
            for (int i = min; i <= max; i++)
            {
                yield return i;
            }//for i
        }

        private IEnumerable<int> enumPow(int min, int max)
        {
            int cur = min;
            while (cur<=max)
            {
                yield return (int)cur;
                cur *= Parameters.power;
            }//while
        }

        private void DataCreator_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void buttonFolderBrowserDialog_Click(object sender, EventArgs e)
        {
            var folderBrowseerDialogPath = new FolderBrowserDialog();
            folderBrowseerDialogPath.Description = "Please select a path where you make files";
            if (folderBrowseerDialogPath.ShowDialog()==DialogResult.OK)
            {
                textBoxPath.Text = folderBrowseerDialogPath.SelectedPath;
            }//if
        }

        private void groupBoxPath_Enter(object sender, EventArgs e)
        {

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {

        }

        private void radioButtonDouble_CheckedChanged(object sender, EventArgs e)
        {

        }

        
    }
}
