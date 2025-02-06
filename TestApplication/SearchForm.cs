using System.ComponentModel;
// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace TestApplication
{
    public partial class SearchForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? FindText { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MatchCase { get; set; }

        public SearchForm()
        {
            InitializeComponent();
            EnableButtons();
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
            txtFind.Text = FindText;
            chkMatchCase.Checked = MatchCase;
        }

        private void TxtFind_TextChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            FindText = txtFind.Text;
            MatchCase = chkMatchCase.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void EnableButtons()
        {
            btnOk.Enabled = txtFind.Text.Length > 0;
        }
    }
}
