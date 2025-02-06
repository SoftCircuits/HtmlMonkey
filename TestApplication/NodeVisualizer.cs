// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.HtmlMonkey;
using System.Diagnostics;
using System.Text;

namespace TestApplication
{
    /// <summary>
    /// Class that provides visualization data for a particular node type.
    /// </summary>
    public class Visualizer
    {
        public Func<object, string> ShortDescription { get; set; } = (o) => string.Empty;
        public Func<object, string> LongDescription { get; set; } = (o) => string.Empty;
        public Action<object, ListView> PopulateProperties { get; set; } = (o, l) => { };
    }

    /// <summary>
    /// Class to provide HTML node visualization data.
    /// </summary>
    public class NodeVisualizer
    {
        private static readonly string[] EmptyColumns = [];
        private static readonly string[] AttributeColumns = [ "Attribute", "Value" ];
        private static readonly string[] ParameterColumns = [ "Parameter" ];
        private static readonly string[] CDataColumns = [ "Delimiter", "Value" ];
        private static readonly string[] DocumentColumns = [ "Property", "Value" ];

        // Note: To work correctly, derived types must appear before base types.
        private static readonly Dictionary<Type, Visualizer> VisualizerLookup = new()
        {
            [typeof(SoftCircuits.HtmlMonkey.HtmlDocument)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionDocument((SoftCircuits.HtmlMonkey.HtmlDocument)n),
                LongDescription = n => LongDescriptionDocument((SoftCircuits.HtmlMonkey.HtmlDocument)n),
                PopulateProperties = (n, lvw) => PopulatePropertiesDocument((SoftCircuits.HtmlMonkey.HtmlDocument)n, lvw)
            },
            [typeof(HtmlCDataNode)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionCData((HtmlCDataNode)n),
                LongDescription = n => LongDescriptionCData((HtmlCDataNode)n),
                PopulateProperties = (n, lvw) => PopulatePropertiesCData((HtmlCDataNode)n, lvw)
            },
            [typeof(HtmlTextNode)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionText((HtmlTextNode)n),
                LongDescription = n => LongDescriptionText((HtmlTextNode)n),
                PopulateProperties = (n, lvw) => PopulatePropertiesText((HtmlTextNode)n, lvw)
            },
            [typeof(HtmlElementNode)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionElement((HtmlElementNode)n),
                LongDescription = n => LongDescriptionElement((HtmlElementNode)n),
                PopulateProperties = (n, lvw) => PopulatePropertiesElement((HtmlElementNode)n, lvw)
            },
            [typeof(XmlHeaderNode)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionXmlHeader((XmlHeaderNode)n),
                LongDescription = n => LongDescriptionXmlHeader((XmlHeaderNode)n),
                PopulateProperties = (n, lvw) => PopulatePropertiesXmlHeader((XmlHeaderNode)n, lvw)
            },
            [typeof(HtmlHeaderNode)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionDocTypeHeader((HtmlHeaderNode)n),
                LongDescription = n => LongDescriptionDocTypeHeader((HtmlHeaderNode)n),
                PopulateProperties = (n, lvw) => PopulatePropertiesDocTypeHeader((HtmlHeaderNode)n, lvw)
            }
        };

        /// <summary>
        /// Returns a visualizer for the given node. Returns node if node is not a valid node.
        /// </summary>
        /// <param name="node">Node for which to return a visualizer.</param>
        public static Visualizer GetVisualizer(object node)
        {
            if (VisualizerLookup.TryGetValue(node.GetType(), out Visualizer? visualizer))
                return visualizer;
            Debug.Assert(false);
            return null;
        }

        private static string ShortDescriptionDocument(SoftCircuits.HtmlMonkey.HtmlDocument document) => $"[{document.GetType()}]";
        private static string ShortDescriptionDocTypeHeader(HtmlHeaderNode _) => "<!doctype>";
        private static string ShortDescriptionXmlHeader(XmlHeaderNode _) => "<?xml>";
        private static string ShortDescriptionElement(HtmlElementNode node) => $"<{node.TagName}>";
        private static string ShortDescriptionText(HtmlTextNode node) => $"\"{TruncateEncoded(node.Text, 32)}\"";
        private static string ShortDescriptionCData(HtmlCDataNode node) => $"\"{TruncateEncoded(node.InnerHtml, 32)}\"";

        private static string LongDescriptionDocument(SoftCircuits.HtmlMonkey.HtmlDocument _) => string.Empty;
        private static string LongDescriptionDocTypeHeader(HtmlHeaderNode _) => string.Empty;
        private static string LongDescriptionXmlHeader(XmlHeaderNode _) => string.Empty;
        private static string LongDescriptionElement(HtmlElementNode _) => string.Empty;
        private static string LongDescriptionText(HtmlTextNode node) => node.InnerHtml;
        private static string LongDescriptionCData(HtmlCDataNode node) => node.InnerHtml;

        private static void PopulatePropertiesDocument(SoftCircuits.HtmlMonkey.HtmlDocument document, ListView listView)
        {
            InitializeListView(DocumentColumns, listView);
            listView.Items.Add("Path").SubItems.Add(document.Path);
        }

        private static void PopulatePropertiesDocTypeHeader(HtmlHeaderNode node, ListView listView)
        {
            InitializeListView(ParameterColumns, listView);
            foreach (var attribute in node.Attributes)
            {
                var item = listView.Items.Add(attribute.Name);
                item.SubItems.Add(attribute.Value ?? "(null)");
            }
        }

        private static void PopulatePropertiesXmlHeader(XmlHeaderNode node, ListView listView)
        {
            InitializeListView(AttributeColumns, listView);
            foreach (var attribute in node.Attributes)
            {
                var item = listView.Items.Add(attribute.Name);
                item.SubItems.Add(attribute.Value ?? "(null)");
            }
        }

        private static void PopulatePropertiesElement(HtmlElementNode node, ListView listView)
        {
            InitializeListView(AttributeColumns, listView);
            foreach (var attribute in node.Attributes)
            {
                var item = listView.Items.Add(attribute.Name);
                item.SubItems.Add(attribute.Value ?? "(null)");
            }
        }

        private static void PopulatePropertiesText(HtmlTextNode _, ListView listView)
        {
            InitializeListView(EmptyColumns, listView);
        }

        private static void PopulatePropertiesCData(HtmlCDataNode node, ListView listView)
        {
            InitializeListView(CDataColumns, listView);
            var item = listView.Items.Add("Prefix");
            item.SubItems.Add(node.Prefix ?? string.Empty);
            item = listView.Items.Add("Suffix");
            item.SubItems.Add(node.Suffix ?? string.Empty);
        }

        private static void InitializeListView(string[] columns, ListView listView)
        {
            listView.Columns.Clear();
            listView.Items.Clear();
            for (int i = 0; i < columns.Length; i++)
            {
                var header = listView.Columns.Add(columns[i]);
                header.Width = 240;
            }
        }

        private static string TruncateEncoded(string s, int maxLength)
        {
            bool isTruncated = false;

            if (s == null)
                return string.Empty;
            if (s.Length > maxLength)
            {
                s = s[..maxLength];
                isTruncated = true;
            }
            s = CStringEncoder.EncodeString(s);
            if (isTruncated)
                s += "...";
            return s;
        }
    }

    #region String encoding

    /// <summary>
    /// Class to convert text to C-style strings. For example, replaces new lines with
    /// &quot;\r\n&quot;.
    /// </summary>
    public static class CStringEncoder
    {
        private static readonly Dictionary<char, string> EncodeLookup = new()
        {
            ['\\'] = "\\\\",
            ['"'] = "\\\"",
            ['\a'] = "\\a",
            ['\b'] = "\\b",
            ['\f'] = "\\f",
            ['\n'] = "\\n",
            ['\r'] = "\\r",
            ['\t'] = "\\t",
            ['\v'] = "\\v",
        };

        public static string EncodeString(string s)
        {
            StringBuilder builder = new(s.Length + (s.Length / 4));

            foreach (char c in s)
            {
                if (EncodeLookup.TryGetValue(c, out string? value))
                    builder.Append(value);
                else if (char.IsControl(c))
                    builder.Append($"\\u{((int)c).ToString("x4")}");
                else
                    builder.Append(c);
            }
            return builder.ToString();
        }
    }

    #endregion

}
