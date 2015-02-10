namespace SubmodularDataSet
{
    partial class ValueForm
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
            this.groupBoxValue = new System.Windows.Forms.GroupBox();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.groupBoxValue.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxValue
            // 
            this.groupBoxValue.Controls.Add(this.textBoxValue);
            this.groupBoxValue.Location = new System.Drawing.Point(3, 3);
            this.groupBoxValue.Name = "groupBoxValue";
            this.groupBoxValue.Size = new System.Drawing.Size(152, 47);
            this.groupBoxValue.TabIndex = 1;
            this.groupBoxValue.TabStop = false;
            this.groupBoxValue.Text = "groupBox1";
            this.groupBoxValue.Enter += new System.EventHandler(this.groupBoxValue_Enter);
            // 
            // textBoxValue
            // 
            this.textBoxValue.Location = new System.Drawing.Point(6, 18);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(140, 19);
            this.textBoxValue.TabIndex = 0;
            this.textBoxValue.Text = "1/#n";
            // 
            // ValueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxValue);
            this.Name = "ValueForm";
            this.Size = new System.Drawing.Size(158, 54);
            this.Load += new System.EventHandler(this.ValueForm_Load);
            this.groupBoxValue.ResumeLayout(false);
            this.groupBoxValue.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxValue;
        private System.Windows.Forms.TextBox textBoxValue;
    }
}
