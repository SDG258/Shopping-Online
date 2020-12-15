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
    public class ManufacturersController : Controller
    {
        private readonly ShoppingContext _context;

        public ManufacturersController(ShoppingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ListManufacturers()
        {
            var shoppingContext = _context.Manufacturers.OrderBy(m => m.ManufacturerId);
            return View(await shoppingContext.ToListAsync());
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(string ManufacturerName)
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
