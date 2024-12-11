// See https://aka.ms/new-console-template for more information

using FunctionalCSharp.New;
using FunctionalCSharp.New.Monads;
using FunctionalCSharp.New.Monads.ListT;
using HtmlAgilityPack;
using static FunctionalCSharp.New.Utils;
using static FunctionalCSharp.New.MonadicConditionals;
using static FunctionalCSharp.New.Monads.ListT.ListT<FunctionalCSharp.New.Monads.Async>;

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
ListT<ResultT<Exception, Async>, (string, string)> CrawlUntilError(string url)
{
    HashSet<string> visited = new();
    HttpClient client = new HttpClient();
    var doc = new HtmlDocument();

    ListT<ResultT<Exception, Async>, (string link, string title)> Execute(string url) =>
        from _ in When(visited.Add(url),
                from html in ListT<ResultT<Exception, Async>>.Lift(
                    ResultT<Exception, Async, string>.Of(client.GetStringAsync(url).ToMonad().Catch())
                ).To()
                from link in
                    List.MSum(from c in ExtractLinks(doc, html)
                        select ListT<ResultT<Exception, Async>>.Pure(c)).To()
                let title = ExtractTitle(doc, html)
                from res in Execute(link).To() + ListT<ResultT<Exception, Async>>.Pure((link, title)).To()
                select res
            )
            .To()
        select _;

    return Execute(url);
}

// Crawls pages asynchronously omitting erratic pages
ListT<Async, (string, string)> Crawl(string url)
{
    HashSet<string> visited = [];
    HttpClient client = new HttpClient();
    var doc = new HtmlDocument();

    ListT<Async, (string link, string title)> Execute(string url)
    {
        ListT<Async, string> TryDownloadHtml() =>
            from res in Lift(client.GetStringAsync(url).ToMonad().Catch()).To()
            from x in res.Either(Pure, _ => Empty<string>()).To()
            select x;

        return from _ in When(visited.Add(url),
                from html in TryDownloadHtml()
                from link in
                    List.MSum(from c in ExtractLinks(doc, html)
                        select Pure(c)).To()
                let title = ExtractTitle(doc, html)
                from res in Execute(link).To() + Pure((link, title)).To()
                select res
            ).To()
            select _;
    }

    return Execute(url);
}

var listT =
    from c in Crawl("http://news.bing.com")
    where c.Item1.Contains(".microsoft.com")
    select Log(c);

void ChunkBy(ListT<Async, (string, string)> asyncSeq,int index)
{
    var (_, rest) = asyncSeq.SplitAt(index).To().Run();
    Console.WriteLine("Continue? (Y/N)");
    if (Console.ReadLine()?.ToLower() == "y") 
        ChunkBy(rest,index);
}

ChunkBy(listT,10);

// switch (listT.RunListT.To().RunResultT.To().Run())
// {
//     case Error<Unit, Exception>(var errorValue):
//         Console.WriteLine(errorValue);
//         break;
//     case Ok<Unit, Exception> ok:
//         break;
//     default:
//         throw new ArgumentOutOfRangeException();
// }