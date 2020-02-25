// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Dictionary class to represent a collection of element attributes.
    /// Keys are not case-sensitive.
    /// </summary>
    public class HtmlAttributeCollection : Dictionary<string, HtmlAttribute>
    {
        public HtmlAttributeCollection()
            : base(HtmlRules.TagStringComparer)
        {
        }

        public HtmlAttributeCollection(HtmlAttributeCollection attributes)
            : base(attributes, HtmlRules.TagStringComparer)
        {
        }

        /// <summary>
        /// Adds an <see cref="HtmlAttribute"></see> to the collection. If the
        /// attribute already exists in the collection, the value of the existing
        /// attribute is updated.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Attribute value.</param>
        public new void Add(string name, HtmlAttribute value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            // Determine if we already have this attribute
            if (TryGetValue(name, out HtmlAttribute attribute))
                attribute.Value = value.Value;
            else
                base.Add(name, value);
        }

        /// <summary>
        /// Returns the <see cref="HtmlAttribute"/> with the given name. Unlike the
        /// standard <see cref="Dictionary{TKey, TValue}"></see>, this property
        /// returns null rather than throwing an exception when the attribute does not
        /// exist.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <returns>Returns the <see cref="HtmlAttribute"/> with the given name.</returns>
        public new HtmlAttribute this[string name]
        {
            get => TryGetValue(name, out HtmlAttribute value) ? value : null;
            set => base[name] = value;
        }

        /// <summary>
        /// Converts this <see cref="HtmlAttributeCollection"></see> to a string.
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (HtmlAttribute attribute in Values)
            {
                builder.Append(' ');
                builder.Append(attribute.ToString());
            }
            return builder.ToString();
        }
    }
}
