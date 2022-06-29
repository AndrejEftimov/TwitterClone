using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Twitter.Areas.Identity.Data;
using Twitter.Data;
using Twitter.Models;
using Twitter.ViewModels;

namespace Twitter.Controllers
{
    public class ListController : BaseController
    {
        public ListController(TwitterContext context, UserManager<TwitterUser> UserManager) : base(context, UserManager) { }

        // GET: List
        public async Task<IActionResult> Index()
        {
            ICollection<List> lists = _context.Lists.Where(l => l.CreatorId == _LoggedInUser.Id).ToList();

            ListIndexViewModel viewModel = new ListIndexViewModel
            {
                LoggedInUser = _LoggedInUser,
                Lists = lists
            };

            return View(viewModel);
        }

        // GET: List/Details/5
        public async Task<IActionResult> Details(int? listId)
        {
            if(listId == null)
                return NotFound();

            List list = _context.Lists.FirstOrDefault(l => l.Id == listId);

            if (list == null)
                return NotFound();

            _context.Entry(list).Collection(l => l.ListMembers).Load();

            var memberIds = _context.ListMember.Where(l => l.ListId == listId).Select(l => l.MemberId);

            ICollection<Post> posts = _context.Posts.Where(p => memberIds.Contains(p.UserId)).Include(p => p.User).ToList();

            ListDetailsViewModel viewModel = new ListDetailsViewModel
            {
                LoggedInUser = _LoggedInUser,
                List = list,
                Posts = posts
            };

            return View(viewModel);
        }

        // GET: List/Create
        public IActionResult Create()
        {
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "DisplayName");
            return View();
        }

        // POST: List/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatorId,Name")] List list)
        {
            if (ModelState.IsValid)
            {
                _context.Add(list);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "DisplayName", list.CreatorId);
            return View(list);
        }

        // GET: List/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Lists == null)
            {
                return NotFound();
            }

            var list = await _context.Lists.FindAsync(id);
            if (list == null)
            {
                return NotFound();
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "DisplayName", list.CreatorId);
            return View(list);
        }

        // POST: List/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatorId,Name")] List list)
        {
            if (id != list.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(list);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListExists(list.Id))
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
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "DisplayName", list.CreatorId);
            return View(list);
        }

        // GET: List/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Lists == null)
            {
                return NotFound();
            }

            var list = await _context.Lists
                .Include(l => l.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (list == null)
            {
                return NotFound();
            }

            return View(list);
        }

        // POST: List/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Lists == null)
            {
                return Problem("Entity set 'TwitterContext.Lists'  is null.");
            }
            var list = await _context.Lists.FindAsync(id);
            if (list != null)
            {
                _context.Lists.Remove(list);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListExists(int id)
        {
          return (_context.Lists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
