using First_MVC_Project.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace First_MVC_Project.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly AppDbContext _context;
      
        public AuthenticationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    //create identity at registration page
                    var identity = new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");            
                }
                catch (DbUpdateException ex)
                {
                    // Redirect to a global error page
                    return View("Error", new ErrorViewModel
                    {
                        Error = ex.Message,
                        Message = ex.StackTrace
                    });
                }
            }
            return View(user);
        }

        public IActionResult Validate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate([Bind("Email,Password")] User user)
        {
            if (User == null)
            {
                return NotFound();
            }

            var targetUser = await _context.Users.FindAsync(user.Email);
            if (targetUser != null)
            {
                if (user.Password == targetUser.Password)
                {
                    //GET : Tasks
                    var UserTasks = _context.Tasks.Where(t => t.UserEmail == targetUser.Email);

                    //create identity at login page
                    var identity = new ClaimsIdentity(new List<Claim>
                    {
                         new Claim(ClaimTypes.Email, targetUser.Email),
                         new Claim(ClaimTypes.Name, targetUser.Name),
                         new Claim(ClaimTypes.Role, "User")
                    }, CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["Message"] = "Password Not matched";
                    return View(user);
                }
            }
            else if (user.Email == "kiran284@gmail.com" && user.Password == "kiran284")
            {
                var identity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Message"] = "User Not matched";
                return View(user);
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);



            return RedirectToAction("Index", "Home");
        }
    }
}