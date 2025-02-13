﻿// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
namespace TestApplication
{
    public partial class VisualizerForm : Form
    {
        private readonly SoftCircuits.HtmlMonkey.HtmlDocument Document;
        private string? FindText;
        private bool MatchCase;

        public VisualizerForm(SoftCircuits.HtmlMonkey.HtmlDocument document)
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

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm frm = new()
            {
                FindText = FindText,
                MatchCase = MatchCase
            };
            if (frm.ShowDialog() == DialogResult.OK)
            {
                FindText = frm.FindText;
                MatchCase = frm.MatchCase;
                htmlVisualizer1.FindNext(FindText, MatchCase);
            }
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FindText))
                htmlVisualizer1.FindNext(FindText, MatchCase);
            else
                findToolStripMenuItem_Click(this, e);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            htmlVisualizer1.ShowProperties();
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
