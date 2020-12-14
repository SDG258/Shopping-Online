using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using ShoppingOnline.Models;

namespace ShoppingOnline.Controllers
{
    public class UsersController : Controller
    {
        private readonly ShoppingContext _context;

        public UsersController(ShoppingContext context)
        {
            _context = context;
        }
        // GET: Users/Login

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            if (Email == null)
            {
                ModelState.AddModelError("Email", "Vui lòng điền thông tin");
            }

            if (Password == null)
            {
                ModelState.AddModelError("Password", "Vui lòng điền thông tin");
                return View();

            }

            var user = _context.Users.SingleOrDefault(x => x.Email == Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy tài khoản trong hệ thống vui lòng đăng ký");
            }
            else if (user != null)
            {
                bool verified = BCrypt.Net.BCrypt.Verify(Password, user.Password);
                if (verified && user.Permission == 1)
                {
                    return Redirect("~/Admin/");
                }
                else if(verified)
                {
                    return Redirect("~/");
                }
                else
                {
                    ModelState.AddModelError("Password", "Vui lòng kiểm tra lại mật khẩu");
                }
            }
            return View();
        }

        // GET: Users/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string FristName, string LastName, string Email, string Phone, string Password, string ConfirmPassword)
        {
            var userEmail = _context.Users.SingleOrDefault(x => x.Email == Email);
            if (FristName == null)
            {
                ModelState.AddModelError("FristName", "Vui lòng điền họ");
            }
            else if (LastName == null)
            {
                ModelState.AddModelError("LastName", "Vui lòng điền tên");
            }

            else if (Email == null)
            {
                ModelState.AddModelError("Email", "Vui lòng điền Mail");

            }
            else if(userEmail != null)
            {
                ModelState.AddModelError("Email", "Tài khoản đã tồn tại vui lòng đăng nhập");
            }
            else if (Phone == null)
            {
                ModelState.AddModelError("Phone", "Vui lòng điền số điện thoại");
            }
            else if (Password == null)
            {
                ModelState.AddModelError("Password", "Vui lòng điền mật khẩu");
            }
            else if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Vui lòng kiểm tra lại mật khẩu và xác nhận mật khẩu");
            }
            else
            {
                User user = new User();
                user.FristName = FristName;
                user.LastName = LastName;
                user.Email = Email;
                user.Phone = Phone;
                user.Permission = 0;

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);

                user.Password = passwordHash;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Redirect("~/");
            }
            return View();
        }

        // Post: Users/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (Email == null)
            {
                ModelState.AddModelError("Email", "Vui lòng điền thông tin");
            }

            var user = _context.Users.SingleOrDefault(x => x.Email == Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy tài khoản trong hệ thống vui lòng đăng ký");
            }
            else if (user != null)
            {
                Random randomNumber = new Random();
                var randomNumberString = randomNumber.Next(0, 9999).ToString("0000");

                //Send Mail
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Test project", "aps.netcore.dev@gmail.com"));
                message.To.Add(new MailboxAddress("ASP.NET Shopping", Email));
                message.Subject = "Test send mail";

                user.Code = randomNumberString;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                string myHostUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                message.Body = new TextPart("html")
                {
                    Text = "<strong>From ASP.NET Shopping</strong>" + "<br>" + "<strong>" + myHostUrl + "/" + user.UserId + "/" + randomNumberString
                };
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("aps.netcore.dev@gmail.com", "Songdu1999#");
                    client.Send(message);
                    client.Disconnect(true);
                }
                return Redirect("~/");
            }
            return View();
        }

        [HttpGet("{id}/{code}")]
        public async Task<IActionResult> GetQueryAsync(int id,string code)
        {
            var user = await _context.Users.FindAsync(id);
            if(user != null)
            {
                if (code == user.Code)
                {
                    user.Code = null;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    //Save Session
                    UserSession userSession = new UserSession()
                    {
                        ID = id,
                        Email = user.Email,
                    };
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(userSession));

                    return Redirect("~/Users/ChangePassword");
                }
            }
            return Redirect("~/");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string Password, string ConfirmPassword)
        {
            var User = HttpContext.Session.GetString("User");
            var userSession = JsonConvert.DeserializeObject<UserSession>(User);
            var user = await _context.Users.FindAsync(userSession.ID);
            if(user!= null)
            {
                if( Password == ConfirmPassword)
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);

                    user.Password = passwordHash;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Email,Password,Phone,FristName,LastName,Code")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Email,Password,Phone,FristName,LastName,Code")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
