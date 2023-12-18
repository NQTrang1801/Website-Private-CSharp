using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PrivateWeb.Models;
using static System.Net.Mime.MediaTypeNames;

namespace PrivateWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly PrivateWebContext _context;

        public CategoriesController(PrivateWebContext context)
        {
            _context = context;
        }

		// GET: Admin/Categories
		/*        public async Task<IActionResult> Index()
				{
					return _context.Categories != null ?
								View(await _context.Categories.ToListAsync()) :
								Problem("Entity set 'PrivateWebContext.Categories'  is null.");
				}*/

		public async Task<IActionResult> Index(string searchString, int? page)
		{
			int pageSize = 6; // Số lượng mục trên mỗi trang
			int pageNumber = page ?? 1;

			IQueryable<Category> categories = _context.Categories;

			if (!string.IsNullOrEmpty(searchString))
			{
				categories = categories.Where(c => c.Name.Contains(searchString));
			}

			int totalCount = await categories.CountAsync();
			int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

			var model = await categories
				.OrderBy(c => c.Id)
    				.OrderByDescending(c => c.CreatedAt)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			ViewData["PageCount"] = pageCount;
			ViewData["PageNumber"] = pageNumber;

			return View(model);
		}


		// GET: Admin/Categories/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Slug,Image,IsFeatured,Status,ShowHome,CreatedAt,UpdatedAt")] Category category, string image_id)
        {
            category.Image = "null.png";
            category.CreatedAt = DateTime.Now;
            category.UpdatedAt = DateTime.Now;
            if (category.Status == 0)
            {
                category.ShowHome = "No";
            }
            _context.Add(category);
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(image_id))
            {
                var tempImage = await _context.TempImages.FindAsync(int.Parse(image_id));
                if (tempImage != null)
                {
                    var extArray = tempImage.Name.Split('.');
                    var ext = extArray[extArray.Length - 1]; // Lấy phần mở rộng của tệp tin

                    var newImageName = $"{category.Id}.{ext}";
                    var uploadedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
                    var categoryDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "category");
                    var thumbDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "category", "thumb");
                    var sourceFilePath = Path.Combine(uploadedDirectory, tempImage.Name);
                    var destinationCategoryFilePath = Path.Combine(categoryDirectory, newImageName);
                    var destinationThumbFilePath = Path.Combine(thumbDirectory, newImageName);
                    if (System.IO.File.Exists(sourceFilePath))
                    {
                        System.IO.File.Copy(sourceFilePath, destinationCategoryFilePath, true);
                        System.IO.File.Copy(sourceFilePath, destinationThumbFilePath, true);
                        category.Image = newImageName;
                        _context.Update(category);
                        await _context.SaveChangesAsync();
                        System.IO.File.Delete(sourceFilePath);
                    }
                }
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Slug,Image,IsFeatured,Status,ShowHome,CreatedAt,UpdatedAt")] Category category, string image_id)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            try
            {
                var oldImage = category.Image;
                category.UpdatedAt = DateTime.Now;
                if (category.Status == 0)
                {
                    category.IsFeatured = 0;
                    category.ShowHome = "No";
                }
                _context.Update(category);
                await _context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(image_id))
                {
                    var tempImage = await _context.TempImages.FindAsync(int.Parse(image_id));
                    if (tempImage != null)
                    {
                        var extArray = tempImage.Name.Split('.');
                        var ext = extArray[extArray.Length - 1];

                        var newImageName = $"{category.Id}-{tempImage.Id}{DateTime.Now.Ticks}.{ext}";
                        var uploadedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
                        var categoryDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "category");
                        var thumbDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "category", "thumb");
                        var sourceFilePath = Path.Combine(uploadedDirectory, tempImage.Name);
                        var destinationCategoryFilePath = Path.Combine(categoryDirectory, newImageName);
                        var destinationThumbFilePath = Path.Combine(thumbDirectory, newImageName);

                        if (System.IO.File.Exists(sourceFilePath))
                        {
                            System.IO.File.Copy(sourceFilePath, destinationCategoryFilePath, true);
                            System.IO.File.Copy(sourceFilePath, destinationThumbFilePath, true);
                            category.Image = newImageName;
                            _context.Update(category);
                            await _context.SaveChangesAsync();
                            System.IO.File.Delete(sourceFilePath);
                        }

                        if (!string.IsNullOrEmpty(oldImage))
                        {
                            var destinationCategoryOldFilePath = Path.Combine(categoryDirectory, oldImage);
                            var destinationThumbOldFilePath = Path.Combine(thumbDirectory, oldImage);
                            if (System.IO.File.Exists(destinationCategoryOldFilePath))
                            {
                                System.IO.File.Delete(destinationCategoryOldFilePath);
                                System.IO.File.Delete(destinationThumbOldFilePath);
                            }
                        }
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
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

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'PrivateWebContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
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
			if (_context.Categories == null)
			{
				return Json(new { success = false, message = "Cập nhật ShowHome thất bại" });
			}

			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return Json(new { success = false, message = "Cập nhật ShowHome thất bại" });
			}

			try
			{
				category.ShowHome = showOnHome; // Chuyển đổi giá trị bool sang chuỗi "Yes" hoặc "No"
				_context.Update(category);
				await _context.SaveChangesAsync();

				// Trả về một response JSON để thông báo rằng cập nhật đã thành công
				return Json(new { success = true, message = "Cập nhật ShowHome thành công" });
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CategoryExists(category.Id))
				{
					return Json(new { success = false, message = "Cập nhật ShowHome thất bại" });
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
			if (_context.Categories == null)
			{
				return Json(new { success = false, message = "Cập nhật isFeatured thất bại" });
			}

			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return Json(new { success = false, message = "Cập nhật isFeatured thất bại" });
			}

			try
			{
				category.IsFeatured = isFeatured;
				_context.Update(category);
				await _context.SaveChangesAsync();

				return Json(new { success = true, message = "Cập nhật isFeatured thành công" });
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CategoryExists(category.Id))
				{
					return Json(new { success = false, message = "Cập nhật isFeatured thất bại" });
				}
				else
				{
					throw;
				}
			}
		}


	}
}
