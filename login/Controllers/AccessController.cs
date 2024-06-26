using login.DTOs;
using login.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace login.Controllers
{
    public class AccessController : Controller
    {
        private ContextDB _contextDB;

        public AccessController(ContextDB contextDB)
        {
            _contextDB = contextDB;
        }
        [HttpGet]
        public IActionResult register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> register(UserDto newUser) {
            if (newUser.password != newUser.confirmPassword)
            {
                ViewData["Message"] = "The passwords is not match";
                return View();
            }

            User user=new User();
            {
                user.name = newUser.name;
                user.email = newUser.email;
                user.password = newUser.password;
            }

            await _contextDB.AddAsync(user);
            await _contextDB.SaveChangesAsync();
            if (user.userID!=0)
            {
                return RedirectToAction("Login","Access");
            }
            ViewData["Message"] = "Failed to create user";
            return View();
        }

        [HttpGet]
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Login(LoginDto login)
        {
            User? foundUser = await _contextDB.User.Where(u => u.email
            == login.email &&
            u.password == login.password
            ).FirstOrDefaultAsync();

            if (foundUser==null)
            {
                ViewData["message"] = "no matches found";
                return View();
            }
            List<Claim> claim = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,foundUser.name)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim,
                CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),properties
                );
            return RedirectToAction("index","home");
        }
    }
}
