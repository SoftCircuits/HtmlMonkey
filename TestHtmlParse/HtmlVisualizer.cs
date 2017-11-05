using HtmlMonkey;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TestHtmlParse
{
    public partial class HtmlVisualizer : UserControl
    {
        private TreeNode DisplayedNode = null;

        public HtmlVisualizer()
        {
            InitializeComponent();
        }

        public void ShowDetails()
        {
            TreeNode treeNode = tvwNodes.SelectedNode;
            if (treeNode.Tag is HtmlNode node)
            {
                frmDetails frm = new frmDetails(node);
                frm.ShowDialog();
            }
        }

        public void ExpandAll()
        {
            tvwNodes.ExpandAll();
        }

        public void CollapseAll()
        {
            tvwNodes.CollapseAll();
        }

        public void LoadDocument(HtmlMonkey.HtmlDocument document)
        {
            tvwNodes.Nodes.Clear();
            TreeNode root = tvwNodes.Nodes.Add($"[HtmlDocument]");
            LoadNodes(document.RootNodes, root);
            tvwNodes.ExpandAll();
            root.EnsureVisible();
        }

        /// <summary>
        /// Populates TreeView control with nodes
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="parent"></param>
        public void LoadNodes(HtmlNodeCollection nodes, TreeNode parent)
        {
            // Populate 
            foreach (HtmlNode node in nodes)
            {
                Visualizer visualizer = NodeVisualizer.GetVisualizer(node);
                Debug.Assert(visualizer != null);
                TreeNode treeNode = parent.Nodes.Add(visualizer.ShortDescription(node));
                treeNode.Tag = node;
                if (node is HtmlElementNode elementNode)
                    LoadNodes(elementNode.Children, treeNode);
            }
        }

        private void tvwNodes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Restart timer
            // Because the code could fall behind trying to update the selected
            // node's details when the user changes the selection quickly, we
            // instead start a timer and only update the selected node's details
            // after the timer has been allowed to expire.
            timer1.Stop();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            TreeNode treeNode = tvwNodes.SelectedNode;
            if (DisplayedNode != treeNode)
            {
                if (treeNode?.Tag is HtmlNode node)
                {
                    Visualizer visualizer = NodeVisualizer.GetVisualizer(node);
                    Debug.Assert(visualizer != null);
                    lblType.Text = node.GetType().Name;
                    visualizer.PopulateProperties(node, lvwProperties);
                    txtText.Text = visualizer.LongDescription(node);
                }
                else
                {
                    lblType.Text = string.Empty;
                    lvwProperties.Columns.Clear();
                    lvwProperties.Items.Clear();
                    txtText.Text = string.Empty;
                }
            }
        }
    }
}
