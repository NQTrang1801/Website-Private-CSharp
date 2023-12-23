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
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly UserManager<PrivateWebUser> userManager;
        private readonly PrivateWebContext _context;

        public OrdersController(PrivateWebContext context, ILogger<OrdersController> logger, UserManager<PrivateWebUser> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
            _context = context;
        }

        // GET: Orders
        // GET: Orders
        public async Task<IActionResult> Index(decimal? total)
        {
            ViewData["UserID"] = userManager.GetUserId(this.User);
            var categories = _context.Categories.Where(c => c.ShowHome == "Yes").ToList();
            ViewData["Categories"] = categories;
            return View();
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Orders orders)
        {
            try
            {
                    // Thêm order vào context
                    _context.Add(orders);
                    await _context.SaveChangesAsync();

                    // Trả về đối tượng JSON với thông tin đơn hàng đã tạo
                    return Json(new { success = true, orderId = orders.Id, message = "Đơn hàng đã được tạo thành công!" });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return Json(new { success = false, message = "Đã xảy ra lỗi khi tạo đơn hàng: " + ex.Message });
            }
        }
    }
}
