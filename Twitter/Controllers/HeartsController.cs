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
    public class HeartsController : Controller
    {
        private readonly TwitterContext _context;

        public HeartsController(TwitterContext context)
        {
            _context = context;
        }

        // GET: Hearts
        public async Task<IActionResult> Index()
        {
            var twitterContext = _context.Hearts.Include(h => h.Post).Include(h => h.User);
            return View(await twitterContext.ToListAsync());
        }

        // GET: Hearts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Hearts == null)
            {
                return NotFound();
            }

            var heart = await _context.Hearts
                .Include(h => h.Post)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (heart == null)
            {
                return NotFound();
            }

            return View(heart);
        }

        // GET: Hearts/Create
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "DisplayName");
            return View();
        }

        // POST: Hearts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,UserId")] Heart heart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(heart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", heart.PostId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "DisplayName", heart.UserId);
            return View(heart);
        }

        // GET: Hearts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Hearts == null)
            {
                return NotFound();
            }

            var heart = await _context.Hearts.FindAsync(id);
            if (heart == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", heart.PostId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "DisplayName", heart.UserId);
            return View(heart);
        }

        // POST: Hearts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,UserId")] Heart heart)
        {
            if (id != heart.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(heart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HeartExists(heart.PostId))
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
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", heart.PostId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "DisplayName", heart.UserId);
            return View(heart);
        }

        // GET: Hearts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Hearts == null)
            {
                return NotFound();
            }

            var heart = await _context.Hearts
                .Include(h => h.Post)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (heart == null)
            {
                return NotFound();
            }

            return View(heart);
        }

        // POST: Hearts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Hearts == null)
            {
                return Problem("Entity set 'TwitterContext.Hearts'  is null.");
            }
            var heart = await _context.Hearts.FindAsync(id);
            if (heart != null)
            {
                _context.Hearts.Remove(heart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HeartExists(int id)
        {
          return (_context.Hearts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}
