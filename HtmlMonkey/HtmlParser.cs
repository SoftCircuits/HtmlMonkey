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
        private TextParser Parser;

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
        /// Parses the given HTML string into a collection of root nodes and their
        /// children.
        /// </summary>
        /// <param name="html">The HTML text to parse.</param>
        public IEnumerable<HtmlNode> ParseChildren(string html)
        {
            HtmlElementNode rootNode = new HtmlElementNode("[TempContainer]");
            HtmlElementNode parentNode = rootNode;
            Parser = new TextParser(html);
            bool selfClosing;
            string tag;

            // Loop until end of input
            while (!Parser.EndOfText)
            {
                if (Parser.Peek() == HtmlRules.TagStart)
                {
                    // CDATA segments (blocks we store but don't parse--includes comments)
                    CDataDefinition definition = HtmlRules.CDataDefinitions.FirstOrDefault(dd => Parser.MatchesCurrentPosition(dd.StartText, dd.StartComparison));
                    if (definition != null)
                    {
                        parentNode.Children.Add(ParseCDataNode(definition));
                        continue;
                    }

                    // Closing tag
                    if (Parser.Peek(1) == HtmlRules.ForwardSlash)
                    {
                        Parser.Index += 2;
                        tag = Parser.ParseWhile(HtmlRules.IsTagCharacter);
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
                        Parser.SkipTo(HtmlRules.TagEnd);
                        Parser.Next();
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
                            if (Parser.Peek() == HtmlRules.ForwardSlash)
                            {
                                Parser.Next();
                                Parser.SkipWhiteSpace();
                                selfClosing = true;
                            }
                            else
                            {
                                selfClosing = false;
                            }
                            Parser.SkipTo(HtmlRules.TagEnd);
                            Parser.Next();

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

                // Text node: must be at least 1 character (handles '<' that was not a tag)
                string text = Parser.ParseCharacter();
                text += Parser.ParseTo(HtmlRules.TagStart);
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

            Debug.Assert(Parser.Peek() == HtmlRules.TagStart);
            char c = Parser.Peek(++pos);
            if (c == '!' || c == '?')
                c = Parser.Peek(++pos);

            if (HtmlRules.IsTagCharacter(c))
            {
                while (HtmlRules.IsTagCharacter(Parser.Peek(++pos)))
                    ;
                // Move past '<'
                Parser.Next();
                // Extract tag name
                int length = pos - 1;
                tag = Parser.Text.Substring(Parser.Index, length);
                Parser.Index += length;
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
            Parser.SkipWhiteSpace();
            char ch = Parser.Peek();
            while (HtmlRules.IsAttributeNameCharacter(ch) || HtmlRules.IsQuoteChar(ch))
            {
                // Parse attribute name
                HtmlAttribute attribute = new HtmlAttribute();
                if (HtmlRules.IsQuoteChar(ch))
                    attribute.Name = $"\"{Parser.ParseQuotedText()}\"";
                else
                    attribute.Name = Parser.ParseWhile(HtmlRules.IsAttributeNameCharacter);
                Debug.Assert(attribute.Name.Length > 0);

                // Parse attribute value
                Parser.SkipWhiteSpace();
                if (Parser.Peek() == '=')
                {
                    Parser.Next(); // Skip '='
                    Parser.SkipWhiteSpace();
                    if (HtmlRules.IsQuoteChar(Parser.Peek()))
                    {
                        // Quoted attribute value
                        attribute.Value = Parser.ParseQuotedText();
                    }
                    else
                    {
                        // Unquoted attribute value
                        attribute.Value = Parser.ParseWhile(HtmlRules.IsAttributeValueCharacter);
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
                Parser.SkipWhiteSpace();
                ch = Parser.Peek();
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
            Parser.SkipTo(tagEnd);
            Parser.Index += tagEnd.Length;
            return node;
        }

        /// <summary>
        /// Parses an XML header tag. Assumes current position is just after tag name.
        /// </summary>
        private XmlHeaderNode ParseXmlHeader()
        {
            XmlHeaderNode node = new XmlHeaderNode(ParseAttributes());
            string tagEnd = "?>";
            Parser.SkipTo(tagEnd);
            Parser.Index += tagEnd.Length;
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
            int start = Parser.Index;

            // Position assumed to just after open tag
            Debug.Assert(Parser.Index > 0 && Parser.Peek(-1) == HtmlRules.TagEnd);
            while (!Parser.EndOfText)
            {
                Parser.SkipTo(endTag, StringComparison.OrdinalIgnoreCase);
                // Check that we didn't just match the first part of a longer tag
                if (!HtmlRules.IsTagCharacter(Parser.Peek(endTag.Length)))
                {
                    content = Parser.Extract(start, Parser.Index);
                    Parser.Index += endTag.Length;
                    Parser.SkipTo(HtmlRules.TagEnd);
                    Parser.Next();
                    return true;
                }
                Parser.Next();
            }
            content = null;
            return false;
        }

        private HtmlCDataNode ParseCDataNode(CDataDefinition definition)
        {
            Debug.Assert(Parser.MatchesCurrentPosition(definition.StartText, definition.StartComparison));
            Parser.Index += definition.StartText.Length;
            string content = Parser.ParseTo(definition.EndText, definition.EndComparison);
            Parser.Index += definition.EndText.Length;
            return new HtmlCDataNode(definition.StartText, definition.EndText, content);
        }
    }
}
