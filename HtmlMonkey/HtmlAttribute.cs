// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Class that represents a single element attribute.
    /// </summary>
    public class HtmlAttribute
    {
        /// <summary>
        /// Gets or sets the name of this attribute.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of this attribute. The library sets this value to
        /// an empty string if the attribute has an empty value. It sets it to null
        /// if the attribute name was not followed by an equal sign.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Constructs an <see cref="HtmlAttribute"/> instance.
        /// </summary>
        public HtmlAttribute()
        {
            Name = string.Empty;
            Value = null;
        }

        /// <summary>
        /// Constructs an <see cref="HtmlAttribute"/> instance.
        /// </summary>
        /// <param name="name">Name of this attribute.</param>
        /// <param name="value">Value of this attribute. The library sets this value
        /// to an empty string if the attribute has an empty value. It sets it to
        /// null if the attribute name was not followed by an equal sign.</param>
        public HtmlAttribute(string name, string? value = null)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Converts this <see cref="HtmlAttribute"></see> to a string.
        /// </summary>
        public override string ToString()
        {
            string attribute = Name ?? "(null)";
            if (Value != null)
                attribute += $"=\"{Value}\"";
            return attribute;
        }
    }
}
