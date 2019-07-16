// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.HtmlMonkey;
using System;
using System.Collections.Generic;
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

        public void ExpandAll()
        {
            tvwNodes.ExpandAll();
        }

        public void CollapseAll()
        {
            tvwNodes.CollapseAll();
        }

        public void LoadDocument(HtmlMonkeyDocument document)
        {
            tvwNodes.Nodes.Clear();
            TreeNode treeNode = tvwNodes.Nodes.Add("Document");
            treeNode.Tag = document;
            treeNode.ImageIndex = treeNode.SelectedImageIndex = GetImageIndex(document);
            LoadNodes(document.RootNodes, treeNode);
            tvwNodes.ExpandAll();
            treeNode.EnsureVisible();
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
                if (treeNode?.Tag is object node)
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

        private static Dictionary<Type, int> ImageIndexLookup = new Dictionary<Type, int>
        {
            [typeof(HtmlCDataNode)] = 5,
            [typeof(HtmlTextNode)] = 4,
            [typeof(HtmlElementNode)] = 3,
            [typeof(XmlHeaderNode)] = 2,
            [typeof(HtmlHeaderNode)] = 1,
            [typeof(HtmlMonkeyDocument)] = 0
        };

        public int GetImageIndex(object node)
        {
            if (ImageIndexLookup.TryGetValue(node.GetType(), out int index))
                return index;
            return -1;
        }

        private void tvwNodes_MouseDown(object sender, MouseEventArgs e)
        {
            var info = tvwNodes.HitTest(e.Location);
            tvwNodes.SelectedNode = info.Node;
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode treeNode = tvwNodes.SelectedNode;
            if (treeNode.Tag is object node)
            {
                frmDetails frm = new frmDetails(node);
                frm.ShowDialog();
            }
        }
    }
}
