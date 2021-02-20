// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
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
        Match,
        RegEx,
        Contains,
        ExistsOnly,
    }
}
