namespace SubmodularDataSet
{
    partial class RangeForm
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxRange = new System.Windows.Forms.GroupBox();
            this.textBoxMax = new System.Windows.Forms.TextBox();
            this.textBoxMin = new System.Windows.Forms.TextBox();
            this.labelRange = new System.Windows.Forms.Label();
            this.groupBoxRange.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxRange
            // 
            this.groupBoxRange.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRange.Controls.Add(this.textBoxMax);
            this.groupBoxRange.Controls.Add(this.textBoxMin);
            this.groupBoxRange.Controls.Add(this.labelRange);
            this.groupBoxRange.Location = new System.Drawing.Point(3, 3);
            this.groupBoxRange.Name = "groupBoxRange";
            this.groupBoxRange.Size = new System.Drawing.Size(203, 44);
            this.groupBoxRange.TabIndex = 0;
            this.groupBoxRange.TabStop = false;
            this.groupBoxRange.Text = "groupBox1";
            this.groupBoxRange.Enter += new System.EventHandler(this.groupBoxRange_Enter);
            // 
            // textBoxMax
            // 
            this.textBoxMax.Location = new System.Drawing.Point(110, 18);
            this.textBoxMax.Name = "textBoxMax";
            this.textBoxMax.Size = new System.Drawing.Size(82, 19);
            this.textBoxMax.TabIndex = 0;
            this.textBoxMax.Text = "0";
            // 
            // textBoxMin
            // 
            this.textBoxMin.Location = new System.Drawing.Point(6, 18);
            this.textBoxMin.Name = "textBoxMin";
            this.textBoxMin.Size = new System.Drawing.Size(82, 19);
            this.textBoxMin.TabIndex = 0;
            this.textBoxMin.Text = "0";
            // 
            // labelRange
            // 
            this.labelRange.AutoSize = true;
            this.labelRange.Location = new System.Drawing.Point(94, 25);
            this.labelRange.Name = "labelRange";
            this.labelRange.Size = new System.Drawing.Size(10, 12);
            this.labelRange.TabIndex = 1;
            this.labelRange.Text = "~";
            // 
            // RangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxRange);
            this.Name = "RangeForm";
            this.Size = new System.Drawing.Size(210, 54);
            this.Load += new System.EventHandler(this.RangeForm_Load);
            this.groupBoxRange.ResumeLayout(false);
            this.groupBoxRange.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxRange;
        private System.Windows.Forms.Label labelRange;
        private System.Windows.Forms.TextBox textBoxMax;
        private System.Windows.Forms.TextBox textBoxMin;
    }
}
