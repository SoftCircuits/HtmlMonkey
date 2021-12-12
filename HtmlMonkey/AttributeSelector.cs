// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SoftCircuits.HtmlMonkey
{
    [Obsolete("This class is deprecated and will be removed from a future version. Please use AttributeSelector instead.")]
    public class SelectorAttribute : AttributeSelector
    {
        public SelectorAttribute(string name, string? value = null, bool ignoreCase = true)
            : base(name, value, ignoreCase)
        {

        }
    }

    /// <summary>
    /// Defines a selector that describes a node attribute.
    /// </summary>
    public class AttributeSelector
    {
        /// <summary>
        /// Gets or sets the name of the attribute to be compared.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value the attribute should be compared to.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the type of comparison that should be performed
        /// on the attribute value.
        /// </summary>
        public AttributeSelectorMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                NodeComparer = _mode switch
                {
                    AttributeSelectorMode.RegEx => RegExComparer,
                    AttributeSelectorMode.Contains => ContainsComparer,
                    AttributeSelectorMode.ExistsOnly => ExistsOnlyComparer,
                    _ => MatchComparer,
                };
            }
        }
        private AttributeSelectorMode _mode;
        private Func<HtmlElementNode, bool> NodeComparer;

        private readonly StringComparison StringComparison;
        private readonly RegexOptions RegexOptions;

        /// <summary>
        /// Constructs a <see cref="AttributeSelector"></see> instance.
        /// </summary>
        /// <param name="ignoreCase">If <c>true</c>, node comparisons are not case-sensitive. If <c>false</c>,
        /// node comparisons are case-sensitive.</param>
        public AttributeSelector(string name, string? value = null, bool ignoreCase = true)
        {
            Name = name;
            Value = value;
            Mode = AttributeSelectorMode.Match;
            NodeComparer = MatchComparer;
            StringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            RegexOptions = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
        }

        #region Matching routines

        /// <summary>
        /// Compares the given node against this selector attribute.
        /// </summary>
        /// <param name="node">Node to be compared.</param>
        /// <returns>True if the node matches, false otherwise.</returns>
        public bool IsMatch(HtmlElementNode node) => NodeComparer(node);

        /// <summary>
        /// Implements <see cref="SelectorAttributeMode.Match"></see> comparer.
        /// </summary>
        private bool MatchComparer(HtmlElementNode node)
        {
            if (Value != null)
            {
                string? attributeValue = node.Attributes[Name]?.Value;
                if (attributeValue != null)
                    return string.Equals(attributeValue, Value, StringComparison);
            }
            return false;
        }

        /// <summary>
        /// Implements <see cref="SelectorAttributeMode.RegEx"></see> comparer.
        /// </summary>
        private bool RegExComparer(HtmlElementNode node)
        {
            if (Value != null)
            {
                string? attributeValue = node.Attributes[Name]?.Value;
                if (attributeValue != null)
                    return Regex.IsMatch(attributeValue, Value, RegexOptions);
            }
            return false;
        }

        /// <summary>
        /// Implements <see cref="SelectorAttributeMode.Contains"></see> comparer.
        /// </summary>
        private bool ContainsComparer(HtmlElementNode node)
        {
            if (Value != null)
            {
                string? attributeValue = node.Attributes[Name]?.Value;
                if (attributeValue != null)
                {
                    foreach (string value in ParseWords(attributeValue))
                    {
                        if (string.Equals(Value, value, StringComparison))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Implements <see cref="SelectorAttributeMode.ExistsOnly"></see> comparer.
        /// </summary>
        private bool ExistsOnlyComparer(HtmlElementNode node) => node.Attributes[Name] != null;

        private static IEnumerable<string> ParseWords(string s)
        {
            bool inWord = false;
            int wordStart = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsWhiteSpace(s[i]))
                {
                    if (inWord)
                    {
                        inWord = false;
#if !NETSTANDARD2_0
                        yield return s[wordStart..i];
#else
                        yield return s.Substring(wordStart, i - wordStart);
#endif
                    }
                }
                else if (!inWord)
                {
                    inWord = true;
                    wordStart = i;
                }
            }

            // Check for last word
            if (inWord)
#if !NETSTANDARD2_0
                yield return s[wordStart..];
#else
                yield return s.Substring(wordStart);
#endif
        }

        #endregion

        public override string ToString()
        {
            const string nullString = "(null)";
            return $"{Name ?? nullString}={Value ?? nullString}";
        }
    }
}
