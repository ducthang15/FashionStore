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
        var baseUrl = "https://unlimtailor.com";

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var urlset = new XElement(ns + "urlset");

        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/", "1.0", "daily"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/suit", "0.9", "weekly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/tuxedo", "0.9", "weekly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/faqs", "0.9", "weekly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/trousers", "0.9", "weekly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/shirt", "0.9", "weekly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/coat", "0.9", "monthly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/blazer", "0.9", "monthly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/fabrics", "0.9", "monthly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/cufflink", "0.9", "monthly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/shoe", "0.9", "monthly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/caravat", "0.9", "monthly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/appointment", "0.9", "weekly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/contact", "0.9", "weekly"));
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/gallery", "0.9", "weekly"));

        var products = _context.Products.ToList();
        foreach (var p in products)
        {
            urlset.Add(CreateUrlElement(ns, $"{baseUrl}/product/{p.Slug}", "0.9", "weekly"));
        }
        var news = _context.News.ToList();
        foreach (var n in news)
        {
            urlset.Add(CreateUrlElement(ns, $"{baseUrl}/blog/details/{n.Slug}", "0.9", "monthly"));
        }
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/aboutus", "0.8", "monthly"));

        var doc = new XDocument(urlset);
        return Content(doc.ToString(), "text/xml", Encoding.UTF8);
    }

    private XElement CreateUrlElement(XNamespace ns, string loc, string priority, string changefreq)
    {
        return new XElement(ns + "url",
            new XElement(ns + "loc", loc),
            new XElement(ns + "lastmod", DateTime.Now.ToString("yyyy-MM-dd")),
            new XElement(ns + "changefreq", changefreq),
            new XElement(ns + "priority", priority)
        );
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
