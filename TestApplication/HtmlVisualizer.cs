// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.HtmlMonkey;
using System.Diagnostics;

namespace TestApplication
{
    public partial class HtmlVisualizer : UserControl
    {
        private TreeNode? DisplayedNode = null;

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

        public void LoadDocument(SoftCircuits.HtmlMonkey.HtmlDocument document)
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
        public static void LoadNodes(HtmlNodeCollection nodes, TreeNode parent)
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

        private void TvwNodes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Restart timer
            // Because the code could fall behind trying to update the selected
            // node's details when the user changes the selection quickly, we
            // instead start a timer and only update the selected node's details
            // after the timer has been allowed to expire.
            timer1.Stop();
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            TreeNode? treeNode = tvwNodes.SelectedNode;
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
                DisplayedNode = treeNode;
            }
        }

        private static readonly Dictionary<Type, int> ImageIndexLookup = new()
        {
            [typeof(HtmlCDataNode)] = 5,
            [typeof(HtmlTextNode)] = 4,
            [typeof(HtmlElementNode)] = 3,
            [typeof(XmlHeaderNode)] = 2,
            [typeof(HtmlHeaderNode)] = 1,
            [typeof(SoftCircuits.HtmlMonkey.HtmlDocument)] = 0
        };

        public static int GetImageIndex(object node)
        {
            if (ImageIndexLookup.TryGetValue(node.GetType(), out int index))
                return index;
            return -1;
        }

        private void TvwNodes_MouseDown(object sender, MouseEventArgs e)
        {
            var info = tvwNodes.HitTest(e.Location);
            tvwNodes.SelectedNode = info.Node;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            ShowProperties();
        }

        public void ShowProperties()
        {
            TreeNode? treeNode = tvwNodes.SelectedNode;
            if (treeNode?.Tag is object node)
            {
                DetailsForm frm = new(node);
                frm.ShowDialog();
            }
        }

        public bool FindNext(string? text, bool matchCase)
        {
            if (string.IsNullOrEmpty(text) || tvwNodes.Nodes.Count == 0)
                return false;

            TextSearch search = new(text, matchCase);

            TreeNode? startNode = tvwNodes.SelectedNode;
            if (startNode == null)
                startNode = tvwNodes.Nodes[0];
            else
                startNode = startNode.NextVisibleNode;

            TreeNode? node = startNode;
            while (node != null)
            {
                Debug.Assert(node.Tag != null);
                if (MatchesNode(node.Tag, search))
                {
                    tvwNodes.SelectedNode = node;
                    return true;
                }
                // Get next node
                node = node.NextVisibleNode;
                // Wrap to start if needed
                node ??= tvwNodes.Nodes[0];
                // Check if we're back to starting node
                if (node == startNode)
                    break;
            }
            MessageBox.Show($"The text '{text}' was not found.", "Find");
            return false;
        }

        private static bool MatchesNode(object node, TextSearch search)
        {
            if (node is HtmlElementNode htmlElementNode)
            {
                return search.IsMatch(htmlElementNode.TagName) ||
                    MatchesAttributes(htmlElementNode.Attributes, search);
            }
            else if (node is HtmlCDataNode htmlCDataNode)
            {
                return search.IsMatch(htmlCDataNode.InnerHtml);
            }
            else if (node is HtmlTextNode htmlTextNode)
            {
                return search.IsMatch(htmlTextNode.Text);
            }
            else if (node is HtmlHeaderNode htmlHeaderNode)
            {
                return MatchesAttributes(htmlHeaderNode.Attributes, search);
            }
            else if (node is XmlHeaderNode xmlHeaderNode)
            {
                return MatchesAttributes(xmlHeaderNode.Attributes, search);
            }
            else if (node is SoftCircuits.HtmlMonkey.HtmlDocument)
            {
                // Root node
            }
            else Debug.Assert(false);
            return false;
        }

        private static bool MatchesAttributes(HtmlAttributeCollection attributes, TextSearch search)
        {
            foreach (var attribute in attributes)
            {
                if (search.IsMatch(attribute.Name) || search.IsMatch(attribute.Value))
                    return true;
            }
            return false;
        }
    }

    class TextSearch(string text, bool matchCase)
    {
        private readonly string Text = text;
        private readonly StringComparison StringComparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        public bool IsMatch(string? s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            return s.Contains(Text, StringComparison);
        }
    }
}
