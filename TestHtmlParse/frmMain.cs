using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlMonkey;
using TestHtmlParse;
using System.IO;
using Microsoft.VisualBasic;
using System.Net;

namespace HtmlMonkey
{
    public partial class Form1 : Form
    {
        protected string Url;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open HTML File";
            openFileDialog1.Filter = "HTML Files|*.html;*.htm|XML Files|*.xml|All Files|*.*";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtHtml.Text = File.ReadAllText(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = Interaction.InputBox("Url to download:", "Download HTML", Url);
            if (!string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    Url = url;
                    using (WebClient client = new WebClient())
                    {
                        txtHtml.Text = client.DownloadString(url);
                    }
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void parseHTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                HtmlMonkey.HtmlDocument document = HtmlDocument.FromHtml(txtHtml.Text);
                Cursor = Cursors.Default;

                frmVisualizer frm = new frmVisualizer(document);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                ex.ShowError();
            }


            ////document.RootNodes.OfType<HtmlElementNode>().Where(n => n.Attributes.Any());

            //var x = document.FindTags("a");
            ////var x = document.FindTags("a").Where(n => n.Attributes.ContainsKey("id"));
            ////var x = document.RootNodes.FindOfType<HtmlElementNode>(n => n.TagName.Equals("a", StringComparison.CurrentCultureIgnoreCase));
            //foreach (var z in x)
            //{
            //    HtmlAttribute href = z.Attributes["href"];
            //    if (z.Attributes.TryGetValue("no-exists", out HtmlAttribute xxx))
            //    {

            //    }
            //}

        }
    }
}
