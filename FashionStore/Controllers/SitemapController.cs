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
        // Tự động lấy domain thực tế (sau này lên mạng nó sẽ tự đổi localhost thành domain thật)
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var urlset = new XElement(ns + "urlset");

        // 1. Trang chủ (Ưu tiên cao nhất)
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/", "1.0", "daily"));

        // 2. Lấy tất cả Sản phẩm (Products) - Rất quan trọng cho SEO bán hàng
        var products = _context.Products.ToList(); // Giả sử bảng của bạn tên Products
        foreach (var p in products)
        {
            // Thay Slug hoặc Id tùy theo cấu trúc Route của bạn
            urlset.Add(CreateUrlElement(ns, $"{baseUrl}/Home/Details/{p.Slug}", "0.9", "weekly"));
        }

        // 3. Lấy tất cả Bài viết (News/Blog)
        var news = _context.News.ToList();
        foreach (var n in news)
        {
            urlset.Add(CreateUrlElement(ns, $"{baseUrl}/Blog/Details/{n.NewsId}", "0.7", "monthly"));
        }

        // 4. Các trang tĩnh khác (Giới thiệu, Liên hệ)
        urlset.Add(CreateUrlElement(ns, $"{baseUrl}/Home/About", "0.5", "monthly"));

        var doc = new XDocument(urlset);
        return Content(doc.ToString(), "text/xml", Encoding.UTF8);
    }

    // Hàm phụ trợ để tạo cấu trúc URL chuẩn SEO
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
