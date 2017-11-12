using HtmlMonkey;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TestHtmlMonkey
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
            TreeNode root = tvwNodes.Nodes.Add("[HtmlDocument]");
            root.ImageIndex = root.SelectedImageIndex = 0;
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
                treeNode.ImageIndex = treeNode.SelectedImageIndex = GetImageIndex(node);
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

        public int GetImageIndex(HtmlNode node)
        {
            Type type = node.GetType();
            if (type == typeof(HtmlCDataNode))
                return 5;
            if (type == typeof(HtmlTextNode))
                return 4;
            if (type == typeof(HtmlElementNode))
                return 3;
            if (type == typeof(XmlHeaderNode))
                return 2;
            if (type == typeof(HtmlHeaderNode))
                return 1;
            return -1;
        }
    }
}
