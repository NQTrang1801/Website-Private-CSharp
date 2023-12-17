using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public HomeController(ILogger<HomeController> logger, UserManager<PrivateWebUser> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewData["UserID"]=userManager.GetUserId(this.User);

            return View("Index/index");
        }

        public IActionResult Categories()
        {
            ViewData["UserID"] = userManager.GetUserId(this.User);
            return View("Categories/categories");
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