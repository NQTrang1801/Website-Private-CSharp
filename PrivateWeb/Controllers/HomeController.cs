using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrivateWeb.Areas.Identity.Data;
using PrivateWeb.Models;
using System.Diagnostics;

namespace PrivateWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<PrivateWebUser> userManager;
        private readonly PrivateWebContext _context;

        public HomeController(PrivateWebContext context, ILogger<HomeController> logger, UserManager<PrivateWebUser> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["UserID"]=userManager.GetUserId(this.User);
            var categories = _context.Categories.Where(c => c.ShowHome == "Yes").ToList();
            ViewData["Categories"] = categories;

            var productsWithPromotion = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Promotion)
                .Include(p => p.SubCategory)
                .Where(p => p.ShowHome == "Yes" && p.Promotion != null && p.Promotion.Value > 0.3m)
                .ToList();

            ViewData["Products"] = productsWithPromotion;
            return View("Index/index");
        }

        public IActionResult Categories()
        {
            ViewData["UserID"] = userManager.GetUserId(this.User);
            return View("Categories/categories");
        }

        public IActionResult Cart()
        {
            var categories = _context.Categories
                                    .Where(c => c.ShowHome == "Yes")
                                    .ToList();
            ViewData["UserID"] = userManager.GetUserId(this.User);
            ViewData["Categories"] = categories;
            return View("Cart/cart");
        }

        public IActionResult ProductDetails()
        {
            ViewData["UserID"] = userManager.GetUserId(this.User);
            return View("ProductDetails/productDetails");
        }

        public IActionResult CheckOut()
        {
            ViewData["UserID"] = userManager.GetUserId(this.User);
            return View("CheckOut/checkout");
        }

        public IActionResult Profile()
        {
            ViewData["UserID"] = userManager.GetUserId(this.User);
            return View("Profile/profile");
        }

        public IActionResult OrderHistories()
        {
            ViewData["UserID"] = userManager.GetUserId(this.User);
            return View("OrderHistories/orderHistories");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}