using FashionStore.Repository;
using FashionStore.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductAdminController : Controller
    {
        private readonly fashionDbContext _context;
        // 1. Khai báo biến môi trường
        private readonly IWebHostEnvironment _webHostEnvironment;

        // 2. QUAN TRỌNG: Phải thêm tham số vào Constructor và gán giá trị
        public ProductAdminController(fashionDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // <--- THIẾU DÒNG NÀY LÀ BỊ LỖI NULL NGAY
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


            // 1️⃣ Ảnh đại diện
            if (files != null && files.Count > 0)
            {
                product.ImageUrl = await UploadFile(files[0]);
            }
            else
            {
                product.ImageUrl = "/images/no-image.png";
            }

            // 2️⃣ LƯU PRODUCT TRƯỚC
            _context.Products.Add(product);
            await _context.SaveChangesAsync(); // 🔥 BẮT BUỘC

            // 3️⃣ LƯU NHIỀU ẢNH
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
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();

            // Bỏ qua kiểm tra
            ModelState.Remove("Category");
            ModelState.Remove("OrderDetails");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId)) return NotFound();
                    else throw;
                }
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
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

    }
}