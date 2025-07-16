// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Defines constants and rules that are used to parse and interpret
    /// HTML and XML.
    /// </summary>
    public class HtmlRules
    {

        #region Constant values

        public static readonly string HtmlHeaderTag = "!doctype";
        public static readonly string XmlHeaderTag = "?xml";

        public static readonly char TagStart = '<';
        public static readonly char TagEnd = '>';
        public static readonly char ForwardSlash = '/';

        public static readonly char DoubleQuote = '"';
        public static readonly char SingleQuote = '\'';

        public static List<CDataDefinition> CDataDefinitions =
        [
            new CDataDefinition
            {
                StartText = "<!--",
                EndText = "-->",
                StartComparison = StringComparison.Ordinal,
                EndComparison = StringComparison.Ordinal
            },
            new CDataDefinition
            {
                StartText = "<![CDATA[",
                EndText = "]]>",
                StartComparison = StringComparison.OrdinalIgnoreCase,
                EndComparison = StringComparison.Ordinal
            },
        ];

        public static readonly StringComparison TagStringComparison = StringComparison.CurrentCultureIgnoreCase;
        public static readonly StringComparer TagStringComparer = StringComparer.CurrentCultureIgnoreCase;

        #endregion

        #region String and character classification

        /// <summary>
        /// Returns true if <paramref name="c"/> is a single or double quote character.
        /// </summary>
        /// <param name="c">Character to test.</param>
        public static bool IsQuoteChar(char c) => c == DoubleQuote || c == SingleQuote;

        private static readonly HashSet<char> InvalidChars;

        static HtmlRules()
        {
            // Characters that are not valid within tag and attribute names (excluding whitespace and control characters)
            InvalidChars =
            [
                '!',
                '?',
                '<',
                '"',
                '\'',
                '>',
                '/',
                '='
            ];
            for (int i = 0xfdd0; i <= 0xfdef; i++)
                InvalidChars.Add((char)i);
            InvalidChars.Add('\ufffe');
            InvalidChars.Add('\uffff');
        }

        /// <summary>
        /// Returns true if <paramref name="c"/> is a valid tag name character.
        /// </summary>
        /// <param name="c">Character to test.</param>
        public static bool IsTagCharacter(char c)
        {
            return !InvalidChars.Contains(c) && !char.IsControl(c) && !char.IsWhiteSpace(c);
        }

        /// <summary>
        /// Returns true if <paramref name="c"/> is a valid attribue name character.
        /// </summary>
        /// <param name="c">Character to test.</param>
        public static bool IsAttributeNameCharacter(char c)
        {
            return !InvalidChars.Contains(c) && !char.IsControl(c) && !char.IsWhiteSpace(c);
        }

        /// <summary>
        /// Returns true if <paramref name="c"/> is a valid unquoted attribue value character.
        /// </summary>
        /// <param name="c">Character to test.</param>
        public static bool IsAttributeValueCharacter(char c)
        {
            return !InvalidChars.Contains(c) && !char.IsControl(c) && !char.IsWhiteSpace(c);
        }

        #endregion

        #region HTML tag rules

        /// <summary>
        /// Defines the current tag rules for HTML parsing.
        /// </summary>
        public static readonly HtmlTagRules TagRules = new();

        /// <summary>
        /// Returns the attributes of the specified HTML tag. If there are no attributes defined for the tag,
        /// <see cref="HtmlTagAttributes.None"/> is returned.
        /// </summary>
        /// <param name="tag">The name of the HTML tag.</param>
        public static HtmlTagAttributes GetTagAttributes(string tag) => TagRules.GetAttributes(tag);

        /// <summary>
        /// Determines whether the specified child tag is valid within the specified parent tag.
        /// </summary>
        /// <param name="parentTag">The parent HTML tag.</param>
        /// <param name="childTag">The child HTML tag.</param>
        /// <returns><see langword="true"/> if it is valid for <paramref name="parentTag"/> to contain <paramref name="childTag"/>;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool TagMayContain(string parentTag, string childTag) => TagRules.TagMayContain(parentTag, childTag);

        #endregion

    }
}
