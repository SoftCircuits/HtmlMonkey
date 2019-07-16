// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
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
        /// Adds an attribute to the collection. If the attribute already exists in the
        /// collection, the value of the existing attribute is updated.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(string key, HtmlAttribute value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            // Determine if we already have this attribute
            if (TryGetValue(key, out HtmlAttribute attribute))
                attribute.Value = value.Value;
            else
                base.Add(key, value);
        }

        /// <summary>
        /// Returns the <see cref="HtmlAttribute"/> with the given name. Unlike
        /// the standard <c>Dictionary</c>, this returns null rather than throwing
        /// an exception if the attribute does not exist.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new HtmlAttribute this[string key]
        {
            get => TryGetValue(key, out HtmlAttribute value) ? value : null;
            set => base[key] = value;
        }

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
