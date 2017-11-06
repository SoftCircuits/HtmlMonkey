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
            foreach (var value in Values)
            {
                builder.Append(' ');
                builder.Append(value.ToString());
            }
            return builder.ToString();
        }
    }
}
