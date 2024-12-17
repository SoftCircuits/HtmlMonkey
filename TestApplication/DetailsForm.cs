// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.HtmlMonkey;
using System.Data;
using System.Diagnostics;

namespace TestApplication
{
    public partial class DetailsForm : Form
    {
        private readonly HtmlNode? Node;
        private readonly SoftCircuits.HtmlMonkey.HtmlDocument? Document;

        public DetailsForm(object node)
        {
            InitializeComponent();
            Debug.Assert(node != null);
            if (node is HtmlNode htmlNode)
                Node = htmlNode;
            else if (node is SoftCircuits.HtmlMonkey.HtmlDocument htmlDocument)
                Document = htmlDocument;
        }

        private void frmDetails_Load(object sender, EventArgs e)
        {
            ViewButton_CheckedChanged(btnOuterHtml, EventArgs.Empty);
        }

        private void ViewButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton button)
            {
                if (int.TryParse(button.Tag as string, out int value))
                {
                    if (Node != null)
                    {
                        if (value == 1)
                            txtText.Text = Node.OuterHtml;
                        else if (value == 2)
                            txtText.Text = Node.InnerHtml;
                        else if (value == 3)
                            txtText.Text = Node.Text;
                        else
                            txtText.Text = string.Empty;
                    }
                    else if (Document != null)
                    {
                        IEnumerable<string> values;
                        if (value == 1)
                            values = Document.RootNodes.Select(n => n.OuterHtml);
                        else if (value == 2)
                            values = Document.RootNodes.Select(n => n.InnerHtml);
                        else if (value == 3)
                            values = Document.RootNodes.Select(n => n.Text);
                        else
                            values = [];
                        txtText.Text = string.Join(string.Empty, values);
                    }
                }
            }
        }
    }
}
