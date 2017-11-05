namespace HtmlMonkey
{
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
            return (Value != null) ? $"{Name}=\"{Value}\"" : Name;
        }
    }
}
