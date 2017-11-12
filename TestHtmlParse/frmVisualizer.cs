using System;
using System.Windows.Forms;

namespace TestHtmlMonkey
{
    public partial class frmVisualizer : Form
    {
        private HtmlMonkey.HtmlDocument Document;

        public frmVisualizer(HtmlMonkey.HtmlDocument document)
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

        private void nodeDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            htmlVisualizer1.ShowDetails();
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
