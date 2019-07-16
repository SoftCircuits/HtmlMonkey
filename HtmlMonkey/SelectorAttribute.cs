// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Text.RegularExpressions;

namespace SoftCircuits.HtmlMonkey
{
    public enum SelectorAttributeMode
    {
        Match,
        RegEx,
        Contains,
        ExistsOnly,
    }

    public class SelectorAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }

        // Should probably expose this setting
        private bool IgnoreCase;
        private StringComparison StringComparison;

        public SelectorAttributeMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                switch (_mode)
                {
                    case SelectorAttributeMode.Match:
                        NodeComparer = IsMatchEqual;
                        break;
                    case SelectorAttributeMode.RegEx:
                        NodeComparer = IsMatchRegEx;
                        break;
                    case SelectorAttributeMode.Contains:
                        NodeComparer = IsMatchContains;
                        break;
                    case SelectorAttributeMode.ExistsOnly:
                    default:
                        NodeComparer = IsMatchExistsOnly;
                        break;
                }
            }
        }
        private SelectorAttributeMode _mode;
        private Func<HtmlElementNode, bool> NodeComparer;

        public SelectorAttribute()
        {
            Mode = SelectorAttributeMode.Match;
            Name = null;
            Value = null;

            IgnoreCase = true;
            StringComparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        }

        public bool IsMatch(HtmlElementNode node) => NodeComparer(node);

        #region Matching routines

        private bool IsMatchEqual(HtmlElementNode node)
        {
            HtmlAttribute attribute = node.Attributes[Name];
            return string.Equals(attribute?.Value, Value, StringComparison);
        }

        private bool IsMatchRegEx(HtmlElementNode node)
        {
            HtmlAttribute attribute = node.Attributes[Name];
            if (attribute != null)
                return Regex.IsMatch(attribute?.Value, Value, IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            return false;
        }

        private static readonly char[] Whitespace = new[] { ' ', '\t', '\r', '\n' };

        private bool IsMatchContains(HtmlElementNode node)
        {
            HtmlAttribute attribute = node.Attributes[Name];
            if (attribute != null && attribute.Value != null)
            {
                string[] values = attribute.Value.Split(Whitespace, StringSplitOptions.RemoveEmptyEntries);
                foreach (string value in values)
                {
                    if (string.Equals(Value, value, StringComparison))
                        return true;
                }
            }
            return false;
        }

        private bool IsMatchExistsOnly(HtmlElementNode node)
        {
            HtmlAttribute attribute = node.Attributes[Name];
            return attribute != null;
        }

        #endregion

        public override string ToString()
        {
            string name = Name ?? "(null)";
            return (Value != null) ?
                string.Format("{0}=\"{1}\"", name, Value) :
                name;
        }
    }
}
