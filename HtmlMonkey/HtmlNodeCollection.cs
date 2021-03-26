// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Represents a collection of nodes.
    /// </summary>
    public class HtmlNodeCollection : List<HtmlNode>
    {
        private readonly HtmlElementNode? ParentNode;

        public HtmlNodeCollection(HtmlElementNode? parentNode = null)
        {
            ParentNode = parentNode;
        }

        /// <summary>
        /// Recursively searches this node collection for nodes matching the specified selector.
        /// </summary>
        /// <param name="selector">Selector that describes the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> Find(string? selector) => Find(this, selector);

        /// <summary>
        /// Recursively searches this node collection for nodes matching the specified compiled
        /// selector.
        /// </summary>
        /// <param name="selectors">Compiled selectors that describe the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> Find(SelectorCollection selectors) => Find(this, selectors);

        /// <summary>
        /// Recursively finds all nodes in this document for each the given predicate returns true.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included
        /// in the results.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlNode> Find(Func<HtmlNode, bool> predicate) => Find(this, predicate);

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<T> FindOfType<T>() where T : HtmlNode => FindOfType<T>(this);

        /// <summary>
        /// Recursively finds all nodes of the specified type, and for which the given predicate
        /// returns true.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in
        /// the results.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<T> FindOfType<T>(Func<T, bool> predicate) where T : HtmlNode => FindOfType<T>(this, predicate);

        /// <summary>
        /// Generates an HTML string from the contents of this node collection.
        /// </summary>
        /// <returns>A string with the markup for this node collection.</returns>
        public string ToHtml() => string.Concat(this.Select(n => n.OuterHtml));

        #region Static methods

        /// <summary>
        /// Recursively finds all nodes for which the given predicate returns true.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<HtmlNode> Find(IEnumerable<HtmlNode> nodes, Func<HtmlNode, bool> predicate)
        {
            // Note: Implemented without recursion for better performance on deeply nested collections

            var stack = new Stack<IEnumerator<HtmlNode>>();
            var enumerator = nodes.GetEnumerator();

            try
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        HtmlNode node = enumerator.Current;
                        if (predicate(node))
                            yield return node;

                        if (node is HtmlElementNode elementNode)
                        {
                            stack.Push(enumerator);
                            enumerator = elementNode.Children.GetEnumerator();
                        }
                    }
                    else if (stack.Count > 0)
                    {
                        enumerator.Dispose();
                        enumerator = stack.Pop();
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
            finally
            {
                enumerator.Dispose();

                // Dispose enumerators in case of exception
                while (stack.Count > 0)
                {
                    enumerator = stack.Pop();
                    enumerator.Dispose();
                }
            }
        }

        /// <summary>
        /// Recursively searches the given nodes for ones matching the specified selectors.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="selector">Selector that describes the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<HtmlElementNode> Find(IEnumerable<HtmlNode> nodes, string? selector)
        {
            SelectorCollection selectors = Selector.ParseSelector(selector);
            return selectors.Find(nodes);
        }

        /// <summary>
        /// Recursively searches the given nodes for ones matching the specified compiled selectors.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="selectors">Compiled selectors that describe the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<HtmlElementNode> Find(IEnumerable<HtmlNode> nodes, SelectorCollection selectors) => selectors.Find(nodes);

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<T> FindOfType<T>(IEnumerable<HtmlNode> nodes) where T : HtmlNode => Find(nodes, n => n is T).Cast<T>();

        /// <summary>
        /// Recursively finds all nodes of the specified type for which the given predicate returns true.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<T> FindOfType<T>(IEnumerable<HtmlNode> nodes, Func<T, bool> predicate) where T : HtmlNode => Find(nodes, n => n is T node && predicate(node)).Cast<T>();

        #endregion

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
#pragma warning disable CS8603 // Possible null reference return.
                    return lastNode as T;
#pragma warning restore CS8603 // Possible null reference return.
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
            int i = IndexOf(node);
            if (i >= 0)
                RemoveAt(i);
        }

        /// <summary>
        /// Removes the node at the specified position from the collection.
        /// </summary>
        /// <param name="index">The position of the item to be removed.</param>
        /// <remarks>
        /// Overrides <see cref="List{T}.RemoveAt(int)"/> in order to handle
        /// navigation property fixups.
        /// </remarks>
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
