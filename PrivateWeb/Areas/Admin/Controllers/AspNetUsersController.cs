using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrivateWeb.Models;

namespace PrivateWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    [Area("Admin")]
    public class AspNetUsersController : Controller
    {
        private readonly PrivateWebContext _context;

        public AspNetUsersController(PrivateWebContext context)
        {
            _context = context;
        }

        // GET: Admin/AspNetUsers
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> IndexMember(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var memberUsers = await _context.AspNetUsers
                .Where(user => user.Roles.Any(role => role.Name == "Member"))
                .Where(user =>
                    string.IsNullOrEmpty(searchString) ||
                    user.Id.Contains(searchString) ||
                    user.FirstName.Contains(searchString) ||
                    user.LastName.Contains(searchString) ||
                    user.Email.Contains(searchString)
                )
                .OrderBy(user => user.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int totalCount = await _context.AspNetUsers
                .Where(user => user.Roles.Any(role => role.Name == "Member"))
                .CountAsync();
            int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewData["PageCount"] = pageCount;
            ViewData["PageNumber"] = pageNumber;

            return View("Index", memberUsers);
        }



        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> IndexAdmin(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var adminUsers = await _context.AspNetUsers
                .Where(user => user.Roles.Any(role => role.Name == "Admin"))
                .Where(user =>
                    string.IsNullOrEmpty(searchString) ||
                    user.Id.Contains(searchString) ||
                    user.FirstName.Contains(searchString) ||
                    user.LastName.Contains(searchString) ||
                    user.Email.Contains(searchString)
                )
                .OrderBy(user => user.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int totalCount = await _context.AspNetUsers
                .Where(user => user.Roles.Any(role => role.Name == "Admin"))
                .CountAsync();
            int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewData["PageCount"] = pageCount;
            ViewData["PageNumber"] = pageNumber;

            return View("Index", adminUsers);
        }

        // GET: Admin/AspNetUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            return View(aspNetUser);
        }

        // GET: Admin/AspNetUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AspNetUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aspNetUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aspNetUser);
        }

        // GET: Admin/AspNetUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return NotFound();
            }
            return View(aspNetUser);
        }

        // POST: Admin/AspNetUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser)
        {
            if (id != aspNetUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aspNetUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspNetUserExists(aspNetUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexMember));
            }
            return View(aspNetUser);
        }

        // GET: Admin/AspNetUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            return View(aspNetUser);
        }

        // POST: Admin/AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.AspNetUsers == null)
            {
                return Problem("Entity set 'PrivateWebContext.AspNetUsers'  is null.");
            }
            var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            if (aspNetUser != null)
            {
                _context.AspNetUsers.Remove(aspNetUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AspNetUserExists(string id)
        {
          return (_context.AspNetUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
