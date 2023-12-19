using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PrivateWeb.Models;

namespace PrivateWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly PrivateWebContext _context;

        public ProductsController(PrivateWebContext context)
        {
            _context = context;
        }

		// GET: Admin/Products
		public async Task<IActionResult> Index(string searchString, int? page)
		{
			int pageSize = 10;
			int pageNumber = page ?? 1;

			IQueryable<Product> products = _context.Products
		            .Include(p => p.Category)
		            .Include(p => p.SubCategory)
		            .Include(p => p.Promotion)
		            .Include(p => p.Images);

			if (!string.IsNullOrEmpty(searchString))
			{
				products = products.Where(p =>
					p.Title.Contains(searchString) ||
					(p.Keywords != null && p.Keywords.Contains(searchString)) ||
					(p.Category != null && p.Category.Name.Contains(searchString)) ||
					(p.SubCategory != null && p.SubCategory.Name.Contains(searchString))
				);
			}


			int totalCount = await products.CountAsync();
			int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

			var model = await products
				.OrderBy(c => c.Id)
				.OrderByDescending(c => c.CreatedAt)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			ViewData["PageCount"] = pageCount;
			ViewData["PageNumber"] = pageNumber;

			return View(model);
		}


		// GET: Admin/Products/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            IQueryable<Category> categories = _context.Categories;
			IQueryable<SubCategory> subCategories = _context.SubCategories;
			IQueryable<Promotion> promotions = _context.Promotions;

            ViewData["ModelCategories"] = categories;
			ViewData["ModelSubCategories"] = subCategories;
			ViewData["ModelPromotions"] = promotions;
            return View();
        }

		// POST: Admin/Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Title,Slug,Keywords,Description,Detail,Care,Price,Amount,Status,CategoryId,SubCategoryId,PromotionId,ImagesId,ShowHome,IsFeatured,CreatedAt,UpdatedAt")] Product product, string image1, string image2, string image3, string image4)
		{
            if (string.IsNullOrEmpty(product.Title))
            {
                ModelState.AddModelError("Title", "Invalid Title.");
            }

            if (string.IsNullOrEmpty(product.Slug))
            {
                ModelState.AddModelError("Slug", "Invalid Slug. ");
            }

            if (product.Price <= 0)
            {
                ModelState.AddModelError("Price", "Invalid product Price.");
            }

            if (product.Amount < 0)
            {
                ModelState.AddModelError("Amount", "Invalid product quantity.");
            }

            if (product.Price == 0)
            {
                ModelState.AddModelError("Price", "Please enter product price.");
            }

            if (product.Amount == 0)
            {
                ModelState.AddModelError("Amount", "Please enter product quantity.");
            }

            // Kiểm tra xem có lỗi nào không
            if (ModelState.ErrorCount > 0)
            {
                IQueryable<Category> categories = _context.Categories;
                IQueryable<SubCategory> subCategories = _context.SubCategories;
                IQueryable<Promotion> promotions = _context.Promotions;

                ViewData["ModelCategories"] = categories;
                ViewData["ModelSubCategories"] = subCategories;
                ViewData["ModelPromotions"] = promotions;
                return View(product);
            }
            product.CreatedAt = DateTime.Now;
				product.UpdatedAt = DateTime.Now;
				product.Images = new ProductsImage();
				product.Images.Image1 = "null.png";
				if (product.Status == 0)
				{
					product.ShowHome = "No";
					product.IsFeatured = 0;
				}
				_context.Add(product.Images);
				await _context.SaveChangesAsync();
				product.ImagesId = product.Images.Id;
				_context.Add(product);
				await _context.SaveChangesAsync();

				if (!string.IsNullOrEmpty(image1) && !image1.Equals("0"))
				{
					// Logic to handle image 1
					string newImageName1 = await HandleProductImage(product.Id, image1, 1);
					product.Images.Image1 = newImageName1;
				}

				if (!string.IsNullOrEmpty(image2) && !image2.Equals("0"))
				{
					// Logic to handle image 2
					string newImageName2 = await HandleProductImage(product.Id, image2, 2);
					product.Images.Image2 = newImageName2;
				} else
				{
					product.Images.Image2 = "null.png";
				}

				if (!string.IsNullOrEmpty(image3) && !image3.Equals("0"))
				{
					// Logic to handle image 3
					string newImageName3 = await HandleProductImage(product.Id, image3, 3);
					product.Images.Image3 = newImageName3;
				}
				else
				{
					product.Images.Image3 = "null.png";
				}

				if (!string.IsNullOrEmpty(image4) && !image4.Equals("0"))
				{
					// Logic to handle image 4
					string newImageName4 = await HandleProductImage(product.Id, image4, 4);
					product.Images.Image4 = newImageName4;
				}
				else
				{
					product.Images.Image4 = "null.png";
				}

				_context.Update(product.Images);
				await _context.SaveChangesAsync();
				var allTempImages = _context.TempImages.ToList();
				_context.TempImages.RemoveRange(allTempImages);
				await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
		    }

		private async Task<string> HandleProductImage(int productId, string imageId, int imageNumber)
		{
			var tempImage = await _context.TempImages.FindAsync(int.Parse(imageId));
			if (tempImage != null)
			{
				var extArray = tempImage.Name.Split('.');
				var ext = extArray[extArray.Length - 1]; 

				var newImageName = $"{productId}-{imageNumber}.{ext}";
				var uploadedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
				var productDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "product", "products", "products images");
				var thumbDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "product", "products", "thumb");
				var sourceFilePath = Path.Combine(uploadedDirectory, tempImage.Name);
				var destinationProductFilePath = Path.Combine(productDirectory, newImageName);
				var destinationThumbFilePath = Path.Combine(thumbDirectory, newImageName);
				if (System.IO.File.Exists(sourceFilePath))
				{
					System.IO.File.Copy(sourceFilePath, destinationProductFilePath, true);
					System.IO.File.Copy(sourceFilePath, destinationThumbFilePath, true);
                    return newImageName;
				}
			}
            return "null.png";
		}


		// GET: Admin/Products/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
				.Where(p => p.Id == id)
				.Include(p => p.Category)
				.Include(p => p.SubCategory)
				.Include(p => p.Promotion)
				.Include(p => p.Images)
                .Include(p => p.Variantsses)
                .Include(p => p.Variantsses).ThenInclude(v => v.Promotion)
                .Include(p => p.Variantsses).ThenInclude(v => v.Size)
                .Include(p => p.Variantsses).ThenInclude(v => v.Color)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            IQueryable<Category> categories = _context.Categories;
            IQueryable<SubCategory> subCategories = _context.SubCategories;
            IQueryable<Promotion> promotions = _context.Promotions;
            IQueryable<Size> sizes = _context.Sizes;
            IQueryable<Color> colors = _context.Colors;

            ViewData["ModelCategories"] = categories;
            ViewData["ModelSubCategories"] = subCategories;
            ViewData["ModelPromotions"] = promotions;
            ViewData["ModelSizes"] = sizes;
            ViewData["ModelColors"] = colors;
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Slug,Keywords,Description,Detail,Care,Price,Amount,Status,CategoryId,SubCategoryId,PromotionId,ImagesId,ShowHome,IsFeatured,CreatedAt,UpdatedAt")] Product product, string image1, string image2, string image3, string image4)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(product.Title))
            {
                ModelState.AddModelError("Title", "Invalid Title.");
            }

            if (string.IsNullOrEmpty(product.Slug))
            {
                ModelState.AddModelError("Slug", "Invalid Slug. ");
            }

            if (product.Price <= 0)
            {
                ModelState.AddModelError("Price", "Invalid product Price.");
            }

            if (product.Amount < 0)
            {
                ModelState.AddModelError("Amount", "Invalid product quantity.");
            }

            if (product.Price == 0)
            {
                ModelState.AddModelError("Price", "Please enter product price.");
            }

            if (product.Amount == 0)
            {
                ModelState.AddModelError("Amount", "Please enter product quantity.");
            }

            if (ModelState.ErrorCount > 0)
            {
                IQueryable<Category> categories = _context.Categories;
                IQueryable<SubCategory> subCategories = _context.SubCategories;
                IQueryable<Promotion> promotions = _context.Promotions;
                IQueryable<Size> sizes = _context.Sizes;
                IQueryable<Color> colors = _context.Colors;

                ViewData["ModelCategories"] = categories;
                ViewData["ModelSubCategories"] = subCategories;
                ViewData["ModelPromotions"] = promotions;
                ViewData["ModelSizes"] = sizes;
                ViewData["ModelColors"] = colors;
                return View(product);
            }
			try
			{
				product.UpdatedAt = DateTime.Now;
				if (product.Status == 0)
				{
					product.ShowHome = "No";
					product.IsFeatured = 0;
				}
				_context.Update(product);
				await _context.SaveChangesAsync();
				var productImages = await _context.ProductsImages.FindAsync(product.ImagesId);
                if (productImages != null)
				{
                    if (!string.IsNullOrEmpty(image1) && !image1.Equals("0") )
					{
						// Logic to handle image 1
						string newImageName1 = await HandleProductUpdateImage(product.Id, image1, 1);
                        productImages.Image1 = newImageName1;
					}

					if (!string.IsNullOrEmpty(image2) && !image2.Equals("0"))
					{
						// Logic to handle image 2
						string newImageName2 = await HandleProductUpdateImage(product.Id, image2, 2);
                        productImages.Image2 = newImageName2;
					}

					if (!string.IsNullOrEmpty(image3) && !image3.Equals("0"))
					{
						// Logic to handle image 3
						string newImageName3 = await HandleProductUpdateImage(product.Id, image3, 3);
                        productImages.Image3 = newImageName3;
					}

					if (!string.IsNullOrEmpty(image4) && !image4.Equals("0"))
					{
						// Logic to handle image 4
						string newImageName4 = await HandleProductUpdateImage(product.Id, image4, 4);
                        productImages.Image4 = newImageName4;
					}

					_context.Update(productImages);
					await _context.SaveChangesAsync();
					var allTempImages = _context.TempImages.ToList();
					_context.TempImages.RemoveRange(allTempImages);
					await _context.SaveChangesAsync();
				}
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductExists(product.Id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
                return RedirectToAction(nameof(Index));
        }

        private async Task<string> HandleProductUpdateImage(int productId, string imageId, int imageNumber)
        {
            var tempImage = await _context.TempImages.FindAsync(int.Parse(imageId));
            if (tempImage != null)
            {
                var extArray = tempImage.Name.Split('.');
                var ext = extArray[extArray.Length - 1];

                var newImageName = $"{productId}-{imageNumber}-{DateTime.Now.Ticks}.{ext}";
                var uploadedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
                var productDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "product", "products", "products images");
                var thumbDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "product", "products", "thumb");
                var sourceFilePath = Path.Combine(uploadedDirectory, tempImage.Name);
                var destinationProductFilePath = Path.Combine(productDirectory, newImageName);
                var destinationThumbFilePath = Path.Combine(thumbDirectory, newImageName);
                if (System.IO.File.Exists(sourceFilePath))
                {
                    System.IO.File.Copy(sourceFilePath, destinationProductFilePath, true);
                    System.IO.File.Copy(sourceFilePath, destinationThumbFilePath, true);
                    return newImageName;
                }
            }
            return "null.png";
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'PrivateWebContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

		[HttpPost]
		public async Task<IActionResult> UploadFiles(IFormFile filedata)
		{
			var files = HttpContext.Request.Form.Files;
			if (files.Any())
			{
				try
				{
					foreach (var file in files)
					{
						if (file.Length > 0)
						{
							// Getting FileName
							var fileName = Path.GetFileName(file.FileName);
							// Assigning Unique Filename (Guid)
							var myUniqueFileName = Convert.ToString(Guid.NewGuid().ToString("N"));
							// Getting file Extension
							var ext = Path.GetExtension(fileName);
							// Concatenating FileName + FileExtension
							var tempImage = new TempImage
							{
								Name = "null.png",
								CreatedAt = DateTime.Now,
								UpdatedAt = DateTime.Now
							};
							_context.TempImages.Add(tempImage);
							await _context.SaveChangesAsync();
							var newFileName = $"{tempImage.Id}{DateTime.Now.Ticks}{ext}";
							tempImage.Name = newFileName;
							_context.TempImages.Update(tempImage);
							await _context.SaveChangesAsync();

							var physicalPath = $"{new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles")).Root}{$@"{newFileName}"}";
							string filePath = $"/UploadedFiles/{newFileName}";

							await using var target = new MemoryStream();
							await file.CopyToAsync(target);

							// Using FileStream and properly disposing it
							using (FileStream fs = System.IO.File.Create(physicalPath))
							{
								target.Seek(0, SeekOrigin.Begin);
								await target.CopyToAsync(fs);
								fs.Flush();
							}
							return Json(new { status = true, image_id = tempImage.Id, Message = "Files Uploaded Successfully!" });
						}
					}
				}
				catch (Exception ex)
				{
					return Json(new { status = false, Message = $"Error: {ex.Message}" });
				}
			}
			return Json(new { status = false, Message = "No files were uploaded!" });
		}


		[HttpPost]
		public async Task<JsonResult> UpdateShowHome(int id, string showOnHome)
		{
			if (_context.Products == null)
			{
				return Json(new { success = false, message = "ShowHome update failed" });
			}

			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return Json(new { success = false, message = "ShowHome update failed" });
			}

			try
			{
				product.ShowHome = showOnHome;
				_context.Update(product);
				await _context.SaveChangesAsync();


				return Json(new { success = true, message = "ShowHome update success" });
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductExists(product.Id))
				{
					return Json(new { success = false, message = "ShowHome update failed" });
				}
				else
				{
					throw;
				}
			}
		}

		[HttpPost]
		public async Task<JsonResult> UpdateIsFeatured(int id, int isFeatured)
		{
			if (_context.Products == null)
			{
				return Json(new { success = false, message = "IsFeatured update failed" });
			}

			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return Json(new { success = false, message = "IsFeatured update failed" });
			}

			try
			{
				product.IsFeatured = isFeatured;
				_context.Update(product);
				await _context.SaveChangesAsync();

				return Json(new { success = true, message = "IsFeatured update success" });
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductExists(product.Id))
				{
					return Json(new { success = false, message = "IsFeatured update failed" });
				}
				else
				{
					throw;
				}
			}
		}
	}

}
