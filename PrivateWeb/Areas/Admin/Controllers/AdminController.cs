using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PrivateWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class AdminController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChatAdmin()
        {
            return View();
        }
    }
}
