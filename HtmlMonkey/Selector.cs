// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Describes a selector used for finding elements.
    /// </summary>
    public class Selector
    {
        /// <summary>
        /// Gets or sets the tag name. Set to <c>null</c> or empty string to
        /// match all tags.
        /// </summary>
        public string? Tag { get; set; }

        /// <summary>
        /// Gets this selector's attribute selectors.
        /// </summary>
        public List<AttributeSelector> Attributes { get; private set; }

        /// <summary>
        /// Gets or sets this selector's child selector.
        /// </summary>
        public Selector? ChildSelector { get; set; }

        /// <summary>
        /// Gets or sets whether this selector applies only to immediate children
        /// (one level down from parent).
        /// </summary>
        public bool ImmediateChildOnly { get; set; }

        /// <summary>
        /// Constructs a new <see cref="Selector"></see> instance.
        /// </summary>
        public Selector()
        {
            Tag = null;
            Attributes = new List<AttributeSelector>();
            ChildSelector = null;
        }

        /// <summary>
        /// Returns true if selector has no data. Child selectors are not included in this
        /// evaluation.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(Tag) && !Attributes.Any();

        /// <summary>
        /// Returns true if this selector matches the specified <see cref="HtmlElementNode"/>.
        /// </summary>
        public bool IsMatch(HtmlElementNode node)
        {
            // Compare tag
            if (!string.IsNullOrWhiteSpace(Tag) && !string.Equals(Tag, node.TagName, HtmlRules.TagStringComparison))
                return false;

            // Compare attributes
            foreach (AttributeSelector selector in Attributes)
            {
                if (!selector.IsMatch(node))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Recursively searches the given root node and returns the nodes that match this
        /// selector.
        /// </summary>
        /// <param name="rootNode">Root node to search.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> Find(HtmlNode rootNode) => Find(new[] { rootNode });

        /// <summary>
        /// Recursively searches the list of nodes and returns the nodes that match this
        /// selector.
        /// </summary>
        /// <param name="nodes">Nodes to search.</param>
        /// <returns>The matching nodes.</returns>
        public IEnumerable<HtmlElementNode> Find(IEnumerable<HtmlNode> nodes)
        {
            List<HtmlElementNode>? results = null;
            bool matchTopLevelNodes = true;

            // Search from this selector on down through its child selectors
            for (Selector? selector = this; selector != null; selector = selector.ChildSelector)
            {
                results = new List<HtmlElementNode>();
                FindRecursive(nodes, selector, matchTopLevelNodes, true, results);
                // In next iteration, apply nodes that matched this iteration
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

        public override string ToString() => Tag ?? "(null)";

        #region Parsing

        private readonly static Dictionary<char, string> SpecialCharacters = new()
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
        public static SelectorCollection ParseSelector(string? selectorText)
        {
            SelectorCollection selectors = new();

            if (!string.IsNullOrWhiteSpace(selectorText))
            {
                TextParser parser = new(selectorText);
                parser.SkipWhiteSpace();

                while (!parser.EndOfText)
                {
                    // Test next character
                    char ch = parser.Peek();
                    if (IsNameCharacter(ch) || ch == '*')
                    {
                        // Parse tag name
                        Selector selector = selectors.GetLastSelector();
                        if (ch == '*')
                            selector.Tag = null;    // Match all tags
                        else
                            selector.Tag = parser.ParseWhile(IsNameCharacter);
                    }
                    else if (SpecialCharacters.TryGetValue(ch, out string? name))
                    {
                        // Parse special attributes
                        parser.Next();
                        string value = parser.ParseWhile(IsValueCharacter);
                        if (value.Length > 0)
                        {
                            AttributeSelector attribute = new(name, value)
                            {
                                Mode = AttributeSelectorMode.Contains
                            };

                            Selector selector = selectors.GetLastSelector();
                            selector.Attributes.Add(attribute);
                        }
                    }
                    else if (ch == '[')
                    {
                        // Parse attribute selector
                        parser.Next();
                        parser.SkipWhiteSpace();
                        name = parser.ParseWhile(IsNameCharacter);
                        if (name.Length > 0)
                        {
                            AttributeSelector attribute = new(name);

                            // Parse attribute assignment operator
                            parser.SkipWhiteSpace();
                            if (parser.Peek() == '=')
                            {
                                attribute.Mode = AttributeSelectorMode.Match;
                                parser.Next();
                            }
                            else if (parser.Peek() == ':' && parser.Peek(1) == '=')
                            {
                                attribute.Mode = AttributeSelectorMode.RegEx;
                                parser.Index += 2;
                            }
                            else
                            {
                                attribute.Mode = AttributeSelectorMode.ExistsOnly;
                            }

                            // Parse attribute value
                            if (attribute.Mode != AttributeSelectorMode.ExistsOnly)
                            {
                                parser.SkipWhiteSpace();
                                if (HtmlRules.IsQuoteChar(parser.Peek()))
                                    attribute.Value = parser.ParseQuotedText();
                                else
                                    attribute.Value = parser.ParseWhile(IsValueCharacter);
                            }

                            Selector selector = selectors.GetLastSelector();
                            selector.Attributes.Add(attribute);
                        }

                        // Close out attribute selector
                        parser.SkipWhiteSpace();
                        Debug.Assert(parser.Peek() == ']');
                        if (parser.Peek() == ']')
                            parser.Next();
                    }
                    else if (ch == ',')
                    {
                        // Multiple selectors
                        parser.Next();
                        parser.SkipWhiteSpace();
                        selectors.Add(new Selector());
                    }
                    else if (ch == '>')
                    {
                        // Whitespace indicates child selector
                        parser.Next();
                        parser.SkipWhiteSpace();
                        Debug.Assert(selectors.Any());
                        Selector selector = selectors.AddChildSelector();
                        selector.ImmediateChildOnly = true;
                    }
                    else if (char.IsWhiteSpace(ch))
                    {
                        // Handle whitespace
                        parser.SkipWhiteSpace();
                        // ',' and '>' change meaning of whitespace
                        if (parser.Peek() != ',' && parser.Peek() != '>')
                            selectors.AddChildSelector();
                    }
                    else
                    {
                        // Unknown syntax
                        Debug.Assert(false);
                        parser.Next();
                    }
                }
            }
            selectors.RemoveEmptySelectors();
            return selectors;
        }

        private static bool IsNameCharacter(char c) => char.IsLetterOrDigit(c) || c == '-';

        private static bool IsValueCharacter(char c) => IsNameCharacter(c);

        #endregion

    }
}
