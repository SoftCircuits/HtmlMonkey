# HtmlMonkey

HtmlMonkey is a lightweight HTML/XML parser written in C#. It allows you to parse an HTML or XML string into a hierarchy of node objects, which can then be traversed or otherwise examined from code. In addition, the node objects can be modified or even built from scratch using code. Finally, the classes can generate the HTML or XML from the data.

The code also include a WinForms application to display the parsed data nodes. This was mostly done for testing the parser, but offers some functionality that may be useful for inspecting the original markup.

## Getting Started

Once you have aquired an HTML file, you can use either of the static methods `HtmlDocument.FromHtml()` or `HtmlDocument.FromFile()` to parse the HTML and create an `HtmlDocument` object.

#### Parse an HTML String

```cs
// Note: We specify the HtmlMonkey namespace below because
// .NET also has an HtmlDocument class
string html = "...";   // HTML markup
HtmlMonkey.HtmlDocument document = HtmlMonkey.HtmlDocument.FromHtml(html);
```

## Using Selectors

HtmlMonkey now supports a modified subset of jQuery selectors to find nodes.

#### Specifying Tag Names

You can specify a tag name to return all the nodes with that tag.

```cs
// Get all <p> tags in the document. Search is not case-sensitive
IEnumerable<HtmlElementNode> nodes = document.Find("p");

// Get all nodes in the document.
// Same as not specifying a tag name
nodes = document.Find("*");
```

#### Specifying Attributes

There are several ways to search for nodes with specific attributes. You can use the pound (#), period (.) or colon (:) to specify a value for the `id`, `class` or `type` attribute.

```cs
// Get any nodes with the attribute id="center-ad"
IEnumerable<HtmlElementNode> nodes = document.Find("#center-ad");

// Get any <div> tags with the attribute class="align-right"
nodes = document.Find("div.align-right");

// All all <input> tags with the attribute type="button"
nodes = document.Find("input:button");
```

For greater control over attributes, you can use square brackets ([]). This is similar to specifying attributes in jQuery, but there are some differences. The first difference is that all the variations for finding a match at the start, middle or end are not supported by HtmlMonkey. However, to make up for this limitation, you can use the `:=` operator to specify that the value is a regular expression and the code will match if the attribute value matches that regular expression.

```cd
// Get any <p> tags with the attribute id="center-ad"
IEnumerable<HtmlElementNode> nodes = document.Find("p[id="center-ad"]);

// Get any <p> tags that have both attributes id="center-ad" and class="align-right"
nodes = document.Find("p[id=center-ad][class=align-right]);

// Get any <p> tags with the attribute data-id with a value that matches the regular
// expression "abc-\d+"
nodes = document.Find("p[data-id:="abc-\\d+"]);
```

The quotes within the square brackets are optional if the value contains no whitespace or most punctuation. In addition, if there is no equal sign within the square brackets, the code simply checks if the attribute exists without regard to its value.

#### Multiple Selectors

There are several cases where you can specify multiple selectors.

```cs
// Returns all <a>, <div> and <p> tags
IEnumerable<HtmlElementNode> nodes = document.Find("a, div, p");

// Returns all <span> tags that are descendants of a <div> tag
nodes = document.Find("div span");

// Returns all <span> tags that are a direct descendant of a <div> tag
nodes = document.Find("div > span");
```

#### Additional Examples

```cs
// Returns all <a> tags that have an href attribute
IEnumerable<HtmlElementNode> nodes = document.Find("a[href]");
```
```cs
// Finds all <a> links that link to blackbeltcoder.com
// Uses regular expressions to allow optional http, https, or www prefix
nodes = document.Find("a[href:=\"^(http:\\/\\/|https:\\/\\/)?(www\\.)?blackbeltcoder.com\"]");
```

----------

At this point, the public interface is rather small. Here are a few starter examples.

##### Parse an HTML String

```cs
// Note: We specify the HtmlMonkey namespace below because
// .NET also has an HtmlDocument class
string html = "...";
HtmlMonkey.HtmlDocument document = HtmlMonkey.HtmlDocument.FromHtml(html);
```

##### Find All Anchor Tag Nodes

```cs
// Note: This method uses case-insensitive comparisons
var nodes = document.FindTags("a");
```

##### Find All Anchor Tag Nodes with an ID Attribute

```cs
// Note: This method uses case-insensitive comparisons for both the tag and attribute
var nodes = document.FindTags("a", n => n.Attributes.ContainsKey("id"));
```

##### Find All Text Nodes

```cs
var nodes = document.FindOfType<HtmlTextNode>();
```

##### Find All Text Nodes with Text Longer than 100 Characters

```cs
var nodes = document.FindOfType<HtmlTextNode>(n => n.Html.Length > 100);
```

##### Find All Anchor Tag Nodes that Link to blackbeltcoder.com

```cs
HtmlMonkey.HtmlDocument document = HtmlDocument.FromHtml(txtHtml.Text);
foreach (var node in document.FindTags("a"))
{
    if (node.Attributes.TryGetValue("href", out HtmlAttribute href) &&
        Uri.TryCreate(href.Value, UriKind.Absolute, out Uri uri))
    {
        string host = uri.Host.ToLower();
        if (host == "blackbeltcoder.com" || host == "www.blackbeltcoder.com")
        {
            // Found a match!
        }
    }
}
```

## Enhancing the Library

This is my initial attempt at this library and I would appreciate any efforts by others to contribute. I want to keep the library small but would like to see more testing done on a wide variety of input markup. What sort of scenarios does the library not handle correctly? This is the type of information I'd be curious about.
