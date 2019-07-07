/////////////////////////////////////////////////////////////
// HTML Monkey
// Copyright (c) 2018 Jonathan Wood
// http://www.softcircuits.com, http://www.blackbeltcoder.com
//
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
        /// Parses an HTML document string and returns a new HtmlDocument.
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
            TextParser parser = new TextParser(html);
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
                    CDataDefinition definition = HtmlRules.CDataDefinitions.FirstOrDefault(dd => parser.MatchesCurrentPosition(dd.StartText, dd.IgnoreCase));
                    if (definition != null)
                    {
                        parentNode.Children.Add(ParseCDataNode(parser, definition));
                        continue;
                    }

                    // Closing tag
                    if (parser.Peek(1) == HtmlRules.ForwardSlash)
                    {
                        parser.MoveAhead(2);
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
                        parser.MoveTo(HtmlRules.TagEnd);
                        parser.MoveAhead();
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
                                parser.MoveAhead();
                                parser.MovePastWhitespace();
                                selfClosing = true;
                            }
                            else
                            {
                                selfClosing = false;
                            }
                            parser.MoveTo(HtmlRules.TagEnd);
                            parser.MoveAhead();

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
                int start = parser.Position;
                // Text must be at least 1 character (handle '<' that is not part of a tag)
                parser.MoveAhead();
                parser.MoveTo(HtmlRules.TagStart);
                Debug.Assert(parser.Position > start);
                parentNode.Children.Add(new HtmlTextNode(parser.Extract(start, parser.Position)));
            }

            //
            return rootNode.Children;
        }

        /// <summary>
        /// Attempts to parse an element tag at the current location. If the tag is parsed,
        /// the parser position is advanced to the end of the tag name and true is returned.
        /// Otherwise, false is returned and the current parser position does not change.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="tag"></param>
        private bool ParseTag(TextParser parser, out string tag)
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
                parser.MoveAhead();
                // Extract tag name
                int length = pos - 1;
                tag = parser.Text.Substring(parser.Position, length);
                parser.MoveAhead(length);
                return true;
            }
            // No tag found at this position
            return false;
        }

        /// <summary>
        /// Parses the attributes of an element tag. When finished, the parser
        /// position is at the next non-space character that follows the attributes.
        /// </summary>
        private HtmlAttributeCollection ParseAttributes(TextParser parser)
        {
            HtmlAttributeCollection attributes = new HtmlAttributeCollection();

            // Parse tag attributes
            parser.MovePastWhitespace();
            while (HtmlRules.IsAttributeNameCharacter(parser.Peek()))
            {
                // Parse attribute name
                HtmlAttribute attribute = new HtmlAttribute()
                {
                    Name = parser.ParseWhile(c => HtmlRules.IsAttributeNameCharacter(c))
                };
                Debug.Assert(attribute.Name.Length > 0);

                // Parse attribute value
                parser.MovePastWhitespace();
                if (parser.Peek() == '=')
                {
                    parser.MoveAhead(); // Skip '='
                    parser.MovePastWhitespace();
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
                parser.MovePastWhitespace();
            }
            return attributes;
        }

        /// <summary>
        /// Parses an HTML DOCTYPE header tag. Assumes current position is just after tag name.
        /// </summary>
        /// <param name="parser">Parser object.</param>
        private HtmlHeaderNode ParseHtmlHeader(TextParser parser)
        {
            HtmlHeaderNode node = new HtmlHeaderNode();
            while (true)
            {
                parser.MovePastWhitespace();
                char c = parser.Peek();
                if (HtmlRules.IsQuoteChar(c))
                    node.Parameters.Add($"\"{parser.ParseQuotedText()}\"");
                else if (HtmlRules.IsAttributeNameCharacter(c))
                    node.Parameters.Add(parser.ParseWhile(c2 => HtmlRules.IsAttributeNameCharacter(c2)));
                else
                    break;
            }
            parser.MoveTo(HtmlRules.TagEnd);
            parser.MoveAhead();
            return node;
        }

        /// <summary>
        /// Parses an XML header tag. Assumes current position is just after tag name.
        /// </summary>
        /// <param name="parser">Parser object.</param>
        private XmlHeaderNode ParseXmlHeader(TextParser parser)
        {
            XmlHeaderNode node = new XmlHeaderNode(ParseAttributes(parser));
            string tagEnd = "?>";
            parser.MoveTo(tagEnd);
            parser.MoveAhead(tagEnd.Length);
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
        private bool ParseToClosingTag(TextParser parser, string tag, out string content)
        {
            string endTag = $"</{tag}";
            int start = parser.Position;

            // Position assumed to just after open tag
            Debug.Assert(parser.Position > 0 && parser.Peek(-1) == HtmlRules.TagEnd);
            while (!parser.EndOfText)
            {
                parser.MoveTo(endTag, true);
                // Check that we didn't just match the first part of a longer tag
                if (!HtmlRules.IsTagCharacter(parser.Peek(endTag.Length)))
                {
                    content = parser.Extract(start, parser.Position);
                    parser.MoveAhead(endTag.Length);
                    parser.MoveTo(HtmlRules.TagEnd);
                    parser.MoveAhead();
                    return true;
                }
                parser.MoveAhead();
            }
            content = null;
            return false;
        }

        private HtmlCDataNode ParseCDataNode(TextParser parser, CDataDefinition definition)
        {
            Debug.Assert(parser.MatchesCurrentPosition(definition.StartText));
            parser.MoveAhead(definition.StartText.Length);
            int start = parser.Position;
            parser.MoveTo(definition.EndText);
            string content = parser.Extract(start, parser.Position);
            parser.MoveAhead(definition.EndText.Length);
            return new HtmlCDataNode(definition.StartText, definition.EndText, content);
        }
    }
}
