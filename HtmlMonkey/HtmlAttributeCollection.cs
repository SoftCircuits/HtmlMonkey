// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics.CodeAnalysis;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Class to represent a collection of element attributes.
    /// </summary>
    public class HtmlAttributeCollection : IEnumerable<HtmlAttribute>
    {
        // Internal attribute collection
        private readonly Dictionary<string, HtmlAttribute> Attributes;

        /// <summary>
        /// Constructs an <see cref="HtmlAttributeCollection"/> instance.
        /// </summary>
        public HtmlAttributeCollection()
        {
            Attributes = new Dictionary<string, HtmlAttribute>(HtmlRules.TagStringComparer);
        }

        /// <summary>
        /// Constructs an <see cref="HtmlAttributeCollection"/> instance.
        /// </summary>
        /// <param name="attributes">Attributes with which to prepopulate this
        /// collection.</param>
        public HtmlAttributeCollection(HtmlAttributeCollection attributes)
        {
            Attributes = new Dictionary<string, HtmlAttribute>(attributes.Attributes, HtmlRules.TagStringComparer);
        }

        /// <summary>
        /// Adds an <see cref="HtmlAttribute"/> to the collection. If the attribute already exists in the
        /// collection, the value of the existing attribute is updated.
        /// </summary>
        /// <param name="name">The name of the attribute to add.</param>
        /// <param name="value">The value of the attribute to add.</param>
        public void Add(string name, string? value) => Add(new HtmlAttribute(name, value));

        /// <summary>
        /// Adds an <see cref="HtmlAttribute"/> to the collection. If the attribute already exists, the
        /// value of the existing attribute is updated.
        /// </summary>
        /// <param name="attribute">The attribute to add.</param>
        public void Add(HtmlAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));
            if (string.IsNullOrEmpty(attribute.Name))
                throw new ArgumentException("Attribute name cannot be null or empty.");

            // Determine if we already have this attribute
            if (Attributes.TryGetValue(attribute.Name, out HtmlAttribute? existingAttribute))
                existingAttribute.Value = attribute.Value;
            else
                Attributes.Add(attribute.Name, attribute);
        }

        /// <summary>
        /// Returns the <see cref="HtmlAttribute"/> with the given name. This property
        /// returns null rather than throwing an exception when the attribute does not
        /// exist.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <returns>Returns the <see cref="HtmlAttribute"/> with the given name.</returns>
        public HtmlAttribute? this[string? name]
        {
            get
            {
                if (name != null && Attributes.TryGetValue(name, out HtmlAttribute? value))
                    return value;
                return null;
            }
            set
            {
                if (name != null)
                {
                    if (value == null)
                        Attributes.Remove(name);    // TODO: ???
                    else
                        Attributes[name] = value;
                }


                    //Attributes[name] = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="HtmlAttribute"/> with the given name. Returns <c>true</c>
        /// if successful, or <c>false</c> if no matching attribute was found.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Returns the attribute with the specified name, if successful.</param>
        /// <returns>True if successful, false if no matching attribute was found.</returns>
        public bool TryGetValue(string name, out HtmlAttribute? value) => Attributes.TryGetValue(name, out value);

        /// <summary>
        /// Converts this <see cref="HtmlAttributeCollection"></see> to a string.
        /// </summary>
        public override string ToString() => Attributes.Any() ? $" {string.Join(" ", this)}" : string.Empty;

        /// <summary>
        /// Gets the number of <see cref="HtmlAttribute"/>s in this collection.
        /// </summary>
        public int Count => Attributes.Count;

        /// <summary>
        /// Gets an enumerable on the attribute names.
        /// </summary>
        public IEnumerable<string?> Names => Attributes.Values.Select(a => a.Name);

        /// <summary>
        /// Gets an enumerable on the attribute values.
        /// </summary>
        public IEnumerable<string?> Values => Attributes.Values.Select(a => a.Value);

        #region IEnumerable

        /// <summary>
        /// Gets an enumerator that iterates through the <see cref="HtmlAttribute"/>
        /// collection.
        /// </summary>
        public IEnumerator<HtmlAttribute> GetEnumerator()
        {
            return Attributes.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through the <see cref="HtmlAttribute"/>
        /// collection.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Attributes.Values.GetEnumerator();
        }

        #endregion

    }
}
