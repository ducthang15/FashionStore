using FashionStore.Utilities;
using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace FashionStore.Areas.Admin.Controllers
{
    public class ProductAdminController : BaseAdminController
    {
        private readonly fashionDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductAdminController(fashionDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product, List<IFormFile> files)
        {
            ModelState.Remove("Category");
            ModelState.Remove("OrderDetails");
            ModelState.Remove("ProductImages");
            ModelState.Remove("ImageUrl");

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(
                    _context.Categories,
                    "CategoryId",
                    "CategoryName"
                );

                return View(product ?? new Product());
            }
            if (files != null)
            {
                if (files.Count > 5)
                {
                    ModelState.AddModelError("", "Chỉ được upload tối đa 5 ảnh");
                }

                foreach (var file in files)
                {
                    if (file.Length > 3 * 1024 * 1024) // 3MB
                    {
                        ModelState.AddModelError("", "Mỗi ảnh tối đa 3MB");
                        break;
                    }
                }
            }
            product.Slug = SlugHelper.GenerateSlug(product.ProductName);
            if (_context.Products.Any(p => p.Slug == product.Slug))
            {
                product.Slug += "-" + DateTime.Now.Ticks;
            }
            if (files != null && files.Count > 0)
            {
                product.ImageUrl = await UploadFile(files[0]);
            }
            else
            {
                product.ImageUrl = "/images/no-image.png";
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            //LƯU NHIỀU ẢNH
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var fileName = await UploadFile(file);

                    var productImage = new ProductImage
                    {
                        ImageUrl = fileName,
                        ProductId = product.ProductId
                    };

                    _context.ProductImages.Add(productImage);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private async Task<string?> UploadFile(IFormFile file)
        {
            string fileName = null;
            if (file != null)
            {
                // Tạo tên file ngẫu nhiên để không trùng (VD: kjh123-anh.jpg)
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                string filePath = Path.Combine(uploadDir, fileName);

                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return "/images/products/" + fileName;
            }
            return null;
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
        public async Task<IActionResult> Edit(int id, Product product, List<IFormFile>? files)
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
                existingProduct.Slug = SlugHelper.GenerateSlug(product.ProductName);

                if (files != null && files.Count > 0)
                {
                    existingProduct.ImageUrl = await UploadFile(files[0]);
                    var oldImages = _context.ProductImages.Where(x => x.ProductId == id);
                    _context.ProductImages.RemoveRange(oldImages);
                    foreach (var file in files)
                    {
                        var fileName = await UploadFile(file);

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

    }
}