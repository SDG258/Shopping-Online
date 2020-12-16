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

        public class ViewModel
        {
            public IEnumerable<Manufacturer> manufacturers { get; set; }
            public IEnumerable<Product> products { get; set; }
        }
        public AdminController(ShoppingContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        //Lấy danh sách Khách hàng
        public async Task<IActionResult> Customers()
        {
            var usersContext = _context.Users.OrderBy(x => x.UserId);

            return View(await usersContext.ToListAsync());
        }

        //Lấy danh sách hãng sản xuất
        public async Task<IActionResult> ListManufacturers()
        {
            var shoppingContext = _context.Manufacturers.OrderBy(m => m.ManufacturerId);
            return View(await shoppingContext.ToListAsync());
        }

        //Hiển thị form hãng sản xuất
        public IActionResult AddManufacturers()
        {
            return View();
        }

        //Thêm hãng sản xuất
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
        //Hiển thi danh sách mã giảm giá
        public async Task<IActionResult> ListDiscount()
        {
            var shoppingContext = _context.Products.OrderBy(m => m.ManufacturerId);
            return View(await shoppingContext.ToListAsync());
        }

        //Hiển thị form thêm sản phẩm
        public async Task<IActionResult> AddProduct()
        {
            ManufacturerAndDiscountViewModel manufacturerAndDiscountViewModel = new ManufacturerAndDiscountViewModel();

            var ManufacturersContext = _context.Manufacturers.OrderBy(m => m.ManufacturerId);
            var DiscountsContext = _context.Discounts.OrderBy(m => m.DiscountId);


            manufacturerAndDiscountViewModel.ManufacturerViewModel = ManufacturersContext.ToList();
            manufacturerAndDiscountViewModel.DiscountViewModel = DiscountsContext.ToList();

            return View(manufacturerAndDiscountViewModel);
        }

        //Thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> AddProduct(string ManufacturerName)
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
