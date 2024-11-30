// See https://aka.ms/new-console-template for more information

using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.ListT;
using HtmlAgilityPack;
using static FunctionalCSharp.New.Utils;
using static FunctionalCSharp.New.MonadicConditionals;

FunctionalCSharp.New.Monads.List<string> ExtractLinks(HtmlDocument doc, string html)
{
    if (string.IsNullOrEmpty(html))
        return List.Empty<string>().To();
    var hrefLinks = new System.Collections.Generic.List<string>();

    // Load the HTML into an HtmlDocument object
    doc.LoadHtml(html);

    // Select all <a> tags in the HTML
    var anchorTags = doc.DocumentNode.SelectNodes("//a[@href]");

    if (anchorTags == null) return new FunctionalCSharp.New.Monads.List<string>(hrefLinks);
    hrefLinks.AddRange(anchorTags.Select(anchor => anchor.GetAttributeValue("href", string.Empty))
        .Where(href =>
            !string.IsNullOrEmpty(href) &&
            href.Trim().StartsWith(@"https://")));

    return new FunctionalCSharp.New.Monads.List<string>(hrefLinks);
}

string? ExtractTitle(HtmlDocument doc, string html)
{
    if (string.IsNullOrEmpty(html))
        return "";
    doc.LoadHtml(html);
    var titleNode = doc.DocumentNode.SelectSingleNode("//title");
    return titleNode?.InnerText;
}

// Crawls pages asynchronously until error is encountered
ListT<MaybeT<Async>, (string, string)> CrawlUntilError(string url)
{
    HashSet<string> visited = new();
    HttpClient client = new HttpClient();
    var doc = new HtmlDocument();

    ListT<MaybeT<Async>, (string link, string title)> Execute(string url) =>
        from _ in When(visited.Add(url),
                from html in ListT<MaybeT<Async>>.Lift(
                    MaybeT<Async, string>.Of(
                        from a in client.GetStringAsync(url).ToAsync().Catch()
                        select a.ToMaybe())
                ).To()
                from link in
                    List.MSum(from c in ExtractLinks(doc, html)
                        select ListT<MaybeT<Async>>.Pure(c)).To()
                let title = ExtractTitle(doc, html)
                from res in Execute(link).To() + ListT<MaybeT<Async>>.Pure((link, title)).To()
                select res
            )
            .To()
        select _;

    return Execute(url);
}

// Crawls pages asynchronously omitting erratic pages
ListT<Async, (string, string)> Crawl(string url)
{
    HashSet<string> visited = new();
    HttpClient client = new HttpClient();
    var doc = new HtmlDocument();
    
    ListT<Async, (string link, string title)> Execute(string url)
    {
        ListT<Async, string> TryDownloadHtml() =>
            from res in ListT<Async>.Lift(client.GetStringAsync(url).ToAsync().Catch()).To()
            from x in res.Either(ListT<Async>.Pure, _ => ListT<Async>.Empty<string>()).To()
            select x;

        return from _ in When(visited.Add(url),
                    from html in TryDownloadHtml()
                    from link in
                        List.MSum(from c in ExtractLinks(doc, html)
                            select ListT<Async>.Pure(c)).To()
                    let title = ExtractTitle(doc, html)
                    from res in Execute(link).To() + ListT<Async>.Pure((link, title)).To()
                    select res
                ).To()
            select _;
    }

    return Execute(url);
}

var listT =
    from c in Crawl("http://news.bing.com")
    select Log(c);

listT.RunListT.To().Run();