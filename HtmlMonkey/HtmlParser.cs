// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Parsing.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Class to parse HTML or XML.
    /// </summary>
    internal class HtmlParser
    {
        private ParsingHelper ParsingHelper;

        /// <summary>
        /// Parses an HTML document string and returns a new <see cref="HtmlDocument"/>.
        /// </summary>
        /// <param name="html">The HTML text to parse.</param>
        public HtmlDocument Parse(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.RootNodes.AddRange(ParseChildren(html));
            return document;
        }

        /// <summary>
        /// Parses the given HTML string into a collection of root nodes.
        /// </summary>
        /// <param name="html">The HTML text to parse.</param>
        public IEnumerable<HtmlNode> ParseChildren(string html)
        {
            ParsingHelper = new ParsingHelper(html);
            HtmlElementNode rootNode = new HtmlElementNode("[Root]");
            HtmlElementNode parentNode = rootNode;
            string tag;
            bool selfClosing;

            // Loop until end of input
            while (!ParsingHelper.EndOfText)
            {
                if (ParsingHelper.Peek() == HtmlRules.TagStart)
                {
                    // Test for CDATA segments, which we store but do not parse. These includes comments.
                    CDataDefinition definition = HtmlRules.CDataDefinitions.FirstOrDefault(dd => ParsingHelper.MatchesCurrentPosition(dd.StartText, dd.StringComparison));
                    if (definition != null)
                    {
                        parentNode.Children.Add(ParseCDataNode(definition));
                        continue;
                    }

                    // Closing tag
                    if (ParsingHelper.Peek(1) == HtmlRules.ForwardSlash)
                    {
                        ParsingHelper += 2;
                        tag = ParsingHelper.ParseWhile(HtmlRules.IsTagCharacter);
                        if (tag.Length > 0)
                        {
                            if (parentNode.TagName.Equals(tag, HtmlRules.TagStringComparison))
                            {
                                // Should never have matched parent if the top-level node
                                Debug.Assert(!parentNode.IsTopLevelNode);
                                parentNode = parentNode.ParentNode;
                            }
                            else
                            {
                                // Handle mismatched closing tag
                                int tagPriority = HtmlRules.GetTagNestLevel(tag);
                                while (!parentNode.IsTopLevelNode && tagPriority > HtmlRules.GetTagNestLevel(parentNode.TagName))
                                    parentNode = parentNode.ParentNode;
                                if (parentNode.TagName.Equals(tag, HtmlRules.TagStringComparison))
                                {
                                    Debug.Assert(!parentNode.IsTopLevelNode);
                                    parentNode = parentNode.ParentNode;
                                }
                            }
                        }
                        ParsingHelper.SkipTo(HtmlRules.TagEnd);
                        ParsingHelper++;
                        continue;
                    }

                    // Open tag
                    if (ParseTag(out tag))
                    {
                        HtmlTagFlag flags = HtmlRules.GetTagFlags(tag);
                        if (flags.HasFlag(HtmlTagFlag.HtmlHeader))
                        {
                            parentNode.Children.Add(ParseHtmlHeader());
                        }
                        else if (flags.HasFlag(HtmlTagFlag.XmlHeader))
                        {
                            parentNode.Children.Add(ParseXmlHeader());
                        }
                        else
                        {
                            // Parse attributes
                            HtmlAttributeCollection attributes = ParseAttributes();

                            // Parse rest of tag
                            if (ParsingHelper.Peek() == HtmlRules.ForwardSlash)
                            {
                                ParsingHelper++;
                                ParsingHelper.SkipWhiteSpace();
                                selfClosing = true;
                            }
                            else
                            {
                                selfClosing = false;
                            }
                            ParsingHelper.SkipTo(HtmlRules.TagEnd);
                            ParsingHelper++;

                            // Add node
                            HtmlElementNode node = new HtmlElementNode(tag, attributes);
                            while (!HtmlRules.TagMayContain(parentNode.TagName, tag) && !parentNode.IsTopLevelNode)
                            {
                                Debug.Assert(parentNode.ParentNode != null);
                                parentNode = parentNode.ParentNode;
                            }
                            parentNode.Children.Add(node);

                            if (flags.HasFlag(HtmlTagFlag.CData))
                            {
                                // CDATA tags are treated as elements but we store and do not parse the inner content
                                if (!selfClosing)
                                {
                                    if (ParseToClosingTag(tag, out string content) && content.Length > 0)
                                        node.Children.Add(new HtmlCDataNode(string.Empty, string.Empty, content));
                                }
                            }
                            else
                            {
                                if (selfClosing && flags.HasFlag(HtmlTagFlag.NoSelfClosing))
                                    selfClosing = false;
                                if (!selfClosing && !flags.HasFlag(HtmlTagFlag.NoChildren))
                                    parentNode = node;  // Node becomes new parent
                            }
                        }
                        continue;
                    }
                }

                // Text node: must be at least 1 character (handle '<' that was not a valid tag)
                string text = ParsingHelper.ParseCharacter();
                text += ParsingHelper.ParseTo(HtmlRules.TagStart);
                parentNode.Children.Add(new HtmlTextNode(text));
            }

            // Return top-level nodes from nodes just parsed
            return rootNode.Children;
        }

        /// <summary>
        /// Attempts to parse an element tag at the current location. If the tag is parsed,
        /// the parser position is advanced to the end of the tag name and true is returned.
        /// Otherwise, false is returned and the current parser position does not change.
        /// </summary>
        /// <param name="tag">Parsed tag name.</param>
        private bool ParseTag(out string tag)
        {
            tag = null;
            int pos = 0;

            Debug.Assert(ParsingHelper.Peek() == HtmlRules.TagStart);
            char c = ParsingHelper.Peek(++pos);
            if (c == '!' || c == '?')
                c = ParsingHelper.Peek(++pos);

            if (HtmlRules.IsTagCharacter(c))
            {
                while (HtmlRules.IsTagCharacter(ParsingHelper.Peek(++pos)))
                    ;
                // Move past '<'
                ParsingHelper++;
                // Extract tag name
                int length = pos - 1;
                tag = ParsingHelper.Text.Substring(ParsingHelper.Index, length);
                ParsingHelper += length;
                return true;
            }
            // No tag found at this position
            return false;
        }

        /// <summary>
        /// Parses the attributes of an element tag. When finished, the parser
        /// position is at the next non-space character that follows the attributes.
        /// </summary>
        private HtmlAttributeCollection ParseAttributes()
        {
            HtmlAttributeCollection attributes = new HtmlAttributeCollection();

            // Parse tag attributes
            ParsingHelper.SkipWhiteSpace();
            char ch = ParsingHelper.Peek();
            while (HtmlRules.IsAttributeNameCharacter(ch) || HtmlRules.IsQuoteChar(ch))
            {
                // Parse attribute name
                HtmlAttribute attribute = new HtmlAttribute();
                if (HtmlRules.IsQuoteChar(ch))
                    attribute.Name = $"\"{ParsingHelper.ParseQuotedText()}\"";
                else
                    attribute.Name = ParsingHelper.ParseWhile(HtmlRules.IsAttributeNameCharacter);
                Debug.Assert(attribute.Name.Length > 0);

                // Parse attribute value
                ParsingHelper.SkipWhiteSpace();
                if (ParsingHelper.Peek() == '=')
                {
                    ParsingHelper++; // Skip '='
                    ParsingHelper.SkipWhiteSpace();
                    if (HtmlRules.IsQuoteChar(ParsingHelper.Peek()))
                    {
                        // Quoted attribute value
                        attribute.Value = ParsingHelper.ParseQuotedText();
                    }
                    else
                    {
                        // Unquoted attribute value
                        attribute.Value = ParsingHelper.ParseWhile(HtmlRules.IsAttributeValueCharacter);
                        Debug.Assert(attribute.Value.Length > 0);
                    }
                }
                else
                {
                    // Null attribute value indicates no equals sign
                    attribute.Value = null;
                }
                // Add attribute to tag
                attributes.Add(attribute);
                // Continue
                ParsingHelper.SkipWhiteSpace();
                ch = ParsingHelper.Peek();
            }
            return attributes;
        }

        /// <summary>
        /// Parses an HTML DOCTYPE header tag. Assumes current position is just after tag name.
        /// </summary>
        private HtmlHeaderNode ParseHtmlHeader()
        {
            HtmlHeaderNode node = new HtmlHeaderNode(ParseAttributes());
            string tagEnd = ">";
            ParsingHelper.SkipTo(tagEnd);
            ParsingHelper += tagEnd.Length;
            return node;
        }

        /// <summary>
        /// Parses an XML header tag. Assumes current position is just after tag name.
        /// </summary>
        private XmlHeaderNode ParseXmlHeader()
        {
            XmlHeaderNode node = new XmlHeaderNode(ParseAttributes());
            string tagEnd = "?>";
            ParsingHelper.SkipTo(tagEnd);
            ParsingHelper += tagEnd.Length;
            return node;
        }

        /// <summary>
        /// Moves the parser position to the closing tag for the given tag name.
        /// If the closing tag is not found, the parser position is set to the end
        /// of the text and false is returned.
        /// </summary>
        /// <param name="tag">Tag name for which the closing tag is being searched.</param>
        /// <param name="content">Returns the content before the closing tag</param>
        /// <returns></returns>
        private bool ParseToClosingTag(string tag, out string content)
        {
            string endTag = $"</{tag}";
            int start = ParsingHelper;

            // Position assumed to just after open tag
            Debug.Assert(ParsingHelper.Index > 0 && ParsingHelper.Peek(-1) == HtmlRules.TagEnd);
            while (!ParsingHelper.EndOfText)
            {
                ParsingHelper.SkipTo(endTag, StringComparison.OrdinalIgnoreCase);
                // Check that we didn't just match the first part of a longer tag
                if (!HtmlRules.IsTagCharacter(ParsingHelper.Peek(endTag.Length)))
                {
                    content = ParsingHelper.Extract(start, ParsingHelper.Index);
                    ParsingHelper += endTag.Length;
                    ParsingHelper.SkipTo(HtmlRules.TagEnd);
                    ParsingHelper++;
                    return true;
                }
                ParsingHelper++;
            }
            content = null;
            return false;
        }

        private HtmlCDataNode ParseCDataNode(CDataDefinition definition)
        {
            Debug.Assert(ParsingHelper.MatchesCurrentPosition(definition.StartText));
            ParsingHelper += definition.StartText.Length;
            string content = ParsingHelper.ParseTo(definition.EndText);
            ParsingHelper += definition.EndText.Length;
            return new HtmlCDataNode(definition.StartText, definition.EndText, content);
        }
    }
}
