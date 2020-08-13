// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Abstract base class for all HTML nodes.
    /// </summary>
    public abstract class HtmlNode
    {
        /// <summary>
        /// Gets this node's parent node, or <c>null</c> if this node
        /// is a top-level node.
        /// </summary>
        public HtmlElementNode ParentNode { get; internal set; }

        /// <summary>
        /// Gets this node's next sibling, or <c>null</c> if this node
        /// is the last of its siblings.
        /// </summary>
        public HtmlNode NextNode { get; internal set; }

        /// <summary>
        /// Gets this node's previous sibling, or <c>null</c> if this
        /// node is the first of its siblings.
        /// </summary>
        public HtmlNode PrevNode { get; internal set; }

        /// <summary>
        /// Returns <c>true</c> if this node is a top-level node and has no parent.
        /// </summary>
        public bool IsTopLevelNode => ParentNode == null;

        /// <summary>
        /// Gets this node's markup, excluding the outer HTML tags.
        /// </summary>
        public virtual string InnerHtml
        {
            get => string.Empty;
            set { }
        }

        /// <summary>
        /// Gets this node's markup, including the outer HTML tags.
        /// </summary>
        public virtual string OuterHtml
        {
            get => string.Empty;
        }

        /// <summary>
        /// Gets this node's text. No markup is included.
        /// </summary>
        public virtual string Text
        {
            get => string.Empty;
            set { }
        }
    }

    /// <summary>
    /// Represents an HTML header (!DOCTYPE) node.
    /// </summary>
    public class HtmlHeaderNode : HtmlNode
    {
        /// <summary>
        /// Gets this node's attributes.
        /// </summary>
        public HtmlAttributeCollection Attributes { get; private set; }

        /// <summary>
        /// Constructs a <see cref="HtmlHeaderNode"/> instance.
        /// </summary>
        /// <param name="attributes">List of attributes for this node.</param>
        public HtmlHeaderNode(HtmlAttributeCollection attributes)
        {
            Attributes = attributes;
        }

        /// <summary>
        /// Gets the outer markup.
        /// </summary>
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

        /// <summary>
        /// Converts this node to a string.
        /// </summary>
        public override string ToString() => $"<{HtmlRules.HtmlHeaderTag} />";
    }

    /// <summary>
    /// Represents an XML header (?xml) node.
    /// </summary>
    public class XmlHeaderNode : HtmlNode
    {
        /// <summary>
        /// Gets this node's attributes.
        /// </summary>
        public HtmlAttributeCollection Attributes { get; private set; }

        /// <summary>
        /// Constructs an <see cref="XmlHeaderNode"/> instance.
        /// </summary>
        /// <param name="attributes">List of attributes for this node.</param>
        public XmlHeaderNode(HtmlAttributeCollection attributes)
        {
            Attributes = attributes;
        }

        /// <summary>
        /// Gets the outer markup.
        /// </summary>
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

        /// <summary>
        /// Converts this node to a string.
        /// </summary>
        public override string ToString() => $"<{HtmlRules.XmlHeaderTag} />";
    }

    /// <summary>
    /// Represents an HTML element (tag) node.
    /// </summary>
    public class HtmlElementNode : HtmlNode
    {
        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Gets this element's attribute values.
        /// </summary>
        public HtmlAttributeCollection Attributes { get; private set; }

        /// <summary>
        /// Gets this element's child nodes.
        /// </summary>
        public HtmlNodeCollection Children { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="HtmlElementNode"/> instance.
        /// </summary>
        /// <param name="tagName">Element tag name.</param>
        /// <param name="attributes">Optional element attributes.</param>
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

        /// <summary>
        /// Gets or sets the inner markup. When setting the markup, it is parsed on
        /// the fly.
        /// </summary>
        public override string InnerHtml
        {
            get
            {
                if (!Children.Any())
                    return string.Empty;
                StringBuilder builder = new StringBuilder();
                foreach (var node in Children)
                    builder.Append(node.OuterHtml);
                return builder.ToString();
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

        /// <summary>
        /// Gets or sets the outer markup.
        /// </summary>
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

        /// <summary>
        /// Gets or sets this node's text.
        /// </summary>
        public override string Text
        {
            get
            {
                if (!Children.Any())
                    return string.Empty;
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

        /// <summary>
        /// Converts this node to a string.
        /// </summary>
        public override string ToString() => $"<{TagName ?? "(null)"} />";
    }

    /// <summary>
    /// Represents a text node.
    /// </summary>
    public class HtmlTextNode : HtmlNode
    {
        protected string Content;

        /// <summary>
        /// Constructs a new <see cref="HtmlTextNode"/> instance.
        /// </summary>
        /// <param name="html">Optional markup for this node.</param>
        public HtmlTextNode(string html = null)
        {
            Content = html ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets this node's raw text.
        /// </summary>
        public override string InnerHtml
        {
            get => Content;
            set => Content = value;
        }

        /// <summary>
        /// Gets this node's raw text.
        /// </summary>
        public override string OuterHtml
        {
            get => InnerHtml;
        }

        /// <summary>
        /// Gets or sets the text for this node. Automatically HTML-encodes
        /// and decodes text values.
        /// </summary>
        public override string Text
        {
            get => WebUtility.HtmlDecode(Content);
            set => Content = WebUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Converts this node to a string. (Same as <see cref="Text"/>.)
        /// </summary>
        public override string ToString() => Text;
    }

    /// <summary>
    /// Represents a node that contains CDATA. This data is saved but not parsed.
    /// Examples include CDATA, comments and the content of SCRIPT and STYLE tags.
    /// </summary>
    public class HtmlCDataNode : HtmlTextNode
    {
        /// <summary>
        /// Gets or sets this node's CDATA prefix markup.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets this node's CDATA suffix markup.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Constructs a new <see cref="HtmlCDataNode"/> instance.
        /// </summary>
        /// <param name="prefix">CDATA prefix markup.</param>
        /// <param name="suffix">CDATA suffix markup.</param>
        /// <param name="html">CDATA content.</param>
        public HtmlCDataNode(string prefix, string suffix, string html)
            : base(html)
        {
            Prefix = prefix;
            Suffix = suffix;
        }

        /// <summary>
        /// Gets or sets this node's inner content.
        /// </summary>
        public override string InnerHtml
        {
            get => base.InnerHtml;
            set => base.InnerHtml = value;
        }

        /// <summary>
        /// Gets this node's markup, including the outer prefix and suffix.
        /// </summary>
        public override string OuterHtml => $"{Prefix}{InnerHtml}{Suffix}";

        /// <summary>
        /// Returns an empty string. CDATA nodes do not contain text.
        /// </summary>
        public override string Text
        {
            get => string.Empty;
            set { }
        }

        /// <summary>
        /// Converts this node to a string.
        /// </summary>
        public override string ToString() => $"{Prefix}...{Suffix}";
    }
}
