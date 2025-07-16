// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Defines the attributes of an HTML tag (element).
    /// </summary>
    [Flags]
    public enum HtmlTagAttributes
    {
        /// <summary>
        /// Specifies no attributes.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Is an HTML DOCTYPE document header tag.
        /// </summary>
        HtmlHeader = 0x0001,

        /// <summary>
        /// Is an XML document header tag.
        /// </summary>
        XmlHeader = 0x0002,

        /// <summary>
        /// Element cannot contain child nodes.
        /// </summary>
        NoChildren = 0x0004,

        /// <summary>
        /// Element cannot contain element of same type.
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

}
