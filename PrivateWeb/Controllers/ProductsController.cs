using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrivateWeb.Areas.Identity.Data;
using PrivateWeb.Models;

namespace PrivateWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly PrivateWebContext _context;
        private readonly UserManager<PrivateWebUser> userManager;
        public ProductsController(PrivateWebContext context, ILogger<ProductsController> logger, UserManager<PrivateWebUser> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Promotion)
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(p => p.Id == productId && p.ShowHome == "Yes");

            if (product == null)
            {
                return NotFound(); // Trả về 404 Not Found nếu không tìm thấy sản phẩm
            }

            var variantss = await _context.Variantsses
                .Include(v => v.Size)
                .Include(v => v.Color)
                .Where(v => v.ProductId == productId && v.Status == 1)
                .ToListAsync();

            var distinctColors = variantss.GroupBy(v => v.ColorId)
                                .Select(g => g.First())
                                .ToList();

            var categories = _context.Categories
                                   .Where(c => c.ShowHome == "Yes")
                                   .ToList();

            ViewData["Categories"] = categories;
            ViewData["Variantsses"] = variantss;
            ViewData["Product"] = product;
            ViewData["ColorList"] = distinctColors;
            return View();
        }

        public async Task<IActionResult> UpdateCart(int indexCart, int productId)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Promotion)
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(p => p.Id == productId && p.ShowHome == "Yes");

            if (product == null)
            {
                return NotFound(); // Trả về 404 Not Found nếu không tìm thấy sản phẩm
            }

            var variantss = await _context.Variantsses
                .Include(v => v.Size)
                .Include(v => v.Color)
                .Where(v => v.ProductId == productId && v.Status == 1)
                .ToListAsync();

            var distinctColors = variantss.GroupBy(v => v.ColorId)
                                .Select(g => g.First())
                                .ToList();


            ViewData["Variantsses"] = variantss;
            ViewData["Product"] = product;
            ViewData["ColorList"] = distinctColors;
            return View("updateCart");

        }


        [HttpPost]
        public IActionResult AddToCartDetails(int productId, int colorId, int sizeId)
        {
            try
            {
                var variantDetails = (from variant in _context.Variantsses
                                      join color in _context.Colors on variant.ColorId equals color.Id
                                      join size in _context.Sizes on variant.SizeId equals size.Id
                                      where variant.ProductId == productId && variant.Status == 1 && variant.ColorId == colorId && variant.SizeId == sizeId
                                      select new
                                      {
                                          Variant = variant,
                                          Color = color.Name,
                                          Size = size.Name
                                      }).FirstOrDefault();


                if (variantDetails != null)
                {
                    return Json(new
                    {
                        success = true,
                        message = "success add to cart.",
                        variant = variantDetails.Variant,
                        color = variantDetails.Color,
                        size = variantDetails.Size
                    });
                }

                return Json(new { success = false, message = "product not found." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }

    }
}
