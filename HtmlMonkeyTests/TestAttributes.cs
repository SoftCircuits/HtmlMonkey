using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class TestAttributes
    {
        private readonly List<string> Names = new List<string>
        {
            "href",
            "target",
            "class",
            "title"
        };

        private readonly List<string> Values = new List<string>
        {
            "http://www.domain.com",
            "_blank",
            "cool-style",
            "Toolip here!"
        };

        [TestMethod]
        public void AttributeTests()
        {
            // Test collection initializers
            HtmlAttributeCollection attributes = new HtmlAttributeCollection
            {
                { Names[0], Values[0] },
                { Names[1], Values[1] },
                { Names[2], Values[2] },
                { Names[3], Values[3] },
            };

            // Count property
            Assert.AreEqual(4, attributes.Count);

            // Names iterator
            Assert.IsTrue(attributes.Names.SequenceEqual(Names));

            // Values iterator
            Assert.IsTrue(attributes.Values.SequenceEqual(Values));
        }
    }
}
