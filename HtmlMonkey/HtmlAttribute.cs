/////////////////////////////////////////////////////////////
// HTML Monkey
// Copyright (c) 2018 Jonathan Wood
// http://www.softcircuits.com, http://www.blackbeltcoder.com
//

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Class that represents a single element attribute.
    /// </summary>
    public class HtmlAttribute
    {
        /// <summary>
        /// Attribute name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Attribute value. Will be an empty string if the attribute was set
        /// to an empty value. Will be null if the attribute name was not
        /// followed by an equal sign.
        /// </summary>
        public string Value { get; set; }

        public override string ToString()
        {
            string name = Name ?? "(null)";
            return (Value != null) ?
                string.Format("{0}=\"{1}\"", name, Value) :
                name;
        }
    }
}
