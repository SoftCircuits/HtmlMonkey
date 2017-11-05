﻿using System;
using System.Collections.Generic;
using System.IO;

namespace HtmlMonkey
{
    public class HtmlDocument
    {
        public string Title { get; set; }
        public HtmlNodeCollection RootNodes { get; set; }

        public HtmlDocument(string title = null)
        {
            Title = title;
            RootNodes = new HtmlNodeCollection(null);
        }

        public static HtmlDocument FromHtml(string html)
        {
            HtmlParser parser = new HtmlParser();
            return parser.Parse(html);
        }

        public static HtmlDocument FromFile(string path)
        {
            HtmlParser parser = new HtmlParser();
            return parser.Parse(File.ReadAllText(path));
        }

        /// <summary>
        /// Recursively finds all the element tags with the given tag name. The tag comparison is not case sensitive.
        /// </summary>
        /// <param name="tagName">Tag name to match.</param>
        public IEnumerable<HtmlElementNode> FindTags(string tagName)
        {
            return RootNodes.FindOfType<HtmlElementNode>(n => n.TagName.Equals(tagName, HtmlRules.TagStringComparison));
        }

        /// <summary>
        /// Recursively finds all HtmlNodes filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        public IEnumerable<HtmlNode> Find(Func<HtmlNode, bool> predicate)
        {
            return RootNodes.Find(predicate);
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type.
        /// </summary>
        public IEnumerable<T> FindOfType<T>()
        {
            return RootNodes.FindOfType<T>();
        }

        /// <summary>
        /// Recursively finds all nodes of the specified type filtered by the given predicate.
        /// </summary>
        /// <param name="predicate">A function that determines if the item should be included in the results.</param>
        public IEnumerable<T> FindOfType<T>(Func<T, bool> predicate) where T : HtmlNode
        {
            return RootNodes.FindOfType<T>(predicate);
        }
    }
}
