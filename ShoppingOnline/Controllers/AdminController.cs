using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Models;

namespace ShoppingOnline.Controllers
{
    public class AdminController : Controller
    {
        private readonly ShoppingContext _context;

        public AdminController(ShoppingContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Admin/Customers
        public async Task<IActionResult> Customers()
        {
            var usersContext = _context.Users.OrderBy(x => x.UserId);

            return View(await usersContext.ToListAsync());
        }

        public async Task<IActionResult> ListManufacturers()
        {
            var shoppingContext = _context.Manufacturers.OrderBy(m => m.ManufacturerId);
            return View(await shoppingContext.ToListAsync());
        }
        public IActionResult AddManufacturers()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddManufacturers(string ManufacturerName)
        {
            if (ManufacturerName == null)
            {
                ModelState.AddModelError("ManufacturerName", "Vui lòng điền họ");
            }
            else
            {
                Manufacturer manufacturer = new Manufacturer();

                manufacturer.ManufacturerName = ManufacturerName;

                _context.Manufacturers.Add(manufacturer);
                await _context.SaveChangesAsync();
            }
            return Redirect("~/Manufacturers/ListManufacturers");
        }
    }
}
