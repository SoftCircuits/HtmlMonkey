using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlMonkey
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

        public new HtmlAttribute this[string key]
        {
            get
            {
                return TryGetValue(key, out HtmlAttribute value) ? value : null;
            }
            set
            {
                base[key] = value;
            }
        }

        /// <summary>
        /// Adds an attribute to the collection. If the attribute
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(string key, HtmlAttribute value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            // Ignore duplicate attributes
            if (!ContainsKey(key))
                base.Add(key, value);
        }
    }
}
