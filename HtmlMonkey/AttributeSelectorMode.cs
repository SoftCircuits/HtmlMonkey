// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.HtmlMonkey
{
    [Obsolete("This enum has been deprecated and will be removed in a future version. Please use AttributeSelectorMode instead.")]
    public enum SelectorAttributeMode { Match, RegEx, Contains, ExistsOnly }

    /// <summary>
    /// Specifies the type of comparison to use on an attribute selector.
    /// </summary>
    public enum AttributeSelectorMode
    {
        /// <summary>
        /// Matches by comparing the attribute value to a string.
        /// </summary>
        Match,

        /// <summary>
        /// Matches by comparing the attribute value to a regular expression.
        /// </summary>
        RegEx,

        /// <summary>
        /// Matches if any word in the attribute value matches a string.
        /// </summary>
        Contains,

        /// <summary>
        /// Matches if the attribute exists, regardless of the attribute value.
        /// </summary>
        ExistsOnly,
    }
}
