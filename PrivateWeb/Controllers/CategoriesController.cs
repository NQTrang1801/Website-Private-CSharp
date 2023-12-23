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
    public class CategoriesController : Controller
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly UserManager<PrivateWebUser> userManager;
        private readonly PrivateWebContext _context;

        public CategoriesController(PrivateWebContext context, ILogger<CategoriesController> logger, UserManager<PrivateWebUser> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
            _context = context;
        }

        // GET: Categories
        public IActionResult Index(string categoryName, int categoryId)
        {
            var productsInCategory = _context.Products
                            .Where(p => p.CategoryId == categoryId &&
                                        p.ShowHome == "Yes" &&
                                        p.Variantsses.Any())
                            .Include(p => p.Promotion)
                            .Include(p => p.Images)
                            .ToList();


            var specialPrices = _context.Products
                        .Where(p => p.CategoryId == categoryId &&
                                    p.ShowHome == "Yes" &&
                                    p.PromotionId != null &&
                                    p.Variantsses.Any()) // Kiểm tra sản phẩm có ít nhất một biến thể
                        .Include(p => p.Promotion)
                        .Include(p => p.Images)
                        .OrderByDescending(p => p.Promotion.Value)
                        .ToList();



            var subCategories = _context.SubCategories
                                    .Where(c => c.CategoryId == categoryId && c.ShowHome == "Yes")
                                    .ToList();

            var categories = _context.Categories
                                    .Where(c => c.ShowHome == "Yes")
                                    .ToList();

            ViewData["Categories"] = categories;
            ViewData["SubCategories"] = subCategories;
            ViewData["specialPrices"] = specialPrices;
            ViewData["NamePage"] = categoryName;
            ViewData["Products"] = productsInCategory;
            return View(productsInCategory);
        }

        [HttpPost]
        public IActionResult AddToCartDefault(int productId)
        {
            try
            {
                var variantDetails = (from variant in _context.Variantsses
                                      join color in _context.Colors on variant.ColorId equals color.Id
                                      join size in _context.Sizes on variant.SizeId equals size.Id
                                      where variant.ProductId == productId && variant.Status == 1
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
