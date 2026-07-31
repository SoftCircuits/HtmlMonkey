// Copyright (c) 2019-2026 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.HtmlMonkey;

namespace HtmlMonkey.Tests
{
    public class HtmlAttributeCollectionTests
    {
        /// <summary>
        /// Tests the ordered dictionary logic of HtmlAttributeCollection.
        /// </summary>
        [Fact]
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
            Assert.Equal(5, attributes.Count);

            // Add
            attributes.Add("Six", "6");
            attributes.Add("Seven", "7");
            attributes.Add("Eight", "8");
            attributes.Add("Nine", "9");
            attributes.Add("Ten", "10");
            Assert.Equal(10, attributes.Count);

            // Add attributes
            attributes.Add(new HtmlAttribute("Eleven", "11"));
            attributes.Add(new HtmlAttribute("Twelve", "12"));
            attributes.Add(new HtmlAttribute("Thirteen", "13"));
            attributes.Add(new HtmlAttribute("Fourteen", "14"));
            attributes.Add(new HtmlAttribute("Fifteen", "15"));
            Assert.Equal(15, attributes.Count);

            // Add range
            attributes.AddRange(
            [
                new HtmlAttribute("Sixteen", "16"),
                new HtmlAttribute("Seventeen", "17"),
                new HtmlAttribute("Eighteen", "18"),
                new HtmlAttribute("Nineteen", "19"),
                new HtmlAttribute("Twenty", "20"),
            ]);
            Assert.Equal(20, attributes.Count);

            // By name
            Assert.Equal("1", attributes["One"]?.Value);
            Assert.Equal("2", attributes["Two"]?.Value);
            Assert.Equal("3", attributes["Three"]?.Value);
            Assert.Equal("4", attributes["Four"]?.Value);
            Assert.Equal("5", attributes["Five"]?.Value);
            Assert.Equal("6", attributes["Six"]?.Value);
            Assert.Equal("7", attributes["Seven"]?.Value);
            Assert.Equal("8", attributes["Eight"]?.Value);
            Assert.Equal("9", attributes["Nine"]?.Value);
            Assert.Equal("10", attributes["Ten"]?.Value);
            Assert.Equal("11", attributes["Eleven"]?.Value);
            Assert.Equal("12", attributes["Twelve"]?.Value);
            Assert.Equal("13", attributes["Thirteen"]?.Value);
            Assert.Equal("14", attributes["Fourteen"]?.Value);
            Assert.Equal("15", attributes["Fifteen"]?.Value);
            Assert.Equal("16", attributes["Sixteen"]?.Value);
            Assert.Equal("17", attributes["Seventeen"]?.Value);
            Assert.Equal("18", attributes["Eighteen"]?.Value);
            Assert.Equal("19", attributes["Nineteen"]?.Value);
            Assert.Equal("20", attributes["Twenty"]?.Value);
            Assert.Null(attributes["Twenty One"]);

            // By index
            Assert.Equal("1", attributes[0].Value);
            Assert.Equal("2", attributes[1].Value);
            Assert.Equal("3", attributes[2].Value);
            Assert.Equal("4", attributes[3].Value);
            Assert.Equal("5", attributes[4].Value);
            Assert.Equal("6", attributes[5].Value);
            Assert.Equal("7", attributes[6].Value);
            Assert.Equal("8", attributes[7].Value);
            Assert.Equal("9", attributes[8].Value);
            Assert.Equal("10", attributes[9].Value);
            Assert.Equal("11", attributes[10].Value);
            Assert.Equal("12", attributes[11].Value);
            Assert.Equal("13", attributes[12].Value);
            Assert.Equal("14", attributes[13].Value);
            Assert.Equal("15", attributes[14].Value);
            Assert.Equal("16", attributes[15].Value);
            Assert.Equal("17", attributes[16].Value);
            Assert.Equal("18", attributes[17].Value);
            Assert.Equal("19", attributes[18].Value);
            Assert.Equal("20", attributes[19].Value);

            // Remove by name
            attributes.Remove("One");
            attributes.Remove("Two");
            attributes.Remove("Three");
            attributes.Remove("Four");
            attributes.Remove("Five");
            Assert.Equal(15, attributes.Count);

            // Remove by index
            attributes.RemoveAt(10);
            attributes.RemoveAt(10);
            attributes.RemoveAt(10);
            attributes.RemoveAt(10);
            attributes.RemoveAt(10);
            Assert.Equal(10, attributes.Count);

            // By name
            Assert.Null(attributes["One"]);
            Assert.Null(attributes["Two"]);
            Assert.Null(attributes["Three"]);
            Assert.Null(attributes["Four"]);
            Assert.Null(attributes["Five"]);
            Assert.Equal("6", attributes["Six"]?.Value);
            Assert.Equal("7", attributes["Seven"]?.Value);
            Assert.Equal("8", attributes["Eight"]?.Value);
            Assert.Equal("9", attributes["Nine"]?.Value);
            Assert.Equal("10", attributes["Ten"]?.Value);
            Assert.Equal("11", attributes["Eleven"]?.Value);
            Assert.Equal("12", attributes["Twelve"]?.Value);
            Assert.Equal("13", attributes["Thirteen"]?.Value);
            Assert.Equal("14", attributes["Fourteen"]?.Value);
            Assert.Equal("15", attributes["Fifteen"]?.Value);
            Assert.Null(attributes["Sixteen"]);
            Assert.Null(attributes["Seventeen"]);
            Assert.Null(attributes["Eighteen"]);
            Assert.Null(attributes["Nineteen"]);
            Assert.Null(attributes["Twenty"]);

            // By index
            Assert.Equal("6", attributes[0].Value);
            Assert.Equal("7", attributes[1].Value);
            Assert.Equal("8", attributes[2].Value);
            Assert.Equal("9", attributes[3].Value);
            Assert.Equal("10", attributes[4].Value);
            Assert.Equal("11", attributes[5].Value);
            Assert.Equal("12", attributes[6].Value);
            Assert.Equal("13", attributes[7].Value);
            Assert.Equal("14", attributes[8].Value);
            Assert.Equal("15", attributes[9].Value);
        }

        private readonly List<string> Names =
        [
            "href",
            "target",
            "class",
            "title"
        ];

        private readonly List<string> Values =
        [
            "http://www.domain.com",
            "_blank",
            "cool-style",
            "Toolip here!"
        ];

        [Fact]
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
            Assert.Equal(4, attributes.Count);

            // Names iterator
            Assert.Equal(Names, [.. attributes.Names]);

            // Values iterator
            Assert.Equal((IEnumerable<string?>)Values, [.. attributes.Values]);
        }

        [Fact]
        public void FindTests()
        {
            string html = 
                """
                <table>
                    <thead>
                        <tr>
                            <th><p>Abc</p></td>
                            <th><p>Def</p></td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><p>Ghi</p></td>
                            <td><p class="abc">Jkl</p></td>
                        </tr>
                        <tr>
                            <td><p>Mno</p></td>
                            <td><p class="abc">Pqr</p></td>
                        </tr>
                    </tbody>
                </table>
                """;

            HtmlElementNode? node = HtmlDocument.FromHtml(html).RootNodes.First() as HtmlElementNode;
            Assert.NotNull(node);

            IEnumerable<HtmlElementNode> nodes = node.Children.FindOfType<HtmlElementNode>();
            Assert.Equal(17, nodes.Count());

            nodes = node.Children.Find("p");
            Assert.Equal(6, nodes.Count());

            nodes = node.Children.Find("tr");
            Assert.Equal(3, nodes.Count());

            nodes = node.Children.Find("td");
            Assert.Equal(4, nodes.Count());

            nodes = node.Children.Find(".abc");
            Assert.Equal(2, nodes.Count());
        }
    }
}
