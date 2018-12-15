/////////////////////////////////////////////////////////////
// HTML Monkey
// Copyright (c) 2018 Jonathan Wood
// http://www.softcircuits.com, http://www.blackbeltcoder.com
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HtmlMonkey
{
    /// <summary>
    /// HtmlDocument class. Contains static methods to create an instance.
    /// Document nodes can be found in the <c>RootNodes</c> member.
    /// </summary>
    public class HtmlDocument
    {
        public string Title { get; set; }

        public HtmlNodeCollection RootNodes { get; set; }

        public HtmlDocument(string title = null)
        {
            Title = title;
            RootNodes = new HtmlNodeCollection(null);
        }

        /// <summary>
        /// Searches the document for nodes matching the specified selector.
        /// </summary>
        /// <param name="selector">Selector that describes the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> Find(string selector)
        {
            return Find(RootNodes, selector);
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        public IEnumerable<T> FindOfType<T>() where T : HtmlNode
        {
            return FindOfType<T>(RootNodes);
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        public IEnumerable<T> FindOfType<T>(Func<T, bool> predicate) where T : HtmlNode
        {
            return FindOfType<T>(RootNodes, predicate);
        }

        /// <summary>
        /// Recursively finds all HtmlNodes filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        public IEnumerable<HtmlNode> Find(Func<HtmlNode, bool> predicate)
        {
            return Find(RootNodes, predicate);
        }

        #region Static methods

        /// <summary>
        /// Constructs an HtmlDocument from the given HTML file.
        /// </summary>
        /// <param name="html">HTML file from which to build the document.</param>
        /// <returns>The newly created document.</returns>
        public static HtmlDocument FromFile(string path)
        {
            return FromHtml(File.ReadAllText(path));
        }

        /// <summary>
        /// Constructs an HtmlDocument from the given HTML text.
        /// </summary>
        /// <param name="html">HTML text from which to build the document.</param>
        /// <returns>The newly created document.</returns>
        public static HtmlDocument FromHtml(string html)
        {
            HtmlParser parser = new HtmlParser();
            return parser.Parse(html);
        }

        /// <summary>
        /// Searches the given nodes for ones matching the specified selector.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="selector">Selector that describes the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<HtmlElementNode> Find(IEnumerable<HtmlNode> nodes, string selector)
        {
            SelectorCollection selectors = Selector.ParseSelector(selector);
            return selectors.Find(nodes);
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        public static IEnumerable<T> FindOfType<T>(IEnumerable<HtmlNode> nodes) where T : HtmlNode
        {
            return Find(nodes, n => n.GetType() == typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        public static IEnumerable<T> FindOfType<T>(IEnumerable<HtmlNode> nodes, Func<T, bool> predicate) where T : HtmlNode
        {
            return Find(nodes, n => n.GetType() == typeof(T) && predicate((T)n)).Cast<T>();
        }

        /// <summary>
        /// Recursively finds all HtmlNodes filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        public static IEnumerable<HtmlNode> Find(IEnumerable<HtmlNode> nodes, Func<HtmlNode, bool> predicate)
        {
            List<HtmlNode> results = new List<HtmlNode>();
            FindRecursive(nodes, predicate, results);
            return results;
        }

        private static void FindRecursive(IEnumerable<HtmlNode> nodes, Func<HtmlNode, bool> predicate, List<HtmlNode> results)
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

    }
}
