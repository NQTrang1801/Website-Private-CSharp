using Microsoft.AspNetCore.Mvc;

namespace PrivateWeb.Areas.Admin.Controllers
{
    public class UtilityController : Controller
    {
        [HttpGet]
        [Route("/getSlug")]
        public IActionResult GetSlug(string title)
        {
            var slug = string.Empty;

            if (!string.IsNullOrEmpty(title))
            {
                // Thực hiện logic tạo slug ở đây, ví dụ:
                slug = title.ToLower().Replace(" ", "-");
            }

            return Json(new { status = true, slug });
        }
    }
}
