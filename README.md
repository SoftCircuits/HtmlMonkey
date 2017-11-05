# HtmlMonkey

HtmlMonkey is an HTML/XML parser written in C#. It allows you to parse an HTML or XML string into a hierarchy of node objects, which can then be traversed or otherwise examined from code. In addition, the node objects can be modified or even built from scratch using code. Finally, the classes can generate the HTML or XML from the data.

The code also include a WinForms application to display the parsed data nodes. This was mostly done for testing the parser, but offers some functionality that may be useful for inspecting the original markup.

At this point, the public interface is rather small. Here are a few starter examples.

#### Parse an HTML String

    // Note: We specify the HtmlDocument namespace below because .NET also has
    // an HtmlDocument class
    string html = "...";
    HtmlMonkey.HtmlDocument document = HtmlDocument.FromHtml(html);

#### Find All Anchor Tag Nodes

    // Note: This method uses case-insensitive comparisons
    var nodes = document.FindTags("a");

#### Find All Anchor Tag Nodes with an ID

    var nodes = document.FindTags("a").Where(n => n.Attributes.ContainsKey("id"));
    
#### Find All Text Nodes with Text Longer than 100 Characters

    var nodes = document.FindOfType<HtmlTextNode>(n => n.Html.Length > 100);

#### Find All Anchor Tag Nodes that Link to github.com

    HtmlMonkey.HtmlDocument document = HtmlDocument.FromHtml(txtHtml.Text);
    var tags = document.FindTags("a");
    foreach (var node in tags)
    {
        if (node.Attributes.TryGetValue("href", out HtmlAttribute href))
        {
            if (Uri.TryCreate(href.Value, UriKind.Absolute, out Uri uri))
            {
                // Note: May need to test for variations such as "www.github.com"
                var host = uri.Host;
                if (host.Equals("github.com", StringComparison.OrdinalIgnoreCase))
                {
                    // Found a match!
                }
            }
        }
    }

## Enhancing the Library

This is my intial attempt at this library and I would appreciate any efforts by others to contribute. I want to keep the library small but would like to see more testing done on a wide variety of input markup. What sort of scenarios does the library not handle correctly? This is the type of information I'd be curious about.
