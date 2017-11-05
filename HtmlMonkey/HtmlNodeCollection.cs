using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace HtmlMonkey
{
    public class HtmlNodeCollection : IEnumerable<HtmlNode>
    {
        private HtmlElementNode ParentNode;
        private List<HtmlNode> Nodes;

        public HtmlNodeCollection(HtmlElementNode parentNode)
        {
            Nodes = new List<HtmlNode>();
            ParentNode = parentNode;
        }

        public HtmlNode this[int index] => Nodes[index];

        #region Add/remove nodes

        public void Add(HtmlNode node)
        {
            Debug.Assert(!Nodes.Contains(node));
            if (Nodes.Count > 0)
            {
                HtmlNode lastNode = Nodes[Nodes.Count - 1];

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

            Nodes.Add(node);
        }

        public void AddRange(IEnumerable<HtmlNode> nodes)
        {
            foreach (HtmlNode node in nodes)
                Add(node);
        }

        public void Remove(HtmlNode node)
        {
            Debug.Assert(Nodes.Contains(node));
            RemoveAt(Nodes.IndexOf(node));
        }

        public void RemoveAt(int index)
        {
            Debug.Assert(index >= 0 && index < Nodes.Count);
            HtmlNode node = Nodes[index];
            if (index > 0)
                Nodes[index - 1].NextNode = node.NextNode;
            if (index < (Nodes.Count - 1))
                Nodes[index + 1].PrevNode = node.PrevNode;
            Nodes.RemoveAt(index);
        }

        public void Clear()
        {
            Nodes.Clear();
        }

        #endregion

        #region Find nodes

        /// <summary>
        /// Recursively finds all HtmlNodes filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        public IEnumerable<HtmlNode> Find(Func<HtmlNode, bool> predicate)
        {
            List<HtmlNode> results = new List<HtmlNode>();
            FindRecursive(this, predicate, results);
            return results;
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        public IEnumerable<T> FindOfType<T>()
        {
            foreach (object obj in Find(n => n.GetType() == typeof(T)))
                yield return (T)obj;
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        public IEnumerable<T> FindOfType<T>(Func<T, bool> predicate) where T : HtmlNode
        {
            foreach (object obj in Find(n => n.GetType() == typeof(T) && predicate((T)n)))
                yield return (T)obj;
        }

        private void FindRecursive(HtmlNodeCollection nodes, Func<HtmlNode, bool> predicate, List<HtmlNode> results)
        {
            foreach (var node in nodes)
            {
                if (predicate(node))
                    results.Add(node);
                if (node is HtmlElementNode elementNode)
                    FindRecursive(elementNode.Children, predicate, results);
            }
        }

        #endregion

        #region IEnumerable interface

        public IEnumerator<HtmlNode> GetEnumerator() => Nodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

    }
}
