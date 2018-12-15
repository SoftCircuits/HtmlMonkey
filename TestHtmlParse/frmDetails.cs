using HtmlMonkey;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestHtmlMonkey
{
    public partial class frmDetails : Form
    {
        private HtmlNode Node;
        private HtmlMonkey.HtmlDocument Document;

        public frmDetails(object node)
        {
            InitializeComponent();
            Debug.Assert(node != null);
            if (node is HtmlNode)
                Node = node as HtmlNode;
            else if (node is HtmlMonkey.HtmlDocument)
                Document = node as HtmlMonkey.HtmlDocument;
        }

        private void frmDetails_Load(object sender, EventArgs e)
        {
            ViewButton_CheckedChanged(btnOuterHtml, EventArgs.Empty);
        }

        private void ViewButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button != null)
            {
                if (int.TryParse(button.Tag as string, out int value))
                {
                    if (Node != null)
                    {
                        if (value == 1)
                            txtText.Text = Node.OuterHtml;
                        else if (value == 2)
                            txtText.Text = Node.Html;
                        else if (value == 3)
                            txtText.Text = Node.Text;
                        else
                            txtText.Text = string.Empty;
                    }
                    else if (Document != null)
                    {
                        IEnumerable<string> values;
                        if (value == 1)
                            values = Document.RootNodes.Select(n => n.ToString());
                        else if (value == 2)
                            values = Document.RootNodes.Select(n => n.Html);
                        else if (value == 3)
                            values = Document.RootNodes.Select(n => n.Text);
                        else
                            values = Enumerable.Empty<string>();
                        txtText.Text = string.Join(string.Empty, values);
                    }
                }
            }
        }
    }
}
