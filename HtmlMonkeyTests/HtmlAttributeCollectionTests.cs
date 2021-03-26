// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System.Collections.Generic;
using System.Linq;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class HtmlAttributeCollectionTests
    {
        /// <summary>
        /// Tests the ordered dictionary logic of HtmlAttributeCollection.
        /// </summary>
        [TestMethod]
        public void AttributeCollectionTest()
        {
            HtmlAttributeCollection attributes = new()
            {
                { "One", "1" },
                { "Two", "2" },
                { "Three", "3" },
                { "Four", "4" },
                { "Five", "5" }
            };
            Assert.AreEqual(5, attributes.Count);

            // Add
            attributes.Add("Six", "6");
            attributes.Add("Seven", "7");
            attributes.Add("Eight", "8");
            attributes.Add("Nine", "9");
            attributes.Add("Ten", "10");
            Assert.AreEqual(10, attributes.Count);

            // Add attributes
            attributes.Add(new HtmlAttribute("Eleven", "11"));
            attributes.Add(new HtmlAttribute("Twelve", "12"));
            attributes.Add(new HtmlAttribute("Thirteen", "13"));
            attributes.Add(new HtmlAttribute("Fourteen", "14"));
            attributes.Add(new HtmlAttribute("Fifteen", "15"));
            Assert.AreEqual(15, attributes.Count);

            // Add range
            attributes.AddRange(new[]
            {
                new HtmlAttribute("Sixteen", "16"),
                new HtmlAttribute("Seventeen", "17"),
                new HtmlAttribute("Eighteen", "18"),
                new HtmlAttribute("Nineteen", "19"),
                new HtmlAttribute("Twenty", "20"),
            });
            Assert.AreEqual(20, attributes.Count);

            // By name
            Assert.AreEqual("1", attributes["One"]?.Value);
            Assert.AreEqual("2", attributes["Two"]?.Value);
            Assert.AreEqual("3", attributes["Three"]?.Value);
            Assert.AreEqual("4", attributes["Four"]?.Value);
            Assert.AreEqual("5", attributes["Five"]?.Value);
            Assert.AreEqual("6", attributes["Six"]?.Value);
            Assert.AreEqual("7", attributes["Seven"]?.Value);
            Assert.AreEqual("8", attributes["Eight"]?.Value);
            Assert.AreEqual("9", attributes["Nine"]?.Value);
            Assert.AreEqual("10", attributes["Ten"]?.Value);
            Assert.AreEqual("11", attributes["Eleven"]?.Value);
            Assert.AreEqual("12", attributes["Twelve"]?.Value);
            Assert.AreEqual("13", attributes["Thirteen"]?.Value);
            Assert.AreEqual("14", attributes["Fourteen"]?.Value);
            Assert.AreEqual("15", attributes["Fifteen"]?.Value);
            Assert.AreEqual("16", attributes["Sixteen"]?.Value);
            Assert.AreEqual("17", attributes["Seventeen"]?.Value);
            Assert.AreEqual("18", attributes["Eighteen"]?.Value);
            Assert.AreEqual("19", attributes["Nineteen"]?.Value);
            Assert.AreEqual("20", attributes["Twenty"]?.Value);
            Assert.AreEqual(null, attributes["Twenty One"]);

            // By index
            Assert.AreEqual("1", attributes[0].Value);
            Assert.AreEqual("2", attributes[1].Value);
            Assert.AreEqual("3", attributes[2].Value);
            Assert.AreEqual("4", attributes[3].Value);
            Assert.AreEqual("5", attributes[4].Value);
            Assert.AreEqual("6", attributes[5].Value);
            Assert.AreEqual("7", attributes[6].Value);
            Assert.AreEqual("8", attributes[7].Value);
            Assert.AreEqual("9", attributes[8].Value);
            Assert.AreEqual("10", attributes[9].Value);
            Assert.AreEqual("11", attributes[10].Value);
            Assert.AreEqual("12", attributes[11].Value);
            Assert.AreEqual("13", attributes[12].Value);
            Assert.AreEqual("14", attributes[13].Value);
            Assert.AreEqual("15", attributes[14].Value);
            Assert.AreEqual("16", attributes[15].Value);
            Assert.AreEqual("17", attributes[16].Value);
            Assert.AreEqual("18", attributes[17].Value);
            Assert.AreEqual("19", attributes[18].Value);
            Assert.AreEqual("20", attributes[19].Value);

            // Remove by name
            attributes.Remove("One");
            attributes.Remove("Two");
            attributes.Remove("Three");
            attributes.Remove("Four");
            attributes.Remove("Five");
            Assert.AreEqual(15, attributes.Count);

            // Remove by index
            attributes.RemoveAt(10);
            attributes.RemoveAt(10);
            attributes.RemoveAt(10);
            attributes.RemoveAt(10);
            attributes.RemoveAt(10);
            Assert.AreEqual(10, attributes.Count);

            // By name
            Assert.AreEqual(null, attributes["One"]);
            Assert.AreEqual(null, attributes["Two"]);
            Assert.AreEqual(null, attributes["Three"]);
            Assert.AreEqual(null, attributes["Four"]);
            Assert.AreEqual(null, attributes["Five"]);
            Assert.AreEqual("6", attributes["Six"]?.Value);
            Assert.AreEqual("7", attributes["Seven"]?.Value);
            Assert.AreEqual("8", attributes["Eight"]?.Value);
            Assert.AreEqual("9", attributes["Nine"]?.Value);
            Assert.AreEqual("10", attributes["Ten"]?.Value);
            Assert.AreEqual("11", attributes["Eleven"]?.Value);
            Assert.AreEqual("12", attributes["Twelve"]?.Value);
            Assert.AreEqual("13", attributes["Thirteen"]?.Value);
            Assert.AreEqual("14", attributes["Fourteen"]?.Value);
            Assert.AreEqual("15", attributes["Fifteen"]?.Value);
            Assert.AreEqual(null, attributes["Sixteen"]);
            Assert.AreEqual(null, attributes["Seventeen"]);
            Assert.AreEqual(null, attributes["Eighteen"]);
            Assert.AreEqual(null, attributes["Nineteen"]);
            Assert.AreEqual(null, attributes["Twenty"]);

            // By index
            Assert.AreEqual("6", attributes[0].Value);
            Assert.AreEqual("7", attributes[1].Value);
            Assert.AreEqual("8", attributes[2].Value);
            Assert.AreEqual("9", attributes[3].Value);
            Assert.AreEqual("10", attributes[4].Value);
            Assert.AreEqual("11", attributes[5].Value);
            Assert.AreEqual("12", attributes[6].Value);
            Assert.AreEqual("13", attributes[7].Value);
            Assert.AreEqual("14", attributes[8].Value);
            Assert.AreEqual("15", attributes[9].Value);
        }

        private readonly List<string> Names = new()
        {
            "href",
            "target",
            "class",
            "title"
        };

        private readonly List<string> Values = new()
        {
            "http://www.domain.com",
            "_blank",
            "cool-style",
            "Toolip here!"
        };

        [TestMethod]
        public void OrderingTest()
        {
            // Test collection initializers
            HtmlAttributeCollection attributes = new()
            {
                { Names[0], Values[0] },
                { Names[1], Values[1] },
                { Names[2], Values[2] },
                { Names[3], Values[3] },
            };

            // Count property
            Assert.AreEqual(4, attributes.Count);

            // Names iterator
            CollectionAssert.AreEqual(Names, attributes.Names.ToList());

            // Values iterator
            CollectionAssert.AreEqual(Values, attributes.Values.ToList());
        }

        [TestMethod]
        public void FindTests()
        {
            string html = @"<table>
    <thead>
        <tr>
            <th><p>Abc</p></td>
            <th><p>Def</p></td>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><p>Ghi</p></td>
            <td><p class=""abc"">Jkl</p></td>
        </tr>
        <tr>
            <td><p>Mno</p></td>
            <td><p class=""abc"">Pqr</p></td>
        </tr>
    </tbody>
</table>";

            HtmlElementNode? node = HtmlDocument.FromHtml(html).RootNodes.First() as HtmlElementNode;
            Assert.IsNotNull(node);

            IEnumerable<HtmlElementNode> nodes = node.Children.FindOfType<HtmlElementNode>();
            Assert.AreEqual(17, nodes.Count());

            nodes = node.Children.Find("p");
            Assert.AreEqual(6, nodes.Count());

            nodes = node.Children.Find("tr");
            Assert.AreEqual(3, nodes.Count());

            nodes = node.Children.Find("td");
            Assert.AreEqual(4, nodes.Count());

            nodes = node.Children.Find(".abc");
            Assert.AreEqual(2, nodes.Count());
        }
    }
}
