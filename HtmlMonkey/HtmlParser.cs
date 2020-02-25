// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
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
        /// <summary>
        /// Parses an HTML document string and returns a new HtmlMonkeyDocument.
        /// </summary>
        /// <param name="html">The HTML text to parse.</param>
        public HtmlMonkeyDocument Parse(string html)
        {
            HtmlMonkeyDocument document = new HtmlMonkeyDocument();
            document.RootNodes.AddRange(ParseChildren(html));
            return document;
        }

        /// <summary>
        /// Parses the given HTML string into a number of root nodes.
        /// </summary>
        /// <param name="html">The HTML text to parse.</param>
        public IEnumerable<HtmlNode> ParseChildren(string html)
        {
            ParsingHelper parser = new ParsingHelper(html);
            HtmlElementNode rootNode = new HtmlElementNode("[Root]");
            HtmlElementNode parentNode = rootNode;
            string tag;
            bool selfClosing;

            // Loop until end of input
            while (!parser.EndOfText)
            {
                if (parser.Peek() == HtmlRules.TagStart)
                {
                    // Test for CDATA segments, which we store but do not parse. This includes comments.
                    CDataDefinition definition = HtmlRules.CDataDefinitions.FirstOrDefault(dd => parser.MatchesCurrentPosition(dd.StartText,
                        dd.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
                    if (definition != null)
                    {
                        parentNode.Children.Add(ParseCDataNode(parser, definition));
                        continue;
                    }

                    // Closing tag
                    if (parser.Peek(1) == HtmlRules.ForwardSlash)
                    {
                        parser += 2;
                        tag = parser.ParseWhile(c => HtmlRules.IsTagCharacter(c));
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
                                int tagPriority = HtmlRules.GetTagPriority(tag);
                                while (!parentNode.IsTopLevelNode && tagPriority > HtmlRules.GetTagPriority(parentNode.TagName))
                                    parentNode = parentNode.ParentNode;
                                if (parentNode.TagName.Equals(tag, HtmlRules.TagStringComparison))
                                {
                                    Debug.Assert(!parentNode.IsTopLevelNode);
                                    parentNode = parentNode.ParentNode;
                                }
                            }
                        }
                        parser.SkipTo(HtmlRules.TagEnd);
                        parser++;
                        continue;
                    }

                    // Open tag
                    if (ParseTag(parser, out tag))
                    {
                        HtmlTagFlag flags = HtmlRules.GetTagFlags(tag);
                        if (flags.HasFlag(HtmlTagFlag.HtmlHeader))
                        {
                            parentNode.Children.Add(ParseHtmlHeader(parser));
                        }
                        else if (flags.HasFlag(HtmlTagFlag.XmlHeader))
                        {
                            parentNode.Children.Add(ParseXmlHeader(parser));
                        }
                        else
                        {
                            // Parse attributes
                            HtmlAttributeCollection attributes = ParseAttributes(parser);

                            // Parse rest of tag
                            if (parser.Peek() == HtmlRules.ForwardSlash)
                            {
                                parser++;
                                parser.SkipWhiteSpace();
                                selfClosing = true;
                            }
                            else
                            {
                                selfClosing = false;
                            }
                            parser.SkipTo(HtmlRules.TagEnd);
                            parser++;

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
                                    if (ParseToClosingTag(parser, tag, out string content) && content.Length > 0)
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

                // Text node
                int start = parser.Index;
                // Text must be at least 1 character (handle '<' that is not part of a tag)
                parser++;
                parser.SkipTo(HtmlRules.TagStart);
                Debug.Assert(parser.Index > start);
                parentNode.Children.Add(new HtmlTextNode(parser.Extract(start, parser.Index)));
            }

            // Return top-level nodes from nodes just parsed
            return rootNode.Children;
        }

        /// <summary>
        /// Attempts to parse an element tag at the current location. If the tag is parsed,
        /// the parser position is advanced to the end of the tag name and true is returned.
        /// Otherwise, false is returned and the current parser position does not change.
        /// </summary>
        /// <param name="parser">Text parser.</param>
        /// <param name="tag">Parsed tag name.</param>
        private bool ParseTag(ParsingHelper parser, out string tag)
        {
            tag = null;
            int pos = 0;

            Debug.Assert(parser.Peek() == HtmlRules.TagStart);
            char c = parser.Peek(++pos);
            if (c == '!' || c == '?')
                c = parser.Peek(++pos);

            if (HtmlRules.IsTagCharacter(c))
            {
                while (HtmlRules.IsTagCharacter(parser.Peek(++pos)))
                    ;
                // Move past '<'
                parser++;
                // Extract tag name
                int length = pos - 1;
                tag = parser.Text.Substring(parser.Index, length);
                parser += length;
                return true;
            }
            // No tag found at this position
            return false;
        }

        /// <summary>
        /// Parses the attributes of an element tag. When finished, the parser
        /// position is at the next non-space character that follows the attributes.
        /// </summary>
        private HtmlAttributeCollection ParseAttributes(ParsingHelper parser)
        {
            HtmlAttributeCollection attributes = new HtmlAttributeCollection();

            // Parse tag attributes
            parser.SkipWhiteSpace();
            char ch = parser.Peek();
            while (HtmlRules.IsAttributeNameCharacter(ch) || HtmlRules.IsQuoteChar(ch))
            {
                // Parse attribute name
                HtmlAttribute attribute = new HtmlAttribute();
                if (HtmlRules.IsQuoteChar(ch))
                    attribute.Name = $"\"{parser.ParseQuotedText()}\"";
                else
                    attribute.Name = parser.ParseWhile(c => HtmlRules.IsAttributeNameCharacter(c));
                Debug.Assert(attribute.Name.Length > 0);

                // Parse attribute value
                parser.SkipWhiteSpace();
                if (parser.Peek() == '=')
                {
                    parser++; // Skip '='
                    parser.SkipWhiteSpace();
                    if (HtmlRules.IsQuoteChar(parser.Peek()))
                    {
                        // Quoted attribute value
                        attribute.Value = parser.ParseQuotedText();
                    }
                    else
                    {
                        // Unquoted attribute value
                        attribute.Value = parser.ParseWhile(c => HtmlRules.IsAttributeValueCharacter(c));
                        Debug.Assert(attribute.Value.Length > 0);
                    }
                }
                else
                {
                    // Null attribute value indicates no equals sign
                    attribute.Value = null;
                }
                // Add attribute to tag
                attributes.Add(attribute.Name, attribute);
                // Continue
                parser.SkipWhiteSpace();
                ch = parser.Peek();
            }
            return attributes;
        }

        /// <summary>
        /// Parses an HTML DOCTYPE header tag. Assumes current position is just after tag name.
        /// </summary>
        /// <param name="parser">Parser object.</param>
        private HtmlHeaderNode ParseHtmlHeader(ParsingHelper parser)
        {
            HtmlHeaderNode node = new HtmlHeaderNode(ParseAttributes(parser));
            string tagEnd = ">";
            parser.SkipTo(tagEnd);
            parser += tagEnd.Length;
            return node;
        }

        /// <summary>
        /// Parses an XML header tag. Assumes current position is just after tag name.
        /// </summary>
        /// <param name="parser">Parser object.</param>
        private XmlHeaderNode ParseXmlHeader(ParsingHelper parser)
        {
            XmlHeaderNode node = new XmlHeaderNode(ParseAttributes(parser));
            string tagEnd = "?>";
            parser.SkipTo(tagEnd);
            parser += tagEnd.Length;
            return node;
        }

        /// <summary>
        /// Moves the parser position to the closing tag for the given tag name.
        /// If the closing tag is not found, the parser position is set to the end
        /// of the text and false is returned.
        /// </summary>
        /// <param name="parser">Parser object.</param>
        /// <param name="tag">Tag name for which the closing tag is being searched.</param>
        /// <param name="content">Returns the content before the closing tag</param>
        /// <returns></returns>
        private bool ParseToClosingTag(ParsingHelper parser, string tag, out string content)
        {
            string endTag = $"</{tag}";
            int start = parser.Index;

            // Position assumed to just after open tag
            Debug.Assert(parser.Index > 0 && parser.Peek(-1) == HtmlRules.TagEnd);
            while (!parser.EndOfText)
            {
                parser.SkipTo(endTag, StringComparison.OrdinalIgnoreCase);
                // Check that we didn't just match the first part of a longer tag
                if (!HtmlRules.IsTagCharacter(parser.Peek(endTag.Length)))
                {
                    content = parser.Extract(start, parser.Index);
                    parser += endTag.Length;
                    parser.SkipTo(HtmlRules.TagEnd);
                    parser++;
                    return true;
                }
                parser++;
            }
            content = null;
            return false;
        }

        private HtmlCDataNode ParseCDataNode(ParsingHelper parser, CDataDefinition definition)
        {
            Debug.Assert(parser.MatchesCurrentPosition(definition.StartText));
            parser += definition.StartText.Length;
            int start = parser.Index;
            parser.SkipTo(definition.EndText);
            string content = parser.Extract(start, parser.Index);
            parser += definition.EndText.Length;
            return new HtmlCDataNode(definition.StartText, definition.EndText, content);
        }
    }
}
