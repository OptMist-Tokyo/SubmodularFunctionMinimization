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
    public partial class RangeForm : UserControl
    {

        public RangeForm()
        {
            InitializeComponent();
        }

        private void RangeForm_Load(object sender, EventArgs e)
        {

        }

       
        public void InitializeValues<T,K>(string title,T minValue,K maxValue)
        {
            this.groupBoxRange.Text = title;
            this.textBoxMin.Text = minValue.ToString();
            this.textBoxMax.Text = maxValue.ToString();
        }

        public string Min
        {
            get
            {
                return textBoxMin.Text;
            }
        }

        public string Max
        {
            get
            {
                return textBoxMax.Text;
            }
        }

        private void groupBoxRange_Enter(object sender, EventArgs e)
        {

        }



    }
}
