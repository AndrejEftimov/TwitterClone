using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Twitter.Data;
using Twitter.Models;

namespace Twitter.Controllers
{
    public class FollowingsController : Controller
    {
        private readonly TwitterContext _context;

        public FollowingsController(TwitterContext context)
        {
            _context = context;
        }

        // GET: Followings
        public async Task<IActionResult> Index()
        {
            var twitterContext = _context.Followings.Include(f => f.FollowedUser).Include(f => f.Follower);
            return View(await twitterContext.ToListAsync());
        }

        // GET: Followings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Followings == null)
            {
                return NotFound();
            }

            var following = await _context.Followings
                .Include(f => f.FollowedUser)
                .Include(f => f.Follower)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (following == null)
            {
                return NotFound();
            }

            return View(following);
        }

        // GET: Followings/Create
        public IActionResult Create()
        {
            ViewData["FollowedUserId"] = new SelectList(_context.Users, "Id", "DisplayName");
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "DisplayName");
            return View();
        }

        // POST: Followings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FollowedUserId,FollowerId")] Following following)
        {
            if (ModelState.IsValid)
            {
                _context.Add(following);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FollowedUserId"] = new SelectList(_context.Users, "Id", "DisplayName", following.FollowedUserId);
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "DisplayName", following.FollowerId);
            return View(following);
        }

        // GET: Followings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Followings == null)
            {
                return NotFound();
            }

            var following = await _context.Followings.FindAsync(id);
            if (following == null)
            {
                return NotFound();
            }
            ViewData["FollowedUserId"] = new SelectList(_context.Users, "Id", "DisplayName", following.FollowedUserId);
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "DisplayName", following.FollowerId);
            return View(following);
        }

        // POST: Followings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FollowedUserId,FollowerId")] Following following)
        {
            if (id != following.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(following);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FollowingExists(following.Id))
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
            ViewData["FollowedUserId"] = new SelectList(_context.Users, "Id", "DisplayName", following.FollowedUserId);
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "DisplayName", following.FollowerId);
            return View(following);
        }

        // GET: Followings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Followings == null)
            {
                return NotFound();
            }

            var following = await _context.Followings
                .Include(f => f.FollowedUser)
                .Include(f => f.Follower)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (following == null)
            {
                return NotFound();
            }

            return View(following);
        }

        // POST: Followings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Followings == null)
            {
                return Problem("Entity set 'TwitterContext.Followings'  is null.");
            }
            var following = await _context.Followings.FindAsync(id);
            if (following != null)
            {
                _context.Followings.Remove(following);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FollowingExists(int id)
        {
          return (_context.Followings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
