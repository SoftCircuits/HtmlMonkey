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
        /// <summary>
        /// Constructs an <see cref="HtmlAttributeCollection"/> instance.
        /// </summary>
        public HtmlAttributeCollection()
            : base(HtmlRules.TagStringComparer)
        {
        }

        /// <summary>
        /// Constructs an <see cref="HtmlAttributeCollection"/> instance.
        /// </summary>
        /// <param name="attributes">Attributes with which to prepopulate this
        /// collection.</param>
        public HtmlAttributeCollection(HtmlAttributeCollection attributes)
            : base(attributes, HtmlRules.TagStringComparer)
        {
        }

        /// <summary>
        /// Adds an <see cref="HtmlAttribute"></see> to the collection. If the
        /// attribute already exists in the collection, the value of the existing
        /// attribute is updated.
        /// </summary>
        /// <param name="attribute">Attribute to be added.</param>
        public void Add(HtmlAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));
            if (string.IsNullOrEmpty(attribute.Name))
                throw new ArgumentException("Attribute name cannot be null or empty.");

            // Determine if we already have this attribute
            if (TryGetValue(attribute.Name, out HtmlAttribute existingAttribute))
                existingAttribute.Value = attribute.Value;
            else
                Add(attribute.Name, attribute);
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
