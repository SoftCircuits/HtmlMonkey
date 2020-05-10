// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Represents a collection of nodes.
    /// </summary>
    public class HtmlNodeCollection : List<HtmlNode>
    {
        private readonly HtmlElementNode ParentNode;

        public HtmlNodeCollection(HtmlElementNode parentNode)
        {
            ParentNode = parentNode;
        }

        #region Add/remove nodes

        /// <summary>
        /// Appends the specified node to the end of the collection. If both the last node in the
        /// collection and the node being added are of type <see cref="HtmlTextNode"></see>, then
        /// the two text nodes are combined into one text node.
        /// </summary>
        /// <param name="node">Node to add.</param>
        /// <returns>Returns <paramref name="node"/> unless the text node was appended to the last
        /// node, in which case the last node is returned.</returns>
        public T Add<T>(T node) where T : HtmlNode
        {
            Debug.Assert(!Contains(node));
            if (Count > 0)
            {
                HtmlNode lastNode = this[Count - 1];

                // Note: We must detect the derived type and not a base type here
                if (node.GetType() == typeof(HtmlTextNode) && lastNode.GetType() == typeof(HtmlTextNode))
                {
                    // Combine if two consecutive HtmlTextNodes
                    lastNode.InnerHtml += node.InnerHtml;
                    return lastNode as T;
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

            return node;
        }

        /// <summary>
        /// Appends a range of nodes using the <see cref="Add"></see> method
        /// to add each one.
        /// </summary>
        /// <param name="nodes">List of nodes to add.</param>
        public new void AddRange(IEnumerable<HtmlNode> nodes)
        {
            foreach (HtmlNode node in nodes)
                Add(node);
        }

        /// <summary>
        /// Removes the specified node from the collection.
        /// </summary>
        /// <param name="node"></param>
        public new void Remove(HtmlNode node)
        {
            Debug.Assert(Contains(node));
            RemoveAt(IndexOf(node));
        }

        /// <summary>
        /// Removes the node at the specified position from the collection.
        /// </summary>
        /// <param name="index">The position of the item to be removed.</param>
        public new void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            HtmlNode node = this[index];
            if (index > 0)
                this[index - 1].NextNode = node.NextNode;
            if (index < (Count - 1))
                this[index + 1].PrevNode = node.PrevNode;
            base.RemoveAt(index);
        }

        #endregion

    }
}
