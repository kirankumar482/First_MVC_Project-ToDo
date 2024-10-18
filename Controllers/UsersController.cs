using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Core;
using First_MVC_Project.Models;
using First_MVC_Project.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace First_MVC_Project.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard(int? year)
        {
            int CountOfUsersCreated;
            int CountOfTasksCreated;
            if (!year.HasValue)
            {
                var tasks = await _context.Tasks.ToListAsync();
                var users = await _context.Users.ToListAsync();
                CountOfTasksCreated = tasks.Count;
                CountOfUsersCreated = users.Count;
                ViewBag.Year = "All Years";
            }
            else
            {
                var users = await _context.Users.Where(user => user.CreatedDate.Year.Equals(year)).ToListAsync();
                var tasks = await _context.Tasks.Where(task => task.CreatedDate.Year.Equals(year)).ToListAsync();
                CountOfTasksCreated = tasks.Count;
                CountOfUsersCreated = users.Count;
                ViewBag.Year = year;
            }

            return View(new UserTaskViewModel
            {
                UsersCount = CountOfUsersCreated,
                TasksCount = CountOfTasksCreated
            });
        }

        //[Route("users/{email}")]
        public async Task<IActionResult> Details(string? email)
        {
            if (email == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string? email)
        {
            if (email == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Bind("Name,Email,Password")] 
        public async Task<IActionResult> Edit(string email ,User user)
        {
            if (email != user.Email)
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
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!UserExists(user.Email))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Redirect to a global error page
                        return View("Error", new ErrorViewModel
                        {
                            Error = ex.Message,
                            Message = ex.StackTrace
                        });
                    }
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string? email)
        {
            if (email == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == email );
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string email)
        {
            var user = await _context.Users.FindAsync(email);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Play()
        {
            return View();
        }

        private bool UserExists(string email)
        {
            return _context.Users.Any(e => e.Email == email );
        }
    }
}
