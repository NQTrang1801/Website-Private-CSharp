using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrivateWeb.Models;

namespace PrivateWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VariantssesController : Controller
    {
        private readonly PrivateWebContext _context;

        public VariantssesController(PrivateWebContext context)
        {
            _context = context;
        }

        // GET: Admin/Variantsses
        public async Task<IActionResult> Index()
        {
            var privateWebContext = _context.Variantsses.Include(v => v.Color).Include(v => v.Product).Include(v => v.Promotion).Include(v => v.Size);
            return View(await privateWebContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,Title,Slug,ProductId,SizeId,ColorId,PromotionId,Image,Quantity,Price,Status,CreatedAt,UpdatedAt")] Variantss variantss)
        {
            if (ModelState.IsValid)
            {
                _context.Add(variantss);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ColorId"] = new SelectList(_context.Colors, "Id", "Id", variantss.ColorId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", variantss.ProductId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "Id", variantss.PromotionId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "Id", "Id", variantss.SizeId);
            return View(variantss);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Slug,ProductId,SizeId,ColorId,PromotionId,Image,Quantity,Price,Status,CreatedAt,UpdatedAt")] Variantss variantss)
        {
            if (id != variantss.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variantss);
                    await _context.SaveChangesAsync();
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
            ViewData["ColorId"] = new SelectList(_context.Colors, "Id", "Id", variantss.ColorId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", variantss.ProductId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "Id", variantss.PromotionId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "Id", "Id", variantss.SizeId);
            return View(variantss);
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
            if (variantss != null)
            {
                _context.Variantsses.Remove(variantss);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VariantssExists(int id)
        {
          return (_context.Variantsses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
