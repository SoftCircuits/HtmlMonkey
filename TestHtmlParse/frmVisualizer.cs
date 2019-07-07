using SoftCircuits.HtmlMonkey;
using System;
using System.Windows.Forms;

namespace TestHtmlMonkey
{
    public partial class frmVisualizer : Form
    {
        private HtmlMonkeyDocument Document;

        public frmVisualizer(HtmlMonkeyDocument document)
        {
            InitializeComponent();
            Document = document;
        }

        private void frmVisualizer_Load(object sender, EventArgs e)
        {
            htmlVisualizer1.LoadDocument(Document);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            htmlVisualizer1.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            htmlVisualizer1.CollapseAll();
        }
    }
}
