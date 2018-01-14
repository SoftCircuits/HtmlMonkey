using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlMonkey
{
    /// <summary>
    /// Defines element tag attributes.
    /// </summary>
    [Flags]
    internal enum HtmlTagFlag
    {
        /// <summary>
        /// Specifies no flags.
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// Is an HTML DOCTYPE document header tag
        /// </summary>
        HtmlHeader = 0x0001,
        /// <summary>
        /// Is an XML document header tag
        /// </summary>
        XmlHeader = 0x0002,
        /// <summary>
        /// Element cannot contain child nodes.
        /// </summary>
        NoChildren = 0x0004,
        /// <summary>
        /// Element cannot contain element of same type
        /// </summary>
        NoNested = 0x0008,
        /// <summary>
        /// Element cannot be self-closing.
        /// </summary>
        NoSelfClosing = 0x0010,
        /// <summary>
        /// Element content is saved but not parsed, and may contain anything.
        /// </summary>
        CData = 0x0020,
    }

    /// <summary>
    /// Defines a CDATA segment. These are segments that we save and store,
    /// but we do not parse the contents. Examples include comments, CDATA
    /// and the content of tags with CData attribute.
    /// </summary>
    /// <remarks>
    /// For all entries, the StartText member must start with HtmlRules.TagStart
    /// or else the parser can miss the segment.
    /// </remarks>
    internal class CDataDefinition
    {
        public string StartText { get; set; }
        public string EndText { get; set; }

        /// <summary>
        /// Should be true if StartText contains letters that require a
        /// case-insensitive comparison.
        /// </summary>
        public bool IgnoreCase { get; set; }
    }

    /// <summary>
    /// Defines constants and rules that are used to parse and interpret
    /// HTML and XML.
    /// </summary>
    internal class HtmlRules
    {

        #region Constant values

        public static readonly string HtmlHeaderTag = "!doctype";
        public static readonly string XmlHeaderTag = "?xml";

        public static readonly char TagStart = '<';
        public static readonly char TagEnd = '>';
        public static readonly char ForwardSlash = '/';

        public static readonly char DoubleQuote = '"';
        public static readonly char SingleQuote = '\'';

        public static List<CDataDefinition> CDataDefinitions = new List<CDataDefinition>
        {
            new CDataDefinition { StartText = "<!--", EndText = "-->", IgnoreCase = false },
            new CDataDefinition { StartText = "<![CDATA[", EndText = "]]>", IgnoreCase = true },
        };

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
            InvalidChars = new HashSet<char>();

            InvalidChars.Add('-');
            InvalidChars.Add('<');

            InvalidChars.Add(' ');
            InvalidChars.Add('"');
            InvalidChars.Add('\'');
            InvalidChars.Add('>');
            InvalidChars.Add('/');
            InvalidChars.Add('=');
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
        /// Returns true if <paramref name="c"/> is a valid attribue value character.
        /// Valid only for unquoted values. Quoted values allow additional characters.
        /// </summary>
        /// <param name="c">Character to test.</param>
        public static bool IsAttributeValueCharacter(char c)
        {
            return !InvalidChars.Contains(c) && !char.IsControl(c) && !char.IsWhiteSpace(c);
        }

        #endregion

        #region Tag classification

        /// <summary>
        /// Defines tag attributes for element tags.
        /// </summary>
        private static Dictionary<string, HtmlTagFlag> TagRules = new Dictionary<string, HtmlTagFlag>(StringComparer.CurrentCultureIgnoreCase)
        {
            ["!doctype"] = HtmlTagFlag.HtmlHeader,
            ["?xml"] = HtmlTagFlag.XmlHeader,
            ["a"] = HtmlTagFlag.NoNested,
            ["area"] = HtmlTagFlag.NoChildren,
            ["base"] = HtmlTagFlag.NoChildren,
            ["basefont"] = HtmlTagFlag.NoChildren,
            ["bgsound"] = HtmlTagFlag.NoChildren,
            ["br"] = HtmlTagFlag.NoChildren,
            ["col"] = HtmlTagFlag.NoChildren,
            ["dd"] = HtmlTagFlag.NoNested,
            ["dt"] = HtmlTagFlag.NoNested,
            ["embed"] = HtmlTagFlag.NoChildren,
            ["frame"] = HtmlTagFlag.NoChildren,
            ["hr"] = HtmlTagFlag.NoChildren,
            ["img"] = HtmlTagFlag.NoChildren,
            ["input"] = HtmlTagFlag.NoChildren,
            ["isindex"] = HtmlTagFlag.NoChildren,
            ["keygen"] = HtmlTagFlag.NoChildren,
            ["li"] = HtmlTagFlag.NoNested,
            ["link"] = HtmlTagFlag.NoChildren,
            ["menuitem"] = HtmlTagFlag.NoChildren,
            ["meta"] = HtmlTagFlag.NoChildren,
            ["noxhtml"] = HtmlTagFlag.CData,
            ["p"] = HtmlTagFlag.NoNested,
            ["param"] = HtmlTagFlag.NoChildren,
            ["script"] = HtmlTagFlag.CData,
            ["source"] = HtmlTagFlag.NoChildren,
            ["spacer"] = HtmlTagFlag.NoChildren,
            ["style"] = HtmlTagFlag.CData,
            ["table"] = HtmlTagFlag.NoNested,
            ["td"] = HtmlTagFlag.NoNested,
            ["th"] = HtmlTagFlag.NoNested,
            ["textarea"] = HtmlTagFlag.NoSelfClosing,
            ["track"] = HtmlTagFlag.NoChildren,
            ["wbr"] = HtmlTagFlag.NoChildren,
        };

        /// <summary>
        /// Returns the attribute flags for the given tag.
        /// </summary>
        public static HtmlTagFlag GetTagFlags(string tag)
        {
            if (TagRules.TryGetValue(tag, out HtmlTagFlag flags))
                return flags;
            return HtmlTagFlag.None;
        }

        /// <summary>
        /// Defines element tag priorities. Used to help resolve mismatched tags.
        /// </summary>
        private static Dictionary<string, int> TagPriority = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase)
        {
            ["div"] = 150,
            ["td"] = 160,
            ["th"] = 160,
            ["tr"] = 170,
            ["thead"] = 180,
            ["tbody"] = 180,
            ["tfoot"] = 180,
            ["table"] = 190,
            ["head"] = 200,
            ["body"] = 200,
            ["html"] = 220,
        };

        /// <summary>
        /// Returns a value that signifies the relative priority of the specified tag.
        /// </summary>
        public static int GetTagPriority(string tag)
        {
            return (TagPriority.TryGetValue(tag, out int priority)) ? priority : 100;
        }

        #endregion

        #region Tag containership logic

        /// <summary>
        /// Returns true if it is considered valid for the given parent tag to contain the
        /// given child tag.
        /// </summary>
        public static bool TagMayContain(string parentTag, string childTag)
        {
            return TagMayContain(parentTag, childTag, GetTagFlags(parentTag));
        }

        /// <summary>
        /// Returns true if it is considered valid for the given parent tag to contain the
        /// given child tag. Provide the parent flags, if available, to improve performance.
        /// </summary>
        public static bool TagMayContain(string parentTag, string childTag, HtmlTagFlag parentFlags)
        {
            if (parentFlags.HasFlag(HtmlTagFlag.NoChildren))
                return false;
            if (parentFlags.HasFlag(HtmlTagFlag.NoNested) && parentTag.Equals(childTag, StringComparison.OrdinalIgnoreCase))
                return false;
            // Attempt to catch some obviously invalid HTML structure
            if (GetTagPriority(childTag) > GetTagPriority(parentTag))
                return false;
            return true;
        }

        #endregion

    }
}
