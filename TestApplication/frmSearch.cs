// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
namespace TestApplication
{
    public partial class frmSearch : Form
    {
        public string? FindText { get; set; }
        public bool MatchCase { get; set; }

        public frmSearch()
        {
            InitializeComponent();
            EnableButtons();
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {
            txtFind.Text = FindText;
            chkMatchCase.Checked = MatchCase;
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void btnOk_Click(object sender, EventArgs e)
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
