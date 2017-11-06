using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace HtmlMonkey
{
    /// <summary>
    /// Base class for all HTML nodes.
    /// </summary>
    /// <remarks>
    /// Html: Inner HTML markup
    /// Text: Inner text
    /// ToString(): HTML markup (including outer tag, if any)
    /// </remarks>
    public abstract class HtmlNode
    {
        public HtmlElementNode ParentNode { get; internal set; }
        public HtmlNode NextNode { get; internal set; }
        public HtmlNode PrevNode { get; internal set; }

        public bool IsTopLevelNode => ParentNode == null;

        public virtual string Html
        {
            get { return string.Empty; }
            set { }
        }

        public virtual string Text
        {
            get { return string.Empty; }
            set { }
        }
    }

    /// <summary>
    /// Represents an HTML header (!DOCTYPE) node.
    /// </summary>
    public class HtmlHeaderNode : HtmlNode
    {
        public List<string> Parameters { get; private set; }

        public HtmlHeaderNode()
        {
            Parameters = new List<string>();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HtmlRules.TagStart);
            builder.Append(HtmlRules.HtmlHeaderTag);
            foreach (string parameter in Parameters)
                builder.AppendFormat(" {0}", parameter);
            builder.Append(HtmlRules.TagEnd);
            return builder.ToString();
        }
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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(HtmlRules.TagStart);
            builder.Append(HtmlRules.XmlHeaderTag);
            builder.Append(Attributes.ToString());
            builder.Append(HtmlRules.TagEnd);
            return builder.ToString();
        }
    }

    /// <summary>
    /// Represents an HTML element (tag) node.
    /// </summary>
    public class HtmlElementNode : HtmlNode
    {
        public string TagName { get; set; }
        public HtmlAttributeCollection Attributes { get; private set; }
        public HtmlNodeCollection Children { get; private set; }

        public HtmlElementNode(string tagName, HtmlAttributeCollection attributes = null)
        {
            TagName = tagName ?? string.Empty;
            Attributes = attributes ?? new HtmlAttributeCollection();
            Children = new HtmlNodeCollection(this);
        }

        public bool IsSelfClosing => !Children.Any() && !HtmlRules.GetTagFlags(TagName).HasFlag(HtmlTagFlag.NoSelfClosing);

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

        public override string Html
        {
            get
            {
                if (Children.Any())
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (var node in Children)
                        builder.Append(node.ToString());
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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // Open tag
            builder.Append(HtmlRules.TagStart);
            builder.Append(TagName);
            // Note: Attributes returned in non-deterministic order ???
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
                builder.Append(Html);

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
    /// Represents a text node.
    /// </summary>
    public class HtmlTextNode : HtmlNode
    {
        private string Content;

        public HtmlTextNode(string html = null)
        {
            Content = html ?? string.Empty;
        }

        public override string Html
        {
            get { return Content; }
            set { Content = value; }
        }

        public override string Text
        {
            get { return WebUtility.HtmlDecode(Content); }
            set { Content = WebUtility.HtmlEncode(value); }
        }

        public override string ToString() => Html;
    }

    /// <summary>
    /// Represents a node that contains CDATA. This data is saved
    /// but not parsed. Examples include CDATA, comments and inner
    /// SCRIPT tags.
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

        /// <summary>
        /// CData displays nothing as text, since we don't know if it's text or not.
        /// </summary>
        public override string Text
        {
            get { return string.Empty; }
            set { }
        }

        public override string ToString() => $"{Prefix}{base.ToString()}{Suffix}";
    }
}
