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
	public class SubCategoriesController : Controller
	{
		private readonly PrivateWebContext _context;

		public SubCategoriesController(PrivateWebContext context)
		{
			_context = context;
		}

		// GET: Admin/SubCategories
		public async Task<IActionResult> Index(string searchString, int? page)
		{
			int pageSize = 6; // Số lượng mục trên mỗi trang
			int pageNumber = page ?? 1;

			IQueryable<SubCategory> subcategories = _context.SubCategories;

			if (!string.IsNullOrEmpty(searchString))
			{
				subcategories = subcategories.Where(c => c.Name.Contains(searchString));
			}

			int totalCount = await subcategories.CountAsync();
			int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

			var model = await subcategories
				.OrderBy(c => c.Id)
				.OrderByDescending(c => c.CreatedAt)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			ViewData["PageCount"] = pageCount;
			ViewData["PageNumber"] = pageNumber;

			return View(model);
		}

		// GET: Admin/SubCategories/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.SubCategories == null)
			{
				return NotFound();
			}

			var subCategory = await _context.SubCategories
				.FirstOrDefaultAsync(m => m.Id == id);
			if (subCategory == null)
			{
				return NotFound();
			}

			return View(subCategory);
		}

		// GET: Admin/SubCategories/Create
		public IActionResult Create()
		{
			IQueryable<Category> categories = _context.Categories;
			ViewData["ModelCategories"] = categories;
			return View();
		}

		// POST: Admin/SubCategories/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,Slug,Status,Image,CategoryId,CreatedAt,UpdatedAt,IsFeatured,ShowHome")] SubCategory subCategory, string image_id)
		{
			subCategory.Image = "null.png";
			subCategory.CreatedAt = DateTime.Now;
			subCategory.UpdatedAt = DateTime.Now;
			if (subCategory.Status == 0)
			{
				subCategory.ShowHome = "No";
			}
			_context.Add(subCategory);
			await _context.SaveChangesAsync();
			if (!string.IsNullOrEmpty(image_id))
			{
				var tempImage = await _context.TempImages.FindAsync(int.Parse(image_id));
				if (tempImage != null)
				{
					var extArray = tempImage.Name.Split('.');
					var ext = extArray[extArray.Length - 1]; // Lấy phần mở rộng của tệp tin

					var newImageName = $"{subCategory.Id}.{ext}";
					var uploadedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
					var subCategoryDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "sub category");
					var thumbDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "sub category", "thumb");
					var sourceFilePath = Path.Combine(uploadedDirectory, tempImage.Name);
					var destinationSubCategoryFilePath = Path.Combine(subCategoryDirectory, newImageName);
					var destinationThumbFilePath = Path.Combine(thumbDirectory, newImageName);
					if (System.IO.File.Exists(sourceFilePath))
					{
						System.IO.File.Copy(sourceFilePath, destinationSubCategoryFilePath, true);
						System.IO.File.Copy(sourceFilePath, destinationThumbFilePath, true);
						subCategory.Image = newImageName;
						_context.Update(subCategory);
						await _context.SaveChangesAsync();
						System.IO.File.Delete(sourceFilePath);
					}
				}
			}
			return RedirectToAction(nameof(Index));
		}

		// GET: Admin/SubCategories/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.SubCategories == null)
			{
				return NotFound();
			}

			var subCategory = await _context.SubCategories.FindAsync(id);
			if (subCategory == null)
			{
				return NotFound();
			}

			IQueryable<Category> categories = _context.Categories;
			ViewData["ModelCategories"] = categories;
			return View(subCategory);
		}

		// POST: Admin/SubCategories/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Slug,Status,Image,CategoryId,CreatedAt,UpdatedAt,IsFeatured,ShowHome")] SubCategory subCategory, string image_id)
		{
			if (id != subCategory.Id)
			{
				return NotFound();
			}

			try
			{
				var oldImage = subCategory.Image;
				subCategory.UpdatedAt = DateTime.Now;
				if (subCategory.Status == 0)
				{
					subCategory.IsFeatured = 0;
					subCategory.ShowHome = "No";
				}
				_context.Update(subCategory);
				await _context.SaveChangesAsync();
				if (!string.IsNullOrEmpty(image_id))
				{
					var tempImage = await _context.TempImages.FindAsync(int.Parse(image_id));
					if (tempImage != null)
					{
						var extArray = tempImage.Name.Split('.');
						var ext = extArray[extArray.Length - 1];

						var newImageName = $"{subCategory.Id}-{tempImage.Id}{DateTime.Now.Ticks}.{ext}";
						var uploadedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
						var subCategoryDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "sub category");
						var thumbDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "sub category", "thumb");
						var sourceFilePath = Path.Combine(uploadedDirectory, tempImage.Name);
						var destinationSubCategoryFilePath = Path.Combine(subCategoryDirectory, newImageName);
						var destinationThumbFilePath = Path.Combine(thumbDirectory, newImageName);

						if (System.IO.File.Exists(sourceFilePath))
						{
							System.IO.File.Copy(sourceFilePath, destinationSubCategoryFilePath, true);
							System.IO.File.Copy(sourceFilePath, destinationThumbFilePath, true);
							subCategory.Image = newImageName;
							_context.Update(subCategory);
							await _context.SaveChangesAsync();
							System.IO.File.Delete(sourceFilePath);
						}

						if (!string.IsNullOrEmpty(oldImage))
						{
							var destinationSubCategoryOldFilePath = Path.Combine(subCategoryDirectory, oldImage);
							var destinationThumbOldFilePath = Path.Combine(thumbDirectory, oldImage);
							if (System.IO.File.Exists(destinationSubCategoryOldFilePath))
							{
								System.IO.File.Delete(destinationSubCategoryOldFilePath);
								System.IO.File.Delete(destinationThumbOldFilePath);
							}
						}
					}
				}
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!SubCategoryExists(subCategory.Id))
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

		// GET: Admin/SubCategories/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.SubCategories == null)
			{
				return NotFound();
			}

			var subCategory = await _context.SubCategories
				.FirstOrDefaultAsync(m => m.Id == id);
			if (subCategory == null)
			{
				return NotFound();
			}

			return View(subCategory);
		}

		// POST: Admin/SubCategories/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.SubCategories == null)
			{
				return Problem("Entity set 'PrivateWebContext.SubCategories'  is null.");
			}
			var subCategory = await _context.SubCategories.FindAsync(id);
			if (subCategory != null)
			{
				_context.SubCategories.Remove(subCategory);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool SubCategoryExists(int id)
		{
			return (_context.SubCategories?.Any(e => e.Id == id)).GetValueOrDefault();
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
			if (_context.SubCategories == null)
			{
				return Json(new { status = false, message = "Failed to update showHome" });
			}

			var subCategory = await _context.SubCategories.FindAsync(id);
			if (subCategory == null)
			{
				return Json(new { status = false, message = "Failed to update showHome" });
			}

			try
			{
				subCategory.ShowHome = showOnHome; // Chuyển đổi giá trị bool sang chuỗi "Yes" hoặc "No"
				_context.Update(subCategory);
				await _context.SaveChangesAsync();

				// Trả về một response JSON để thông báo rằng cập nhật đã thành công
				return Json(new { status = true, message = "success to update showHome" });
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!SubCategoryExists(subCategory.Id))
				{
					return Json(new { status = false, message = "Failed to update showHome" });
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
			if (_context.SubCategories == null)
			{
				return Json(new { status = false, message = "Failed to update is_featured" });
			}

			var subCategory = await _context.SubCategories.FindAsync(id);
			if (subCategory == null)
			{
				return Json(new { status = false, message = "Failed to update is_featured" });
			}

			try
			{
				subCategory.IsFeatured = isFeatured;
				_context.Update(subCategory);
				await _context.SaveChangesAsync();

				return Json(new { status = true, message = "success to update is_featured" });
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!SubCategoryExists(subCategory.Id))
				{
					return Json(new { status = false, message = "Failed to update is_featured" });
				}
				else
				{
					throw;
				}
			}
		}


	}
}
