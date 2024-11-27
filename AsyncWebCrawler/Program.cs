// See https://aka.ms/new-console-template for more information

using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.ListT;
using HtmlAgilityPack;
using static FunctionalCSharp.New.Utils;

ListT<Async, (string, string)> Crawl(string url)
{
    HashSet<string> visited = new();
    HttpClient client = new HttpClient();
    var doc = new HtmlDocument();
    
    ListT<Async, (string link, string title)> Execute(string url)
    {
        FunctionalCSharp.New.Monads.List<string> ExtractLinks(string html)
        {
            var hrefLinks = new System.Collections.Generic.List<string>();
            
            // Load the HTML into an HtmlDocument object
            doc.LoadHtml(html);

            // Select all <a> tags in the HTML
            var anchorTags = doc.DocumentNode.SelectNodes("//a[@href]");

            if (anchorTags == null) return new FunctionalCSharp.New.Monads.List<string>(hrefLinks);
            hrefLinks.AddRange(anchorTags.Select(anchor => anchor.GetAttributeValue("href", string.Empty))
                .Where(href => !string.IsNullOrEmpty(href) && href.Trim().StartsWith(@"https://")));

            return new FunctionalCSharp.New.Monads.List<string>(hrefLinks);
        }
        
        string? ExtractTitle(string html)
        {
            doc.LoadHtml(html);
            var titleNode = doc.DocumentNode.SelectSingleNode("//title");
            return titleNode?.InnerText;
        }
        
        return
            from _ in When(visited.Add(url),
                    from html in ListT<Async>.Lift(client.GetStringAsync(url).ToAsync()).To()
                    from link in 
                        List.MSum(from c in ExtractLinks(html)
                                  select ListT<Async>.Pure(c)).To()
                    let title = ExtractTitle(html)
                    from res in Execute(link).To() + ListT<Async>.Pure((link, title)).To()
                    select res)
                .To()
            select _;
        
    }

    return Execute(url);
}

var listT =
    from c in Crawl("http://news.bing.com")
    select Log(c);

listT.RunListT().To().Run();