// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Base class for all HTML nodes.
    /// </summary>
    public abstract class HtmlNode
    {
        /// <summary>
        /// Returns this node's parent node, or <c>null</c> if this node
        /// is a top-level node.
        /// </summary>
        public HtmlElementNode ParentNode { get; internal set; }

        /// <summary>
        /// Returns this node's next sibling, or <c>null</c> if this node
        /// is the last of all its siblings.
        /// </summary>
        public HtmlNode NextNode { get; internal set; }

        /// <summary>
        /// Returns this node's previous sibling, or <c>null</c> if this
        /// node is the first of all its siblings.
        /// </summary>
        public HtmlNode PrevNode { get; internal set; }

        /// <summary>
        /// Returns true if this node is a top-level node and has no parent.
        /// </summary>
        public bool IsTopLevelNode => ParentNode == null;

        /// <summary>
        /// Markup for everything inside of the HTML tags.
        /// </summary>
        public virtual string InnerHtml
        {
            get => string.Empty;
            set { }
        }

        /// <summary>
        /// Markup for everything including the outer HTML tags and everything inside
        /// of the HTML tags.
        /// </summary>
        public virtual string OuterHtml
        {
            get => string.Empty;
        }

        /// <summary>
        /// All text from this element. (Does not include any HTML markup.)
        /// </summary>
        public virtual string Text
        {
            get => string.Empty;
            set { }
        }

        /// <summary>
        /// An abbreviated form of the node's markup (no inner markup).
        /// </summary>
        public override string ToString() => string.Empty;
    }

    /// <summary>
    /// Represents an HTML header (!DOCTYPE) node.
    /// </summary>
    public class HtmlHeaderNode : HtmlNode
    {
        public HtmlAttributeCollection Attributes { get; private set; }

        public HtmlHeaderNode(HtmlAttributeCollection attributes)
        {
            Attributes = attributes;
        }

        public override string OuterHtml
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(HtmlRules.TagStart);
                builder.Append(HtmlRules.HtmlHeaderTag);
                builder.Append(Attributes.ToString());
                builder.Append(HtmlRules.TagEnd);
                return builder.ToString();
            }
        }

        public override string ToString() => OuterHtml;
    }

    /// <summary>
    /// Represents an XML header (?xml) node.
    /// </summary>
    public class XmlHeaderNode : HtmlNode
    {
        public HtmlAttributeCollection Attributes { get; private set; }

        public XmlHeaderNode(HtmlAttributeCollection attributes)
        {
            Attributes = attributes;
        }

        public override string OuterHtml
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(HtmlRules.TagStart);
                builder.Append(HtmlRules.XmlHeaderTag);
                builder.Append(Attributes.ToString());
                builder.Append("?");
                builder.Append(HtmlRules.TagEnd);
                return builder.ToString();
            }
        }

        public override string ToString() => OuterHtml;
    }

    /// <summary>
    /// Represents an HTML element (tag) node.
    /// </summary>
    public class HtmlElementNode : HtmlNode
    {
        /// <summary>
        /// This element's tag name.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// This elements attributes.
        /// </summary>
        public HtmlAttributeCollection Attributes { get; private set; }

        /// <summary>
        /// This elements child nodes.
        /// </summary>
        public HtmlNodeCollection Children { get; private set; }

        public HtmlElementNode(string tagName, HtmlAttributeCollection attributes = null)
        {
            TagName = tagName ?? string.Empty;
            Attributes = attributes ?? new HtmlAttributeCollection();
            Children = new HtmlNodeCollection(this);
        }

        /// <summary>
        /// Returns true if this element is self-closing and has no children.
        /// </summary>
        public bool IsSelfClosing => !Children.Any() && !HtmlRules.GetTagFlags(TagName).HasFlag(HtmlTagFlag.NoSelfClosing);

        public override string InnerHtml
        {
            get
            {
                if (Children.Any())
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (var node in Children)
                        builder.Append(node.OuterHtml);
                    return builder.ToString();
                }
                return string.Empty;
            }
            set
            {
                // Replaces all existing content
                Children.Clear();
                if (!string.IsNullOrEmpty(value))
                {
                    var parser = new HtmlParser();
                    Children.AddRange(parser.ParseChildren(value));
                }
            }
        }

        public override string OuterHtml
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                // Open tag
                builder.Append(HtmlRules.TagStart);
                builder.Append(TagName);
                // Note: Attributes returned in non-deterministic order
                builder.Append(Attributes.ToString());

                // Finish self-closing tag
                if (IsSelfClosing)
                {
                    Debug.Assert(!Children.Any());
                    builder.Append(' ');
                    builder.Append(HtmlRules.ForwardSlash);
                    builder.Append(HtmlRules.TagEnd);
                }
                else
                {
                    builder.Append(HtmlRules.TagEnd);
                    // Inner HTML
                    builder.Append(InnerHtml);
                    // Closing tag
                    builder.Append(HtmlRules.TagStart);
                    builder.Append(HtmlRules.ForwardSlash);
                    builder.Append(TagName);
                    builder.Append(HtmlRules.TagEnd);
                }
                return builder.ToString();
            }
        }

        public override string Text
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (var node in Children)
                    builder.Append(node.Text);
                return builder.ToString();
            }
            set
            {
                // Replaces all existing content
                Children.Clear();
                if (!string.IsNullOrEmpty(value))
                    Children.Add(new HtmlTextNode() { Text = value });
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // Open tag
            builder.Append(HtmlRules.TagStart);
            builder.Append(TagName);
            // Note: Attributes returned in non-deterministic order ???
            builder.Append(Attributes.ToString());
            builder.Append(' ');
            builder.Append(HtmlRules.ForwardSlash);
            builder.Append(HtmlRules.TagEnd);
            return builder.ToString();
        }
    }

    /// <summary>
    /// Represents a text node.
    /// </summary>
    public class HtmlTextNode : HtmlNode
    {
        private string Content;

        public HtmlTextNode(string html = null)
        {
            Content = html ?? string.Empty;
        }

        public override string InnerHtml
        {
            get => Content;
            set => Content = value;
        }

        public override string OuterHtml
        {
            get => InnerHtml;
        }

        public override string Text
        {
            get => WebUtility.HtmlDecode(Content);
            set => Content = WebUtility.HtmlEncode(value);
        }

        public override string ToString() => InnerHtml;
    }

    /// <summary>
    /// Represents a node that contains CDATA. This data is saved but not parsed.
    /// Examples include CDATA, comments and the content of SCRIPT and STYLE tags.
    /// </summary>
    public class HtmlCDataNode : HtmlTextNode
    {
        public string Prefix { get; set; }
        public string Suffix { get; set; }

        public HtmlCDataNode(string prefix, string suffix, string html)
            : base(html)
        {
            Prefix = prefix;
            Suffix = suffix;
        }

        public override string InnerHtml
        {
            get => base.InnerHtml;
            set => base.InnerHtml = value;
        }

        public override string OuterHtml => $"{Prefix}{base.ToString()}{Suffix}";

        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public override string ToString() => OuterHtml;
    }
}
