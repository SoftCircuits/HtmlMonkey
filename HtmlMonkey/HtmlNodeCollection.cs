/////////////////////////////////////////////////////////////
// HTML Monkey
// Copyright (c) 2018 Jonathan Wood
// http://www.softcircuits.com, http://www.blackbeltcoder.com
//
using System.Collections.Generic;
using System.Diagnostics;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Represents a IEnumerable collection of nodes. Includes methods
    /// for recursively finding nodes.
    /// </summary>
    public class HtmlNodeCollection : List<HtmlNode>
    {
        private HtmlElementNode ParentNode;

        public HtmlNodeCollection(HtmlElementNode parentNode)
        {
            ParentNode = parentNode;
        }

        #region Add/remove nodes

        public new void Add(HtmlNode node)
        {
            Debug.Assert(!Contains(node));
            if (Count > 0)
            {
                HtmlNode lastNode = this[Count - 1];

                // Note: We must detect the derived type and not a base type here
                if (node.GetType() == typeof(HtmlTextNode) && lastNode.GetType() == typeof(HtmlTextNode))
                {
                    // Combine if two consecutive HtmlTextNodes
                    lastNode.Html += node.Html;
                    return;
                }
                else
                {
                    lastNode.NextNode = node;
                    node.PrevNode = lastNode;
                }
            }
            else node.PrevNode = null;
            node.NextNode = null;
            node.ParentNode = ParentNode;

            base.Add(node);
        }

        public new void AddRange(IEnumerable<HtmlNode> nodes)
        {
            foreach (HtmlNode node in nodes)
                Add(node);
        }

        public new void Remove(HtmlNode node)
        {
            Debug.Assert(Contains(node));
            RemoveAt(IndexOf(node));
        }

        public new void RemoveAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                HtmlNode node = this[index];
                if (index > 0)
                    this[index - 1].NextNode = node.NextNode;
                if (index < (Count - 1))
                    this[index + 1].PrevNode = node.PrevNode;
                base.RemoveAt(index);
            }
        }

        #endregion

    }
}
