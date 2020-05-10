// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Holds the nodes of a parsed HTML or XML document. Use the <see cref="RootNodes"/>
    /// property to access these nodes. Use the <see cref="ToHtml"/> method to convert the
    /// nodes back to markup.
    /// </summary>
    [Obsolete("This class has been deprecated and will be removed in a future version. Please use HtmlDocument instead.")]
    public class HtmlMonkeyDocument : HtmlDocument
    {
    }

    /// <summary>
    /// Holds the nodes of a parsed HTML or XML document. Use the <see cref="RootNodes"/>
    /// property to access these nodes. Use the <see cref="ToHtml"/> method to convert the
    /// nodes back to markup.
    /// </summary>
    public class HtmlDocument
    {
        /// <summary>
        /// Specifies the source document file. May be empty or <c>null</c> if the source file
        /// is not known.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The document root nodes. Provides access to all document nodes.
        /// </summary>
        public HtmlNodeCollection RootNodes { get; private set; }

        /// <summary>
        /// Initializes an empty <see cref="HtmlDocument"> instance.
        /// </summary>
        public HtmlDocument()
        {
            Path = string.Empty;
            RootNodes = new HtmlNodeCollection(null);
        }

        /// <summary>
        /// Generates an HTML string from the contents of this
        /// <see cref="HtmlDocument"></see>.
        /// </summary>
        /// <returns>A string with the markup for this document.</returns>
        public string ToHtml() => string.Concat(RootNodes.Select(n => n.OuterHtml));

        /// <summary>
        /// Recursively searches the given nodes for ones matching the specified selector.
        /// </summary>
        /// <param name="selector">Selector that describes the nodes to find.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> Find(string selector) => Find(RootNodes, selector);

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<T> FindOfType<T>() where T : HtmlNode => FindOfType<T>(RootNodes);

        /// <summary>
        /// Recursively finds all nodes of the specified type, filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in
        /// the results.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<T> FindOfType<T>(Func<T, bool> predicate) where T : HtmlNode => FindOfType<T>(RootNodes, predicate);

        /// <summary>
        /// Recursively finds all HtmlNodes filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included
        /// in the results.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlNode> Find(Func<HtmlNode, bool> predicate) => Find(RootNodes, predicate);

        #region Static methods

        /// <summary>
        /// Parses an HTML or XML file and returns an <see cref="HtmlDocument"></see> instance that
        /// contains the parsed nodes.
        /// </summary>
        /// <param name="path">The HTML or XML file to parse.</param>
        /// <returns>Returns an <see cref="HtmlDocument"></see> instance that contains the parsed
        /// nodes.</returns>
        public static HtmlDocument FromFile(string path) => FromHtml(File.ReadAllText(path));

        /// <summary>
        /// Parses an HTML or XML file and returns an <see cref="HtmlDocument"></see> instance that
        /// contains the parsed nodes.
        /// </summary>
        /// <param name="path">The HTML or XML file to parse.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <returns>Returns an <see cref="HtmlDocument"></see> instance that contains the parsed
        /// nodes.</returns>
        public static HtmlDocument FromFile(string path, Encoding encoding) => FromHtml(File.ReadAllText(path, encoding));

        /// <summary>
        /// Parses an HTML or XML string and returns an <see cref="HtmlDocument"></see> instance that
        /// contains the parsed nodes.
        /// </summary>
        /// <param name="html">The HTML or XML string to parse.</param>
        /// <returns>Returns an <see cref="HtmlDocument"></see> instance that contains the parsed
        /// nodes.</returns>
        public static HtmlDocument FromHtml(string html)
        {
            HtmlParser parser = new HtmlParser();
            return parser.Parse(html);
        }

        /// <summary>
        /// Recursively searches the given nodes for ones matching the specified selector.
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
        /// <param name="nodes">The nodes to be searched.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<T> FindOfType<T>(IEnumerable<HtmlNode> nodes) where T : HtmlNode
        {
            return Find(nodes, n => n.GetType() == typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type filtered by the given predicate.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<T> FindOfType<T>(IEnumerable<HtmlNode> nodes, Func<T, bool> predicate) where T : HtmlNode
        {
            return Find(nodes, n => n.GetType() == typeof(T) && predicate((T)n)).Cast<T>();
        }

        /// <summary>
        /// Recursively finds all HtmlNodes filtered by the given predicate.
        /// </summary>
        /// <param name="nodes">The nodes to be searched.</param>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        /// <returns>The matching nodes.</returns>
        public static IEnumerable<HtmlNode> Find(IEnumerable<HtmlNode> nodes, Func<HtmlNode, bool> predicate)
        {
            List<HtmlNode> results = new List<HtmlNode>();
            FindRecursive(nodes, predicate, results);
            return results;
        }

        /// <summary>
        /// Recursive portion of <see cref="Find"></see>.
        /// </summary>
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
