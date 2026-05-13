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
    [HttpGet]
    public async Task<IActionResult> LiveSearch(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 2)
            return Json(new List<object>());
        string lowerKeyword = keyword.ToLower();
        var products = await _context.Products.Where(p => (p.ProductName != null && p.ProductName.ToLower().Contains(lowerKeyword)) || (p.Description != null && p.Description.ToLower().Contains(lowerKeyword))).Take(4)
            .Select(p => new
        {
            title = p.ProductName,
            url = $"/product/{p.Slug}",
            image = p.ImageUrl,
            type = "Product",
            price = p.Price > 0 ? p.Price.ToString("N0") + " ₫" : "Contact"
        }).ToListAsync();
        var blogs = await _context.News.Where(b => b.Title.ToLower().Contains(lowerKeyword)).Take(2)
            .Select(b => new
        {
            title = b.Title,
            url = $"/blog/details/{b.Slug}",
            image = b.ImageUrl,
            type = "Article",
            price = ""
        }).ToListAsync();
        var results = products.Cast<object>().Concat(blogs).ToList();
        return Json(results);
    }
}