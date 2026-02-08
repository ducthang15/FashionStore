using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;

public class SitemapController : Controller
{
    private readonly fashionDbContext _context;

    public SitemapController(fashionDbContext context)
    {
        _context = context;
    }

    [Route("sitemap.xml")]
    public IActionResult Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var urls = new List<string>();

        urls.Add($"{baseUrl}/");

        var blogs = _context.News.ToList();
        foreach (var b in blogs)
        {
            urls.Add($"{baseUrl}/Blog/Details/{b.NewsId}");
        }

        var xml = BuildXml(urls);

        return Content(xml, "text/xml");
    }


    private string BuildXml(List<string> urls)
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var doc = new XDocument(
            new XElement(ns + "urlset",
                urls.Select(u =>
                    new XElement(ns + "url",
                        new XElement(ns + "loc", u)
                    ))
            )
        );

        return doc.ToString();
    }
}
