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
        private readonly IWebHostEnvironment webHostEnvironment;

        public ListController(TwitterContext context, UserManager<TwitterUser> UserManager, IWebHostEnvironment hostEnvironment) : base(context, UserManager)
        {
            webHostEnvironment = hostEnvironment;
        }

        // GET: List
        public async Task<IActionResult> Index()
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

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
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            if (listId == null)
                return NotFound();

            List list = _context.Lists.FirstOrDefault(l => l.Id == listId);

            if (list == null)
                return NotFound();

            _context.Entry(list).Collection(l => l.ListMembers).Load();

            var memberIds = _context.ListMember.Where(l => l.ListId == listId).Select(l => l.MemberId);

            ICollection<Post> posts = _context.Posts.Where(p => memberIds.Contains(p.UserId)).Include(p => p.User).OrderByDescending(p => p.DateCreated).ToList();

            ListDetailsViewModel viewModel = new ListDetailsViewModel
            {
                LoggedInUser = _LoggedInUser,
                List = list,
                Posts = posts
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? listName, string? listDesc)
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            if (listName != null && listName.Length > 0)
            {
                // check if list with listName exists
                if (_context.Lists.FirstOrDefault(l => l.Name.Equals(listName)) == null)
                {
                    List list = new List
                    {
                        CreatorId = _LoggedInUser.Id,
                        Name = listName,
                        Description = listDesc
                    };

                    _context.Lists.Add(list);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", "List", new { listId = list.Id });
                }
            }

            return RedirectToAction("Index");
        }

        // GET: List/Create
        public IActionResult AdminCreate()
        {
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "DisplayName");
            return View();
        }

        // POST: List/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCreate([Bind("Id,CreatorId,Name")] List list)
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

        public async Task<IActionResult> Edit(int? listId)
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            if (listId == null)
                return NotFound();

            List list = _context.Lists.FirstOrDefault(l => l.Id == listId);
            if (list == null)
                return NotFound();

            if (list.CreatorId != _LoggedInUser.Id)
                return LocalRedirect("/Identity/Account/AccessDenied");

            _context.Entry(list).Collection(l => l.ListMembers).Load();
            IEnumerable<int>? selectedUserIds = list.ListMembers.Select(lm => lm.MemberId);

            ListEditViewModel viewModel = new ListEditViewModel
            {
                LoggedInUser = _LoggedInUser,
                List = list,
                UserList = new MultiSelectList(_context.Users, "Id", "UserName", selectedUserIds)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ListEditViewModel viewModel)
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            if (viewModel.List.CreatorId != _LoggedInUser.Id)
                return LocalRedirect("/Identity/Account/AccessDenied");

            if (ModelState.IsValid)
            {
                if (viewModel.formFile != null)
                    viewModel.List.CoverImage = UploadedFile(viewModel.formFile);

                // remove list members
                IEnumerable<ListMember> listMembers = _context.ListMember.Where(lm => lm.ListId == viewModel.List.Id).ToList();
                _context.RemoveRange(listMembers);

                // then add list members
                if (viewModel.UserIds != null)
                {
                    ListMember listMember;
                    viewModel.List.MemberCount = 0;
                    foreach (int memberId in viewModel.UserIds)
                    {
                        listMember = new ListMember
                        {
                            MemberId = memberId,
                            ListId = viewModel.List.Id
                        };

                        _context.Add(listMember);
                        viewModel.List.MemberCount++;
                    }
                }

                _context.Update(viewModel.List);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "List", new { listId = viewModel.List.Id });
            }

            viewModel.LoggedInUser = _LoggedInUser;
            _context.Entry(viewModel.List).Collection(l => l.ListMembers).Load();
            IEnumerable<int>? selectedUserIds = viewModel.List.ListMembers.Select(lm => lm.MemberId);
            viewModel.UserList = new MultiSelectList(_context.Users, "Id", "UserName", selectedUserIds);

            return View(viewModel);
        }

        // GET: List/Edit/5
        public async Task<IActionResult> AdminEdit(int? id)
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
        public async Task<IActionResult> AdminEdit(int id, [Bind("Id,CreatorId,Name")] List list)
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

        private string UploadedFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
