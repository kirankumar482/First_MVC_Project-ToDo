using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using First_MVC_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace First_MVC_Project.Controllers
{
    [Authorize(Roles = "User")]
    public class GoalsController : Controller
    {
        private AppDbContext _context;

        public GoalsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Goals
        public async Task<IActionResult> Index()
        {
            string? targetEmail = User.FindFirstValue(ClaimTypes.Email);
            if(targetEmail == null)
            {
                return NotFound();
            }
            var appDbContext = _context.Tasks.Where(t => t.UserEmail == targetEmail).OrderBy(t => t.DueDate);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Goals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goal = await _context.Tasks
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (goal == null)
            {
                return NotFound();
            }

            return View(goal);
        }

        // GET: Goals/Create
        public IActionResult Create()
        {
            ViewData["userEmail"] = User.FindFirst(ClaimTypes.Email)?.Value;
            return View();
        }

        // POST: Goals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Bind("Id,Name,Description,CreatedDate,DueDate,UserEmail")]
        public async Task<IActionResult> Create(Goal goal)
        {
            Debug.WriteLine(goal.DueDate);
            if (ModelState.IsValid)
            {
                _context.Tasks.Add(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(goal);
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            if(_context.Tasks.Count() > 0)
            {
                var tasks = _context.Tasks.ToList();
                _context.Tasks.RemoveRange(tasks);
                await _context.SaveChangesAsync(); 
                TempData["Message"] = "Tasks cleared successfully!";
            }
            else
            {
                TempData["Message"] = "No tasks to clear.";
            }  
            return View(nameof(Index),_context.Tasks.ToList());
        }

        // GET: Goals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goal = await _context.Tasks.FindAsync(id);
            if (goal == null)
            {
                return NotFound();
            }
            ViewData["UserEmail"] = new SelectList(_context.Users, "Email", "Email", goal.UserEmail);
            return View(goal);
        }

        // POST: Goals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        //[Bind("Id,Name,Description,CreatedDate,DueDate,UserEmail")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Goal goal)
        {
            if (id != goal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(goal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoalExists(goal.Id))
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
            ViewData["UserEmail"] = new SelectList(_context.Users, "Email", "Email", goal.UserEmail);
            return View(goal);
        }

        // GET: Goals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goal = await _context.Tasks
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (goal == null)
            {
                return NotFound();
            }

            return View(goal);
        }

        // POST: Goals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var goal = await _context.Tasks.FindAsync(id);
            if (goal != null)
            {
                _context.Tasks.Remove(goal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoalExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
