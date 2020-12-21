using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Models;

namespace ShoppingOnline.Controllers
{
    public class AdminController : Controller
    {
        private readonly ShoppingContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public class ViewModel
        {
            public IEnumerable<Manufacturer> manufacturers { get; set; }
            public IEnumerable<Product> products { get; set; }
        }
        public AdminController(ShoppingContext context, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

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
            var CheckManufacturerName = _context.Manufacturers.SingleOrDefault(x => x.ManufacturerName == ManufacturerName);
            if (ManufacturerName == null)
            {
                ModelState.AddModelError("ManufacturerName", "Vui lòng điền hãng sản xuất");
                return View();
            }
            else if(CheckManufacturerName != null)
            {
                ModelState.AddModelError("ManufacturerName", "Hãng sản xuất đã tồn tại");
                return View();

            }
            else
            {
                Manufacturer manufacturer = new Manufacturer();

                manufacturer.ManufacturerName = ManufacturerName;

                _context.Manufacturers.Add(manufacturer);
                await _context.SaveChangesAsync();
            }
            return Redirect("~/Admin/ListManufacturers");
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
            InfoProduct infoProduct = new InfoProduct();

            var ManufacturersContext = _context.Manufacturers.OrderBy(m => m.ManufacturerId).ToList();
            var DiscountsContext = _context.Discounts.OrderBy(m => m.DiscountId);
            var RamContext = _context.Rams.OrderBy(m => m.RamId);
            var RomContext = _context.Roms.OrderBy(m => m.RomId);




            infoProduct.ManufacturerViewModel = ManufacturersContext.ToList();
            infoProduct.DiscountViewModel = DiscountsContext.ToList();
            infoProduct.RamViewModel = RamContext.ToList();
            infoProduct.RomViewModel = RomContext.ToList();


            return View(infoProduct);
        }

        //Thêm sản phẩm
        private async Task<string> GetUniqueFileName(ProImgAndModelduct product, string fileName)
        {
            var Manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.ManufacturerId == product.ManufacturerId);
            var Ram = await _context.Rams.FirstOrDefaultAsync(r => r.RamId == product.RamId);
            var Rom = await _context.Roms.FirstOrDefaultAsync(r => r.RomId == product.RomId);
            return Manufacturer.ManufacturerName + "_" + product.NameProduct + "_" + Ram.Memory + "_" + Rom.Space + Guid.NewGuid().ToString().Substring(0, 4) + "_" + Path.GetExtension(fileName);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProImgAndModelduct product, Rom rom, Ram ram)
        {
            if (product.NameProduct == null)
            {
                ModelState.AddModelError("NameProduct", "Vui lòng điền trường này");
            }
            string uniqueFileName = await GetUniqueFileName(product, product.ImgFile.FileName);
            //Thêm hình
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "ProductImg");
            var filePath = Path.Combine(uploads, uniqueFileName);
            product.ImgFile.CopyTo(new FileStream(filePath, FileMode.Create));

            Product productNew = new Product();
            productNew.NameProduct = product.NameProduct;
            productNew.ManufacturerId = product.ManufacturerId;
            productNew.RamId = product.RamId;
            productNew.RomId = product.RomId;
            productNew.ImgUrl = uniqueFileName;
            if(product.Discount != null)
            {
                productNew.DiscountId = product.DiscountId;
            }
            productNew.Note = product.Note;
            productNew.Price = product.Price;

            _context.Products.Update(productNew);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListProduct));

        }
        //Hiển thị danh sách sản phẩm
        public async Task<IActionResult> ListProduct()
        {
            var shoppingContext = _context.Products.OrderBy(m => m.ProductId).Include(o => o.Rom).Include(a => a.Ram).Include(m => m.Manufacturer).Include(d => d.Discount);
            return View(await shoppingContext.ToListAsync());
        }

        //Xoá sản phẩm
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListProduct));
        }
    }
}
