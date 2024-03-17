namespace TestApplication
{
    partial class frmDetails
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnInnerText = new System.Windows.Forms.RadioButton();
            this.btnInnerHtml = new System.Windows.Forms.RadioButton();
            this.btnOuterHtml = new System.Windows.Forms.RadioButton();
            this.txtText = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnInnerText);
            this.groupBox1.Controls.Add(this.btnInnerHtml);
            this.groupBox1.Controls.Add(this.btnOuterHtml);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(720, 53);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "View:";
            // 
            // btnInnerText
            // 
            this.btnInnerText.AutoSize = true;
            this.btnInnerText.Location = new System.Drawing.Point(259, 19);
            this.btnInnerText.Name = "btnInnerText";
            this.btnInnerText.Size = new System.Drawing.Size(46, 17);
            this.btnInnerText.TabIndex = 2;
            this.btnInnerText.TabStop = true;
            this.btnInnerText.Tag = "3";
            this.btnInnerText.Text = "&Text";
            this.btnInnerText.UseVisualStyleBackColor = true;
            this.btnInnerText.Click += new System.EventHandler(this.ViewButton_CheckedChanged);
            // 
            // btnInnerHtml
            // 
            this.btnInnerHtml.AutoSize = true;
            this.btnInnerHtml.Location = new System.Drawing.Point(141, 19);
            this.btnInnerHtml.Name = "btnInnerHtml";
            this.btnInnerHtml.Size = new System.Drawing.Size(82, 17);
            this.btnInnerHtml.TabIndex = 1;
            this.btnInnerHtml.TabStop = true;
            this.btnInnerHtml.Tag = "2";
            this.btnInnerHtml.Text = "&Inner HTML";
            this.btnInnerHtml.UseVisualStyleBackColor = true;
            this.btnInnerHtml.CheckedChanged += new System.EventHandler(this.ViewButton_CheckedChanged);
            // 
            // btnOuterHtml
            // 
            this.btnOuterHtml.AutoSize = true;
            this.btnOuterHtml.Location = new System.Drawing.Point(19, 19);
            this.btnOuterHtml.Name = "btnOuterHtml";
            this.btnOuterHtml.Size = new System.Drawing.Size(84, 17);
            this.btnOuterHtml.TabIndex = 0;
            this.btnOuterHtml.TabStop = true;
            this.btnOuterHtml.Tag = "1";
            this.btnOuterHtml.Text = "&Outer HTML";
            this.btnOuterHtml.UseVisualStyleBackColor = true;
            this.btnOuterHtml.CheckedChanged += new System.EventHandler(this.ViewButton_CheckedChanged);
            // 
            // txtText
            // 
            this.txtText.Location = new System.Drawing.Point(12, 71);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.ReadOnly = true;
            this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtText.Size = new System.Drawing.Size(720, 498);
            this.txtText.TabIndex = 1;
            // 
            // frmDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 582);
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Node Details";
            this.Load += new System.EventHandler(this.frmDetails_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton btnInnerText;
        private System.Windows.Forms.RadioButton btnInnerHtml;
        private System.Windows.Forms.RadioButton btnOuterHtml;
        private System.Windows.Forms.TextBox txtText;
    }
}