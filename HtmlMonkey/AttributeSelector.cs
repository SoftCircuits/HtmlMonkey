// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Text.RegularExpressions;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Specifies the type of comparison to use on an attribute value.
    /// </summary>
    public enum SelectorAttributeMode
    {
        Match,
        RegEx,
        Contains,
        ExistsOnly,
    }

    [Obsolete("This class is deprecated and will be removed from a future version. Please use AttributeSelector instead.")]
    public class SelectorAttribute : AttributeSelector
    {
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
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of comparison that should be performed
        /// on the attribute value.
        /// </summary>
        public SelectorAttributeMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                switch (_mode)
                {
                    case SelectorAttributeMode.Match:
                    default:
                        NodeComparer = MatchComparer;
                        break;
                    case SelectorAttributeMode.RegEx:
                        NodeComparer = RegExComparer;
                        break;
                    case SelectorAttributeMode.Contains:
                        NodeComparer = ContainsComparer;
                        break;
                    case SelectorAttributeMode.ExistsOnly:
                        NodeComparer = ExistsOnlyComparer;
                        break;
                }
            }
        }
        private SelectorAttributeMode _mode;
        private Func<HtmlElementNode, bool> NodeComparer;

        private readonly StringComparison StringComparison;
        private readonly RegexOptions RegexOptions;

        /// <summary>
        /// Constructs a <see cref="AttributeSelector"></see> instance.
        /// </summary>
        /// <param name="ignoreCase">If <c>true</c>, node comparisons are not case-sensitive. If <c>false</c>,
        /// node comparisons are case-sensitive.</param>
        public AttributeSelector(bool ignoreCase = true)
        {
            Name = null;
            Value = null;
            Mode = SelectorAttributeMode.Match;
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
            
            HtmlAttribute attribute = node.Attributes[Name];
            return string.Equals(attribute?.Value, Value, StringComparison);
        }

        /// <summary>
        /// Implements <see cref="SelectorAttributeMode.RegEx"></see> comparer.
        /// </summary>
        private bool RegExComparer(HtmlElementNode node)
        {
            HtmlAttribute attribute = node.Attributes[Name];
            if (attribute != null)
                return Regex.IsMatch(attribute?.Value, Value, RegexOptions);
            return false;
        }

        /// <summary>
        /// Implements <see cref="SelectorAttributeMode.Contains"></see> comparer.
        /// </summary>
        private bool ContainsComparer(HtmlElementNode node)
        {
            HtmlAttribute attribute = node.Attributes[Name];
            if (attribute != null && attribute.Value != null)
            {
                string[] values = attribute.Value.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string value in values)
                {
                    if (string.Equals(Value, value, StringComparison))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Implements <see cref="SelectorAttributeMode.ExistsOnly"></see> comparer.
        /// </summary>
        private bool ExistsOnlyComparer(HtmlElementNode node) => node.Attributes[Name] != null;

        #endregion

        public override string ToString()
        {
            const string nullString = "(null)";
            return $"{Name ?? nullString}={Value ?? nullString}";
        }
    }
}
