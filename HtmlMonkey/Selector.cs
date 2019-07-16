// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SoftCircuits.HtmlMonkey
{
    public class Selector
    {
        /// <summary>
        /// Tag name.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Attribute selectors.
        /// </summary>
        public List<SelectorAttribute> Attributes { get; private set; }

        /// <summary>
        /// Child selector.
        /// </summary>
        public Selector ChildSelector { get; set; }

        /// <summary>
        /// Indicates only matches one level down from parent
        /// </summary>
        public bool ImmediateChildOnly { get; set; }

        public Selector()
        {
            Tag = null;
            Attributes = new List<SelectorAttribute>();
            ChildSelector = null;
        }

        /// <summary>
        /// Returns true if selector has no data.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(Tag) && !Attributes.Any();

        /// <summary>
        /// Returns true if the specified node matches this selector.
        /// </summary>
        public bool IsMatch(HtmlElementNode node)
        {
            // Compare tag
            if (!string.IsNullOrWhiteSpace(Tag) && !string.Equals(Tag, node.TagName, HtmlRules.TagStringComparison))
                return false;

            // Compare attributes
            foreach (SelectorAttribute attribute in Attributes)
            {
                if (!attribute.IsMatch(node))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns the nodes that match this selector.
        /// </summary>
        /// <param name="nodes">Nodes to be searched.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> Find(IEnumerable<HtmlNode> nodes)
        {
            List<HtmlElementNode> results = null;
            bool matchTopLevelNodes = true;

            for (Selector selector = this; selector != null; selector = selector.ChildSelector)
            {
                results = new List<HtmlElementNode>();
                FindRecursive(nodes, selector, matchTopLevelNodes, true, results);
                nodes = results;
                matchTopLevelNodes = false;
            }
            return results?.Distinct() ?? Enumerable.Empty<HtmlElementNode>();
        }

        /// <summary>
        /// Resursive portion of Find().
        /// </summary>
        private void FindRecursive(IEnumerable<HtmlNode> nodes, Selector selector,
            bool matchTopLevelNodes, bool recurse, List<HtmlElementNode> results)
        {
            Debug.Assert(matchTopLevelNodes || recurse);

            foreach (var node in nodes)
            {
                if (node is HtmlElementNode elementNode)
                {
                    if (matchTopLevelNodes && selector.IsMatch(elementNode))
                        results.Add(elementNode);

                    if (recurse)
                        FindRecursive(elementNode.Children, selector, true, !selector.ImmediateChildOnly, results);
                }
            }
        }

        public override string ToString()
        {
            return Tag ?? "(null)";
        }

        #region Parsing

        private static Dictionary<char, string> SpecialCharacters = new Dictionary<char, string>
        {
            { '#', "id" },
            { '.', "class" },
            { ':', "type" },
        };

        /// <summary>
        /// Parses the given selector text and returns the corresponding data structures.
        /// </summary>
        /// <param name="selectorText">The selector text to be parsed.</param>
        /// <remarks>
        /// Returns multiple <see cref="Selector"/>s when the selector contains commas.
        /// </remarks>
        /// <returns>The parsed selector data structures.</returns>
        public static SelectorCollection ParseSelector(string selectorText)
        {
            SelectorCollection selectors = new SelectorCollection();

            if (!string.IsNullOrWhiteSpace(selectorText))
            {
                TextParser parser = new TextParser(selectorText);
                parser.MovePastWhitespace();

                while (!parser.EndOfText)
                {
                    // Test next character
                    char ch = parser.Peek();
                    if (IsNameCharacter(ch) || ch == '*')
                    {
                        // Parse tag name
                        Selector selector = selectors.GetLast(true);
                        if (ch == '*')
                            selector.Tag = null;    // Match all tags
                        else
                            selector.Tag = parser.ParseWhile(c => IsNameCharacter(c));
                    }
                    else if (SpecialCharacters.TryGetValue(ch, out string name))
                    {
                        // Parse special attributes
                        parser.MoveAhead();
                        string value = parser.ParseWhile(c => IsValueCharacter(c));
                        if (value.Length > 0)
                        {
                            SelectorAttribute attribute = new SelectorAttribute
                            {
                                Name = name,
                                Value = value,
                                Mode = SelectorAttributeMode.Contains
                            };

                            Selector selector = selectors.GetLast(true);
                            selector.Attributes.Add(attribute);
                        }
                    }
                    else if (ch == '[')
                    {
                        // Parse attribute selector
                        parser.MoveAhead();
                        parser.MovePastWhitespace();
                        name = parser.ParseWhile(c => IsNameCharacter(c));
                        if (name.Length > 0)
                        {
                            SelectorAttribute attribute = new SelectorAttribute
                            {
                                Name = name
                            };

                            // Parse attribute assignment operator
                            parser.MovePastWhitespace();
                            if (parser.Peek() == '=')
                            {
                                attribute.Mode = SelectorAttributeMode.Match;
                                parser.MoveAhead();
                            }
                            else if (parser.Peek() == ':' && parser.Peek(1) == '=')
                            {
                                attribute.Mode = SelectorAttributeMode.RegEx;
                                parser.MoveAhead(2);
                            }
                            else
                            {
                                attribute.Mode = SelectorAttributeMode.ExistsOnly;
                            }

                            // Parse attribute value
                            if (attribute.Mode != SelectorAttributeMode.ExistsOnly)
                            {
                                parser.MovePastWhitespace();
                                if (HtmlRules.IsQuoteChar(parser.Peek()))
                                    attribute.Value = parser.ParseQuotedText();
                                else
                                    attribute.Value = parser.ParseWhile(c => IsValueCharacter(c));
                            }

                            Selector selector = selectors.GetLast(true);
                            selector.Attributes.Add(attribute);
                        }

                        // Close out attribute selector
                        parser.MovePastWhitespace();
                        Debug.Assert(parser.Peek() == ']');
                        if (parser.Peek() == ']')
                            parser.MoveAhead();
                    }
                    else if (ch == ',')
                    {
                        // Multiple selectors
                        parser.MoveAhead();
                        parser.MovePastWhitespace();
                        selectors.Add(new Selector());
                    }
                    else if (ch == '>')
                    {
                        // Whitespace indicates child selector
                        parser.MoveAhead();
                        parser.MovePastWhitespace();
                        Debug.Assert(selectors.Any());
                        Selector selector = selectors.AddChildSelector();
                        selector.ImmediateChildOnly = true;
                    }
                    else if (char.IsWhiteSpace(ch))
                    {
                        // Handle whitespace
                        parser.MovePastWhitespace();
                        // ',' and '>' change meaning of whitespace
                        if (parser.Peek() != ',' && parser.Peek() != '>')
                            selectors.AddChildSelector();
                    }
                    else
                    {
                        // Unknown syntax
                        Debug.Assert(false);
                        parser.MoveAhead();
                    }
                }
            }
            selectors.RemoveEmpty();
            return selectors;
        }

        private static bool IsNameCharacter(char c) => char.IsLetterOrDigit(c);

        private static bool IsValueCharacter(char c) => char.IsLetterOrDigit(c) || c == '-';

        #endregion

    }
}
