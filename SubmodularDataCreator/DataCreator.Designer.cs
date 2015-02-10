namespace SubmodularDataSet
{
    partial class DataCreator
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
            this.buttonExecution = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.groupBoxPath = new System.Windows.Forms.GroupBox();
            this.buttonFolderBrowserDialog = new System.Windows.Forms.Button();
            this.radioButtonInt = new System.Windows.Forms.RadioButton();
            this.groupBoxPathType = new System.Windows.Forms.GroupBox();
            this.radioButtonDouble = new System.Windows.Forms.RadioButton();
            this.groupBoxNumber = new System.Windows.Forms.GroupBox();
            this.numericUpDownNMax = new System.Windows.Forms.NumericUpDown();
            this.radioButtonRange = new System.Windows.Forms.RadioButton();
            this.labelK = new System.Windows.Forms.Label();
            this.radioButtonPow = new System.Windows.Forms.RadioButton();
            this.labelN = new System.Windows.Forms.Label();
            this.numericUpDownK = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownNMin = new System.Windows.Forms.NumericUpDown();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.groupBoxPath.SuspendLayout();
            this.groupBoxPathType.SuspendLayout();
            this.groupBoxNumber.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNMin)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonExecution
            // 
            this.buttonExecution.Location = new System.Drawing.Point(15, 176);
            this.buttonExecution.Name = "buttonExecution";
            this.buttonExecution.Size = new System.Drawing.Size(127, 29);
            this.buttonExecution.TabIndex = 0;
            this.buttonExecution.Text = "Execute and Save";
            this.buttonExecution.UseVisualStyleBackColor = true;
            this.buttonExecution.Click += new System.EventHandler(this.button_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(167, 176);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(80, 29);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(6, 18);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(226, 19);
            this.textBoxPath.TabIndex = 0;
            this.textBoxPath.Text = "C:\\Submodular";
            this.textBoxPath.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // groupBoxPath
            // 
            this.groupBoxPath.Controls.Add(this.buttonFolderBrowserDialog);
            this.groupBoxPath.Controls.Add(this.textBoxPath);
            this.groupBoxPath.Location = new System.Drawing.Point(15, 15);
            this.groupBoxPath.Name = "groupBoxPath";
            this.groupBoxPath.Size = new System.Drawing.Size(282, 47);
            this.groupBoxPath.TabIndex = 3;
            this.groupBoxPath.TabStop = false;
            this.groupBoxPath.Text = "Path";
            this.groupBoxPath.Enter += new System.EventHandler(this.groupBoxPath_Enter);
            // 
            // buttonFolderBrowserDialog
            // 
            this.buttonFolderBrowserDialog.Location = new System.Drawing.Point(238, 14);
            this.buttonFolderBrowserDialog.Name = "buttonFolderBrowserDialog";
            this.buttonFolderBrowserDialog.Size = new System.Drawing.Size(38, 23);
            this.buttonFolderBrowserDialog.TabIndex = 3;
            this.buttonFolderBrowserDialog.Text = "ref.";
            this.buttonFolderBrowserDialog.UseVisualStyleBackColor = true;
            this.buttonFolderBrowserDialog.Click += new System.EventHandler(this.buttonFolderBrowserDialog_Click);
            // 
            // radioButtonInt
            // 
            this.radioButtonInt.AutoSize = true;
            this.radioButtonInt.Checked = true;
            this.radioButtonInt.Location = new System.Drawing.Point(16, 21);
            this.radioButtonInt.Name = "radioButtonInt";
            this.radioButtonInt.Size = new System.Drawing.Size(58, 16);
            this.radioButtonInt.TabIndex = 4;
            this.radioButtonInt.TabStop = true;
            this.radioButtonInt.Text = "integer";
            this.radioButtonInt.UseVisualStyleBackColor = true;
            // 
            // groupBoxPathType
            // 
            this.groupBoxPathType.Controls.Add(this.radioButtonDouble);
            this.groupBoxPathType.Controls.Add(this.radioButtonInt);
            this.groupBoxPathType.Location = new System.Drawing.Point(207, 68);
            this.groupBoxPathType.Name = "groupBoxPathType";
            this.groupBoxPathType.Size = new System.Drawing.Size(84, 71);
            this.groupBoxPathType.TabIndex = 4;
            this.groupBoxPathType.TabStop = false;
            this.groupBoxPathType.Text = "Type";
            // 
            // radioButtonDouble
            // 
            this.radioButtonDouble.AutoSize = true;
            this.radioButtonDouble.Location = new System.Drawing.Point(16, 43);
            this.radioButtonDouble.Name = "radioButtonDouble";
            this.radioButtonDouble.Size = new System.Drawing.Size(42, 16);
            this.radioButtonDouble.TabIndex = 5;
            this.radioButtonDouble.Text = "real";
            this.radioButtonDouble.UseVisualStyleBackColor = true;
            this.radioButtonDouble.CheckedChanged += new System.EventHandler(this.radioButtonDouble_CheckedChanged);
            // 
            // groupBoxNumber
            // 
            this.groupBoxNumber.Controls.Add(this.numericUpDownNMax);
            this.groupBoxNumber.Controls.Add(this.radioButtonRange);
            this.groupBoxNumber.Controls.Add(this.labelK);
            this.groupBoxNumber.Controls.Add(this.radioButtonPow);
            this.groupBoxNumber.Controls.Add(this.labelN);
            this.groupBoxNumber.Controls.Add(this.numericUpDownK);
            this.groupBoxNumber.Controls.Add(this.numericUpDownNMin);
            this.groupBoxNumber.Location = new System.Drawing.Point(15, 68);
            this.groupBoxNumber.Name = "groupBoxNumber";
            this.groupBoxNumber.Size = new System.Drawing.Size(186, 102);
            this.groupBoxNumber.TabIndex = 6;
            this.groupBoxNumber.TabStop = false;
            this.groupBoxNumber.Text = "Number";
            // 
            // numericUpDownNMax
            // 
            this.numericUpDownNMax.Location = new System.Drawing.Point(108, 22);
            this.numericUpDownNMax.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownNMax.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownNMax.Name = "numericUpDownNMax";
            this.numericUpDownNMax.Size = new System.Drawing.Size(72, 19);
            this.numericUpDownNMax.TabIndex = 6;
            this.numericUpDownNMax.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // radioButtonRange
            // 
            this.radioButtonRange.AutoSize = true;
            this.radioButtonRange.Location = new System.Drawing.Point(76, 72);
            this.radioButtonRange.Name = "radioButtonRange";
            this.radioButtonRange.Size = new System.Drawing.Size(51, 16);
            this.radioButtonRange.TabIndex = 5;
            this.radioButtonRange.Text = "range";
            this.radioButtonRange.UseVisualStyleBackColor = true;
            // 
            // labelK
            // 
            this.labelK.AutoSize = true;
            this.labelK.Location = new System.Drawing.Point(11, 49);
            this.labelK.Name = "labelK";
            this.labelK.Size = new System.Drawing.Size(24, 12);
            this.labelK.TabIndex = 5;
            this.labelK.Text = "K :=";
            // 
            // radioButtonPow
            // 
            this.radioButtonPow.AutoSize = true;
            this.radioButtonPow.Checked = true;
            this.radioButtonPow.Location = new System.Drawing.Point(22, 72);
            this.radioButtonPow.Name = "radioButtonPow";
            this.radioButtonPow.Size = new System.Drawing.Size(53, 16);
            this.radioButtonPow.TabIndex = 4;
            this.radioButtonPow.TabStop = true;
            this.radioButtonPow.Text = "power";
            this.radioButtonPow.UseVisualStyleBackColor = true;
            // 
            // labelN
            // 
            this.labelN.AutoSize = true;
            this.labelN.Location = new System.Drawing.Point(10, 24);
            this.labelN.Name = "labelN";
            this.labelN.Size = new System.Drawing.Size(25, 12);
            this.labelN.TabIndex = 4;
            this.labelN.Text = "N :=";
            // 
            // numericUpDownK
            // 
            this.numericUpDownK.Location = new System.Drawing.Point(64, 47);
            this.numericUpDownK.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownK.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownK.Name = "numericUpDownK";
            this.numericUpDownK.Size = new System.Drawing.Size(63, 19);
            this.numericUpDownK.TabIndex = 2;
            this.numericUpDownK.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // numericUpDownNMin
            // 
            this.numericUpDownNMin.Location = new System.Drawing.Point(40, 22);
            this.numericUpDownNMin.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownNMin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownNMin.Name = "numericUpDownNMin";
            this.numericUpDownNMin.Size = new System.Drawing.Size(62, 19);
            this.numericUpDownNMin.TabIndex = 1;
            this.numericUpDownNMin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.HideSelection = false;
            this.textBoxMessage.Location = new System.Drawing.Point(15, 211);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(276, 87);
            this.textBoxMessage.TabIndex = 7;
            // 
            // DataCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.groupBoxNumber);
            this.Controls.Add(this.groupBoxPathType);
            this.Controls.Add(this.groupBoxPath);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonExecution);
            this.Name = "DataCreator";
            this.Size = new System.Drawing.Size(300, 310);
            this.Load += new System.EventHandler(this.DataCreator_Load);
            this.groupBoxPath.ResumeLayout(false);
            this.groupBoxPath.PerformLayout();
            this.groupBoxPathType.ResumeLayout(false);
            this.groupBoxPathType.PerformLayout();
            this.groupBoxNumber.ResumeLayout(false);
            this.groupBoxNumber.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNMin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonExecution;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.GroupBox groupBoxPath;
        private System.Windows.Forms.RadioButton radioButtonInt;
        private System.Windows.Forms.GroupBox groupBoxPathType;
        private System.Windows.Forms.RadioButton radioButtonDouble;
        private System.Windows.Forms.Button buttonFolderBrowserDialog;
        private System.Windows.Forms.GroupBox groupBoxNumber;
        private System.Windows.Forms.Label labelK;
        private System.Windows.Forms.Label labelN;
        private System.Windows.Forms.NumericUpDown numericUpDownK;
        private System.Windows.Forms.NumericUpDown numericUpDownNMin;
        private System.Windows.Forms.RadioButton radioButtonRange;
        private System.Windows.Forms.RadioButton radioButtonPow;
        private System.Windows.Forms.NumericUpDown numericUpDownNMax;
        private System.Windows.Forms.TextBox textBoxMessage;
    }
}
