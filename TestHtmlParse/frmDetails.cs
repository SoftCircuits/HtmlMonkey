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

        public frmDetails(HtmlNode node)
        {
            InitializeComponent();
            Debug.Assert(node != null);
            Node = node;
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
                    if (value == 1)
                        txtText.Text = Node.ToString();
                    else if (value == 2)
                        txtText.Text = Node.Html;
                    else if (value == 3)
                        txtText.Text = Node.Text;
                    else
                        txtText.Text = string.Empty;
                }
            }
        }
    }
}
