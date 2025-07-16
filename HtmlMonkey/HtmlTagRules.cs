// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections.Generic;
using System.Linq;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Defines rules for HTML tags. This includes tag attributes, and tag nesting rules.
    /// </summary>
    public class HtmlTagRules
    {
        /// <summary>
        /// Defines the attributes for HTML tags.
        /// </summary>
        private readonly Dictionary<string, HtmlTagAttributes> Attributes;

        /// <summary>
        /// Defines the valid tags which which tags can be nested within which parent tags.
        /// This is a dictionary of tag names and the corresponding parent tags that are valid.
        /// If a tag is in this collection, that tag is only allowed within one of the specified
        /// parent tags. If a tag is not in this collection, there are no restrictions on which
        /// tags can contain this tag (aside from any behaviors defined in <see cref="Attributes"/>).
        /// </summary>
        private readonly Dictionary<string, HashSet<string>> NestingRules;

        /// <summary>
        /// Constructs a new instance of <see cref="HtmlTagRules"/>.
        /// </summary>
        public HtmlTagRules()
        {
            // TODO: For now, we prepopulate common attributes for HTML. Is this what we want?
            Attributes = new(HtmlRules.TagStringComparer)
            {
                ["!doctype"] = HtmlTagAttributes.HtmlHeader | HtmlTagAttributes.NoChildren,
                ["?xml"] = HtmlTagAttributes.XmlHeader | HtmlTagAttributes.NoChildren,
                ["a"] = HtmlTagAttributes.NoNested,
                ["area"] = HtmlTagAttributes.NoChildren,
                ["base"] = HtmlTagAttributes.NoChildren,
                ["basefont"] = HtmlTagAttributes.NoChildren,
                ["bgsound"] = HtmlTagAttributes.NoChildren,
                ["br"] = HtmlTagAttributes.NoChildren,
                ["col"] = HtmlTagAttributes.NoChildren,
                ["dd"] = HtmlTagAttributes.NoNested,
                ["dt"] = HtmlTagAttributes.NoNested,
                ["embed"] = HtmlTagAttributes.NoChildren,
                ["frame"] = HtmlTagAttributes.NoChildren,
                ["hr"] = HtmlTagAttributes.NoChildren,
                ["img"] = HtmlTagAttributes.NoChildren,
                ["input"] = HtmlTagAttributes.NoChildren,
                ["isindex"] = HtmlTagAttributes.NoChildren,
                ["keygen"] = HtmlTagAttributes.NoChildren,
                ["li"] = HtmlTagAttributes.NoNested,
                ["link"] = HtmlTagAttributes.NoChildren,
                ["menuitem"] = HtmlTagAttributes.NoChildren,
                ["meta"] = HtmlTagAttributes.NoChildren,
                ["noxhtml"] = HtmlTagAttributes.CData,
                ["p"] = HtmlTagAttributes.NoNested,
                ["param"] = HtmlTagAttributes.NoChildren,
                ["script"] = HtmlTagAttributes.CData,
                ["select"] = HtmlTagAttributes.NoSelfClosing,
                ["source"] = HtmlTagAttributes.NoChildren,
                ["spacer"] = HtmlTagAttributes.NoChildren,
                ["style"] = HtmlTagAttributes.CData,
                ["table"] = HtmlTagAttributes.NoNested,
                ["td"] = HtmlTagAttributes.NoNested,
                ["th"] = HtmlTagAttributes.NoNested,
                ["textarea"] = HtmlTagAttributes.NoSelfClosing,
                ["track"] = HtmlTagAttributes.NoChildren,
                ["wbr"] = HtmlTagAttributes.NoChildren,
            };
            NestingRules = new(HtmlRules.TagStringComparer);
            // TODO: For now, we don't prepopulate any nesting rules. Should we?
            //ClearNestingRules();
            //SetNestingRule("html", []);
            //SetNestingRule("head", ["html"]);
            //SetNestingRule("body", ["html"]);
            //SetNestingRule("thead", ["table"]);
            //SetNestingRule("tbody", ["table"]);
            //SetNestingRule("tfoot", ["table"]);
            //SetNestingRule("tr", ["table", "thead", "tbody"]);
            //SetNestingRule("td", ["tr"]);
            //SetNestingRule("th", ["tr"]);
            //SetNestingRule("li", ["ol", "ul"]);
            //SetNestingRule("option", ["select", "optgroup"]);
            //SetNestingRule("optgroup", ["select"]);
            //SetNestingRule("dt", ["dl"]);
            //SetNestingRule("dd", ["dl"]);
        }

        #region Attributes

        /// <summary>
        /// Sets the attributes for the specified HTML tag. Overwrites any existing attributes for that tag.
        /// </summary>
        /// <param name="tag">The name of the HTML tag.</param>
        /// <param name="attributes">The attributes to set.</param>
        public void SetAttributes(string tag, HtmlTagAttributes attributes)
        {
            if (attributes != HtmlTagAttributes.None)
                Attributes[tag] = attributes;
            else
                Attributes.Remove(tag);
        }

        /// <summary>
        /// Returns the attributes for the specified tag. Returns <see cref="HtmlTagAttributes.None"/> if there
        /// is no entry for the specified tag.
        /// </summary>
        /// <param name="tag">The tag name.</param>
        public HtmlTagAttributes GetAttributes(string tag) => Attributes.TryGetValue(tag, out HtmlTagAttributes attributes) ?
            attributes :
            HtmlTagAttributes.None;

        /// <summary>
        /// Retrieves a snapshot of all tag attributes as a collection of key-value pairs.
        /// </summary>
        public IEnumerable<KeyValuePair<string, HtmlTagAttributes>> GetAllAttributes() => Attributes.ToList();

        /// <summary>
        /// Removes all tag attributes rules.
        /// </summary>
        public void ClearAttributes() => Attributes.Clear();

        /// <summary>
        /// Removes the attributes for the specified HTML tag.
        /// </summary>
        /// <param name="tag">The name of the HTML tag to remove.</param>
        /// <returns>True if the nesting rule was found and removed; otherwise, false.</returns>
        public bool RemoveAttributes(string tag) => Attributes.Remove(tag);

        #endregion

        #region Nesting Rules

        /// <summary>
        /// Sets the nesting rule for the specified HTML tag. Overwrites any existing nesting rule for that tag.
        /// </summary>
        /// <param name="tag">The name of the HTML tag.</param>
        /// <param name="parentTags">The nesting rules to set. This is a list of tag names that are valid as
        /// a parent tag of the tag being set. If this list is empty, the tag is not valid within any parent tags.
        /// If this list is null, the tag has no nesting rules and there will be no restrictions on which tags
        /// this tag can be a child of.</param>
        public void SetNestingRule(string tag, IEnumerable<string>? parentTags)
        {
            if (parentTags != null)
                NestingRules[tag] = new HashSet<string>(parentTags, HtmlRules.TagStringComparer);
            else
                NestingRules.Remove(tag);
        }

        /// <summary>
        /// Returns the nesting rules for the specified HTML tag. This is a list of tag names that are valid as
        /// a parent tag of the specified tag. If this list is empty, the tag is not valid within any parent tags.
        /// If this list is null, the tag has no nesting rules and there will be no restrictions on which tags
        /// this tag can be a child of.
        /// </summary>
        /// <param name="tag">The tag name.</param>
        public HashSet<string>? GetNestingRule(string tag) => NestingRules.TryGetValue(tag, out HashSet<string>? parentTags) ?
            parentTags :
            null;

        /// <summary>
        /// Retrieves a snapshot of all tag nesting rules as a collection of key-value pairs.
        /// </summary>
        public IEnumerable<KeyValuePair<string, HashSet<string>>> GetAllNestingRules() => NestingRules.ToList();

        /// <summary>
        /// Removes all tag nesting rules.
        /// </summary>
        public void ClearNestingRules() => NestingRules.Clear();

        /// <summary>
        /// Removes the nesting rule for the specified HTML tag.
        /// </summary>
        /// <param name="tag">The name of the HTML tag to remove.</param>
        /// <returns>True if the nesting rule was found and removed; otherwise, false.</returns>
        public bool RemoveNestingRule(string tag) => NestingRules.Remove(tag);

        #endregion

        /// <summary>
        /// Determines whether the specified child tag is valid within the specified parent tag.
        /// </summary>
        /// <remarks>This method looks at both <see cref="Attributes"/> and <see cref="NestingRules"/>.</remarks>
        /// <param name="parentTag">The parent HTML tag.</param>
        /// <param name="childTag">The child HTML tag.</param>
        /// <returns><see langword="true"/> if it is valid for <paramref name="parentTag"/> to contain <paramref name="childTag"/>;
        /// otherwise, <see langword="false"/>.</returns>
        public bool TagMayContain(string parentTag, string childTag)
        {
            HtmlTagAttributes parentFlags = GetAttributes(parentTag);

            if (parentFlags.HasFlag(HtmlTagAttributes.NoChildren))
                return false;

            if (parentFlags.HasFlag(HtmlTagAttributes.NoNested) && parentTag.Equals(childTag, HtmlRules.TagStringComparison))
                return false;

            if (NestingRules.TryGetValue(childTag, out HashSet<string>? parents))
                return parents.Contains(parentTag);

            return true;
        }
    }
}
