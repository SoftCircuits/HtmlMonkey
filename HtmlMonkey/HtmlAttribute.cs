// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Class that represents a single element attribute.
    /// </summary>
    public class HtmlAttribute
    {
        private const string Null = "(null)";

        /// <summary>
        /// Attribute name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Attribute value. Will be an empty string if the attribute has an empty
        /// value. Will be null if the attribute name was not followed by an equal
        /// sign.
        /// </summary>
        public string Value { get; set; }

        public override string ToString()
        {
            return (Value != null) ?
                $"{Name ?? Null}=\"{Value}\"" :
                Name ?? Null;
        }
    }
}
