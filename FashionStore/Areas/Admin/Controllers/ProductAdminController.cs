using FashionStore.Utilities;
using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using FashionStore.Services;

namespace FashionStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductAdminController : BaseAdminController
    {
        private readonly fashionDbContext _context;
        private readonly IStorageService _storageService;
        public ProductAdminController(fashionDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }
        public async Task<IActionResult> Index(int? categoryId, string keyword, int page = 1)
        {
            const int pageSize = 20;
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(p => p.ProductName.Contains(keyword));
            }
            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            page = Math.Max(1, page);

            if (totalPages > 0)
            {
                page = Math.Min(page, totalPages);
            }
            var products = await query
                .OrderByDescending(p => p.ProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CategoryId = new SelectList(
                await _context.Categories.AsNoTracking().ToListAsync(),
                "CategoryId",
                "CategoryName",
                categoryId);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.CategoryFilter = categoryId;
            ViewBag.Keyword = keyword;

            return View(products);
        }
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product, List<IFormFile> files, int mainImageIndex)
        {
            ModelState.Remove("Category");
            ModelState.Remove("OrderDetails");
            ModelState.Remove("ProductImages");
            ModelState.Remove("ImageUrl");

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName");
                return View(product ?? new Product());
            }
            product.Slug = SlugHelper.GenerateSlug(product.ProductName);
            var maxOrder = await _context.Products
             .Where(p => p.CategoryId == product.CategoryId)
             .MaxAsync(p => (int?)p.DisplayOrder);

            if (maxOrder.HasValue)
            {
                product.DisplayOrder = maxOrder.Value + 1;
            }
            if (_context.Products.Any(p => p.Slug == product.Slug))
            {
                product.Slug += "-" + DateTime.Now.Ticks;
            }
            product.ImageUrl = "/images/no-image.png";
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            if (files != null && files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var fileName = await UploadFile(files[i]);

                    if (i == mainImageIndex)
                    {
                        product.ImageUrl = fileName;
                    }

                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = fileName
                    });
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private async Task<string?> UploadFile(IFormFile file)
        {
            return await _storageService.UploadFileAsync(file, "images/products");
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, List<IFormFile>? files, int mainImageIndex)
        {
            if (id != product.ProductId) return NotFound();

            ModelState.Remove("Category");
            ModelState.Remove("OrderDetails");
            ModelState.Remove("ProductImages");
            if (files != null)
            {
                if (files.Count > 5)
                {
                    ModelState.AddModelError("", "Chỉ được upload tối đa 5 ảnh");
                }

                foreach (var file in files)
                {
                    if (file.Length > 3 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "Mỗi ảnh tối đa 3MB");
                        break;
                    }
                }
            }
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (existingProduct == null) return NotFound();

                existingProduct.ProductName = product.ProductName;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Description = product.Description;
                existingProduct.MetaDescription = product.MetaDescription;
                existingProduct.Slug = SlugHelper.GenerateSlug(product.ProductName);

                if (files != null && files.Count > 0)
                {
                    var oldImages = _context.ProductImages.Where(x => x.ProductId == id);
                    _context.ProductImages.RemoveRange(oldImages);

                    for (int i = 0; i < files.Count; i++)
                    {
                        var fileName = await UploadFile(files[i]);

                        if (i == mainImageIndex)
                        {
                            existingProduct.ImageUrl = fileName;
                        }

                        _context.ProductImages.Add(new ProductImage
                        {
                            ProductId = id,
                            ImageUrl = fileName
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult DeleteMultiple(List<int> ids)
        {
            var items = _context.Products.Where(x => ids.Contains(x.ProductId));
            _context.Products.RemoveRange(items);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult DeleteAll()
        {
            _context.Products.RemoveRange(_context.Products);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        [HttpGet]
        public async Task<IActionResult> Sort(int categoryId)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
                return NotFound();

            var products = await _context.Products
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.DisplayOrder == null ? 1 : 0)
                .ThenBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.ProductId)
                .ToListAsync();

            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = category.CategoryName;

            return View(products);
        }
        [HttpPost]
        public async Task<IActionResult> SaveOrder(
            int categoryId, [FromBody] List<int>? ids)
        {
            if (ids == null || ids.Count == 0 ||
                ids.Distinct().Count() != ids.Count)
                return BadRequest("Danh sách ID không hợp lệ.");

            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            if (products.Count != ids.Count ||
                products.Any(p => !ids.Contains(p.ProductId)))
                return BadRequest("Danh sách sản phẩm không hợp lệ.");

            var lookup = products.ToDictionary(p => p.ProductId);

            for (int i = 0; i < ids.Count; i++)
                lookup[ids[i]].DisplayOrder = i + 1;

            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}