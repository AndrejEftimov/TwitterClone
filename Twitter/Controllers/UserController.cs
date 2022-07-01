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
    public class UserController : BaseController
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public UserController(TwitterContext context, UserManager<TwitterUser> userManager, IWebHostEnvironment hostEnvironment) : base(context, userManager)
        {
            webHostEnvironment = hostEnvironment;
        }

        // GET: User
        public async Task<IActionResult> Index(int? userId)
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            if (userId == null)
                userId = _LoggedInUser.Id;

            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            _context.Entry<User>(user).Collection(u => u.Posts).Load();
            user.Posts = user.Posts.OrderByDescending(p => p.DateCreated).ToList();

            if (_LoggedInUser.Id != user.Id)
            {
                bool isFollowing = _context.Followings.Any(f => f.FollowerId == _LoggedInUser.Id && f.FollowedUserId == userId);

                if (isFollowing == true)
                    ViewData["isFollowing"] = true;
                else
                    ViewData["isFollowing"] = false;
            }

            UserIndexViewModel viewModel = new UserIndexViewModel
            {
                LoggedInUser = _LoggedInUser,
                User = user
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? userId)
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            if (userId != _LoggedInUser.Id)
                return LocalRedirect("/Identity/Account/AccessDenied");

            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            UserEditViewModel viewModel = new UserEditViewModel
            {
                LoggedInUser = _LoggedInUser,
                User = user
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel viewModel)
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            if (viewModel.User.Id != _LoggedInUser.Id)
                return LocalRedirect("/Identity/Account/AccessDenied");
            
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == viewModel.User.Id);

                user.DisplayName = viewModel.User.DisplayName;
                user.Description = viewModel.User.Description;

                if (viewModel.ProfileImageFile != null)
                {
                    user.ProfileImage = UploadedFile(viewModel.ProfileImageFile);
                }

                if (viewModel.CoverImageFile != null)
                {
                    user.CoverImage = UploadedFile(viewModel.CoverImageFile);
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            viewModel.LoggedInUser = _LoggedInUser;

            return View(viewModel);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,DisplayName,Description,FollowerCount,FollowingCount,ProfileImage,CoverImage,DateCreated")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> AdminEdit(int? id)
        {
            if (id == null || _context.Users == null)
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

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEdit(int id, [Bind("Id,UserName,DisplayName,Description,FollowerCount,FollowingCount,ProfileImage,CoverImage,DateCreated")] User user)
        {
            if (id != user.Id)
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
                    if (!UserExists(user.Id))
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

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'TwitterContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
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
