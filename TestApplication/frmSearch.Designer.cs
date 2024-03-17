namespace TestApplication
{
    partial class frmSearch
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
            label1 = new Label();
            txtFind = new TextBox();
            btnOk = new Button();
            btnCancel = new Button();
            chkMatchCase = new CheckBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 32);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(40, 20);
            label1.TabIndex = 0;
            label1.Text = "&Find:";
            // 
            // txtFind
            // 
            txtFind.Location = new Point(16, 57);
            txtFind.Margin = new Padding(4, 5, 4, 5);
            txtFind.Name = "txtFind";
            txtFind.Size = new Size(429, 27);
            txtFind.TabIndex = 1;
            txtFind.TextChanged += txtFind_TextChanged;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(127, 169);
            btnOk.Margin = new Padding(4, 5, 4, 5);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(100, 35);
            btnOk.TabIndex = 3;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(235, 169);
            btnCancel.Margin = new Padding(4, 5, 4, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkMatchCase
            // 
            chkMatchCase.AutoSize = true;
            chkMatchCase.Location = new Point(20, 112);
            chkMatchCase.Margin = new Padding(4, 5, 4, 5);
            chkMatchCase.Name = "chkMatchCase";
            chkMatchCase.Size = new Size(105, 24);
            chkMatchCase.TabIndex = 2;
            chkMatchCase.Text = "&Match case";
            chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // frmSearch
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(463, 238);
            Controls.Add(chkMatchCase);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(txtFind);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmSearch";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Search";
            Load += frmSearch_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkMatchCase;
    }
}