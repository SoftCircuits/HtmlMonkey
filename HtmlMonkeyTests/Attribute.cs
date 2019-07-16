// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System.Collections.Generic;
using System.Linq;

namespace HtmlMonkeyTests
{
    public class Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Attribute(string name, string value = null)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            Assert.AreNotEqual(null, Name);
            if (Value == null)
                return Name;
            return $"{Name}=\"{Value}\"";
        }

        public static void CompareAttributes(IEnumerable<Attribute> expectedAttributes, HtmlAttributeCollection attributes)
        {
            if (expectedAttributes == null)
            {
                Assert.AreEqual(0, attributes.Count);
                return;
            }
            Assert.AreEqual(expectedAttributes.Count(), attributes.Count);
            if (expectedAttributes.Count() == attributes.Count)
            {
                foreach (Attribute attribute in expectedAttributes)
                {
                    HtmlAttribute attr = attributes[attribute.Name];
                    Assert.AreNotEqual(null, attr);
                    Assert.AreEqual(attribute.Value, attributes[attribute.Name]?.Value);
                }
            }
        }
    }
}
