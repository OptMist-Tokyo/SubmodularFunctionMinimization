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
    public partial class ValueForm : UserControl
    {
        public ValueForm()
        {
            InitializeComponent();
        }

        private void ValueForm_Load(object sender, EventArgs e)
        {

        }

        public void InitializeValues<T>(string tittle, T value)
        {
            groupBoxValue.Text = tittle;
            textBoxValue.Text = value.ToString();
        }

        public string Value
        {
            get { return textBoxValue.Text; }
        }

        private void groupBoxValue_Enter(object sender, EventArgs e)
        {

        }

    }
}
