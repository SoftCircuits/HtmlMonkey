// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using TestHtmlMonkey;

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

        private async void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = Interaction.InputBox("Url to download:", "Download HTML", Url);
            if (!string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    // Prevent other tasks during download
                    Enabled = false;

                    Url = url;
                    using (HttpClient client = new HttpClient())
                    {
                        txtHtml.Text = await client.GetStringAsync(url);
                    }
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }
                finally
                {
                    Enabled = true;
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
                SoftCircuits.HtmlMonkey.HtmlDocument document = SoftCircuits.HtmlMonkey.HtmlDocument.FromHtml(txtHtml.Text);
                Cursor = Cursors.Default;

                frmVisualizer frm = new frmVisualizer(document);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                ex.ShowError();
            }
        }
    }
}
