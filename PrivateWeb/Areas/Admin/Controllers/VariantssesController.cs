using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PrivateWeb.Models;

namespace PrivateWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    [Area("Admin")]
    public class VariantssesController : Controller
    {
        private readonly PrivateWebContext _context;

        public VariantssesController(PrivateWebContext context)
        {
            _context = context;
        }

        // GET: Admin/Variantsses
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            int pageSize = 6; // Số lượng mục trên mỗi trang
            int pageNumber = page ?? 1;

            IQueryable<Variantss> variantss = _context.Variantsses
                    .Include(p => p.Product)
                    .Include(p => p.Size)
                    .Include(p => p.Color)
                    .Include(p => p.Promotion);

            if (!string.IsNullOrEmpty(searchString))
            {
                variantss = variantss.Where(c => c.Title.Contains(searchString));
                variantss = variantss.Where(p =>
                    p.Title.Contains(searchString) ||
                    (p.Product.Title != null && p.Product.Title.Contains(searchString))
                );
            }

            int totalCount = await variantss.CountAsync();
            int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

            var model = await variantss
                .OrderBy(c => c.Id)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["PageCount"] = pageCount;
            ViewData["PageNumber"] = pageNumber;

            return View(model);
        }

        // GET: Admin/Variantsses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Variantsses == null)
            {
                return NotFound();
            }

            var variantss = await _context.Variantsses
                .Include(v => v.Color)
                .Include(v => v.Product)
                .Include(v => v.Promotion)
                .Include(v => v.Size)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (variantss == null)
            {
                return NotFound();
            }

            return View(variantss);
        }

        // GET: Admin/Variantsses/Create
        public IActionResult Create()
        {
            ViewData["ColorId"] = new SelectList(_context.Colors, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "Id");
            ViewData["SizeId"] = new SelectList(_context.Sizes, "Id", "Id");
            return View();
        }

        // POST: Admin/Variantsses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Slug,ProductId,SizeId,ColorId,PromotionId,Image,Quantity,Price,Status,CreatedAt,UpdatedAt")] Variantss variantss, string imageId)
        {
            variantss.Image = "null.png";
            variantss.CreatedAt = DateTime.Now;
            variantss.UpdatedAt = DateTime.Now;
           
            _context.Add(variantss);
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(imageId) && !imageId.Equals("0"))
            {
                var tempImage = await _context.TempImages.FindAsync(int.Parse(imageId));
                if (tempImage != null)
                {
                    var extArray = tempImage.Name.Split('.');
                    var ext = extArray[extArray.Length - 1]; // Lấy phần mở rộng của tệp tin

                    var newImageName = $"{variantss.Id}.{ext}";
                    var uploadedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
                    var variantDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "product", "variantss");
                    var thumbDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "product", "variantss", "thumb");
                    var sourceFilePath = Path.Combine(uploadedDirectory, tempImage.Name);
                    var destinationVariantFilePath = Path.Combine(variantDirectory, newImageName);
                    var destinationThumbFilePath = Path.Combine(thumbDirectory, newImageName);
                    if (System.IO.File.Exists(sourceFilePath))
                    {
                        System.IO.File.Copy(sourceFilePath, destinationVariantFilePath, true);
                        System.IO.File.Copy(sourceFilePath, destinationThumbFilePath, true);
                        variantss.Image = newImageName;
                        _context.Update(variantss);
                        await _context.SaveChangesAsync();
                        System.IO.File.Delete(sourceFilePath);
                    }
                }
            }
            int productId = variantss.ProductId;
            // Chuyển hướng đến action Edit của ProductsController với Id của sản phẩm
            return RedirectToAction("Edit", "Products", new { id = productId });
        }

        // GET: Admin/Variantsses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Variantsses == null)
            {
                return NotFound();
            }

            var variantss = await _context.Variantsses.FindAsync(id);
            if (variantss == null)
            {
                return NotFound();
            }
            ViewData["ColorId"] = new SelectList(_context.Colors, "Id", "Id", variantss.ColorId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", variantss.ProductId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "Id", variantss.PromotionId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "Id", "Id", variantss.SizeId);
            return View(variantss);
        }

        // POST: Admin/Variantsses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Slug,ProductId,SizeId,ColorId,PromotionId,Image,Quantity,Price,Status,CreatedAt,UpdatedAt")] Variantss variantss, string imageId)
        {
                int productId = variantss.ProductId;
                if (id != variantss.Id)
                {
                    return NotFound();
                }
                try
                {

                var oldImage = variantss.Image;
                variantss.UpdatedAt = DateTime.Now;
                _context.Update(variantss);
                await _context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(imageId) && !imageId.Equals("0"))
                {
                    var tempImage = await _context.TempImages.FindAsync(int.Parse(imageId));
                    if (tempImage != null)
                    {
                        var extArray = tempImage.Name.Split('.');
                        var ext = extArray[extArray.Length - 1];

                        var newImageName = $"{variantss.Id}-{tempImage.Id}{DateTime.Now.Ticks}.{ext}";
                        var uploadedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
                        var variantDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "product", "variantss");
                        var thumbDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadHome", "product", "variantss", "thumb");
                        var sourceFilePath = Path.Combine(uploadedDirectory, tempImage.Name);
                        var destinationFilePath = Path.Combine(variantDirectory, newImageName);
                        var destinationThumbFilePath = Path.Combine(thumbDirectory, newImageName);

                        if (System.IO.File.Exists(sourceFilePath))
                        {
                            System.IO.File.Copy(sourceFilePath, destinationFilePath, true);
                            System.IO.File.Copy(sourceFilePath, destinationThumbFilePath, true);
                            variantss.Image = newImageName;
                            _context.Update(variantss);
                            await _context.SaveChangesAsync();
                            System.IO.File.Delete(sourceFilePath);
                        }

                        if (!string.IsNullOrEmpty(oldImage))
                        {
                            var destinationOldFilePath = Path.Combine(variantDirectory, oldImage);
                            var destinationThumbOldFilePath = Path.Combine(thumbDirectory, oldImage);
                            if (System.IO.File.Exists(destinationOldFilePath))
                            {
                                System.IO.File.Delete(destinationOldFilePath);
                                System.IO.File.Delete(destinationThumbOldFilePath);
                            }
                        }
                    }
                }
                return RedirectToAction("Edit", "Products", new { id = productId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VariantssExists(variantss.Id))
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

        // GET: Admin/Variantsses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Variantsses == null)
            {
                return NotFound();
            }

            var variantss = await _context.Variantsses
                .Include(v => v.Color)
                .Include(v => v.Product)
                .Include(v => v.Promotion)
                .Include(v => v.Size)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (variantss == null)
            {
                return NotFound();
            }

            return View(variantss);
        }

        // POST: Admin/Variantsses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Variantsses == null)
            {
                return Problem("Entity set 'PrivateWebContext.Variantsses'  is null.");
            }
            var variantss = await _context.Variantsses.FindAsync(id);
            var idP = variantss?.ProductId;
            if (variantss != null)
            {
                _context.Variantsses.Remove(variantss);
                await _context.SaveChangesAsync();
            }
            
           
            return View(variantss);
        }

        private bool VariantssExists(int id)
        {
          return (_context.Variantsses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
