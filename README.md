# HtmlMonkey

HTML/XML parser.



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

