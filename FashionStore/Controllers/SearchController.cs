using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
public class SearchController : Controller
{
    private readonly fashionDbContext _context;

    public SearchController(fashionDbContext context)
    {
        _context = context;
    }
    // Alias tiếng Anh -> category tiếng Việt
    private readonly Dictionary<string, string> categoryAlias = new()
    {
        { "shoe", "giày" },
        { "shoes", "giày" },

        { "shirt", "sơ mi" },

        { "vest", "suit" },

        { "pant", "quần" },
        { "pants", "quần" },
        { "trouser", "quần" },

        { "tuxedo", "vest cưới" },
        { "vests", "suit" },
        { "cufflinks", "Khuy măng sét" },
        { "coat", "măng tô" },
        { "fabric", "vải" },
        { "blazer", "áo khoác ngoài" }
    };
    [HttpGet]
    public async Task<IActionResult> LiveSearch(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 2)
        {
            return Json(new List<object>());
        }

        string lowerKeyword = keyword.ToLower().Trim();

        // Nếu user search tiếng Anh
        // thì đổi sang category tiếng Việt
        foreach (var item in categoryAlias)
        {
            // user gõ gần đúng
            if (item.Key.StartsWith(lowerKeyword))
            {
                lowerKeyword = item.Value;
                break;
            }
        }

        var products = await _context.Products

            // load Category
            .Include(p => p.Category)

            .Where(p =>

                // Search tên sản phẩm
                (p.ProductName != null &&
                 p.ProductName.ToLower().Contains(lowerKeyword))

                ||

                // Search mô tả
                (p.Description != null &&
                 p.Description.ToLower().Contains(lowerKeyword))

                ||

                // Search category
                (p.Category != null &&
                 p.Category.CategoryName.ToLower().Contains(lowerKeyword))

            )

            .Take(10)

            .Select(p => new
            {
                title = p.ProductName,

                url = $"/product/{p.Slug}",

                image = p.ImageUrl,

                type = "Product",

                price = p.Price > 0
                    ? p.Price.ToString("N0") + " ₫"
                    : "Contact"
            })

            .ToListAsync();

        var blogs = await _context.News

            .Where(b =>
                b.Title != null &&
                b.Title.ToLower().Contains(lowerKeyword)
            )

            .Take(4)

            .Select(b => new
            {
                title = b.Title,

                url = $"/blog/details/{b.Slug}",

                image = b.ImageUrl,

                type = "Article",

                price = ""
            })

            .ToListAsync();

        var results = products
            .Cast<object>()
            .Concat(blogs)
            .ToList();

        return Json(results);
    }
}