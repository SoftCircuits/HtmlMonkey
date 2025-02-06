// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Defines a CDATA segment. These are segments that the library saves and stores,
    /// but does not parse the contents. Examples include comments, CDATA blocks and
    /// the content of tags with CData attribute.
    /// </summary>
    internal class CDataDefinition
    {
        /// <summary>
        /// Text that marks the start of the CData block. Must start with <see cref="HtmlRules.TagStart"/>
        /// in order for the HTML parser to recognize this segment.
        /// </summary>
        public string StartText { get; set; }

        /// <summary>
        /// Text that marks the end of the CData block.
        /// </summary>
        public string EndText { get; set; }

        /// <summary>
        /// Gets or sets the string comparison used to compare <see cref="StartText"/>.
        /// </summary>
        public StringComparison StartComparison { get; set; }

        /// <summary>
        /// Gets or sets the string comparison used to compare <see cref="EndText"/>.
        /// </summary>
        public StringComparison EndComparison { get; set; }

        public CDataDefinition()
        {
            StartText = EndText = string.Empty;
        }
    }
}
