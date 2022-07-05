using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public UserController(TwitterContext context, UserManager<TwitterUser> userManager, IWebHostEnvironment hostEnvironment) : base(context, userManager)
        {
            webHostEnvironment = hostEnvironment;
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index(int? userId)
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            if (userId == null)
                userId = _LoggedInUser.Id;

            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            _context.Entry<User>(user).Collection(u => u.Posts).Query().Include(p => p.Hearts).Load();
            user.Posts = user.Posts.OrderByDescending(p => p.DateCreated).ToList();
            _context.Entry<User>(user).Collection(u => u.Followers).Load();

            UserIndexViewModel viewModel = new UserIndexViewModel
            {
                LoggedInUser = _LoggedInUser,
                User = user
            };

            return View(viewModel);
        }

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
        [HttpPost]
        // doesn't work with this DataAnnotation
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(int? userId)
        {
            if (userId == null)
                return Ok();

            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return Ok();

            Following following = _context.Followings.FirstOrDefault(f => f.FollowedUserId == user.Id && f.FollowerId == _LoggedInUser.Id);
            if (following == null)
            {
                user.FollowerCount++;
                _LoggedInUser.FollowingCount++;

                following = new Following
                {
                    FollowedUserId = user.Id,
                    FollowerId = _LoggedInUser.Id,
                };

                _context.Followings.Add(following);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        // doesn't work with this DataAnnotation
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow(int? userId)
        {
            if (userId == null)
                return Ok();

            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return Ok();

            Following following = _context.Followings.FirstOrDefault(f => f.FollowedUserId == user.Id && f.FollowerId == _LoggedInUser.Id);
            if (following != null)
            {
                user.FollowerCount--;
                _LoggedInUser.FollowingCount--;

                _context.Followings.Remove(following);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        //// GET: User/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Users == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            return View(_context.Users.OrderByDescending(u => u.DateCreated).ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AdminIndex(int? userId, string? userName, string? userDesc)
        {
            IEnumerable<User> users = _context.Users;
            if (userId != null)
                users = users.Where(u => u.Id == userId);

            if (userName != null)
            {
                userName = userName.ToUpper();
                users = users.Where(u => u.UserName.ToUpper().Contains(userName) || u.DisplayName.ToUpper().Contains(userName));
            }

            if(userDesc != null)
            {
                userDesc = userDesc.ToUpper();
                users = users.Where(u => u.Description.ToUpper().Contains(userDesc));
            }

            users = users.OrderByDescending(u => u.DateCreated);

            return View(users.ToList());
        }

        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AdminCreate([Bind("Id,UserName,DisplayName,Description,FollowerCount,FollowingCount,ProfileImage,CoverImage,DateCreated")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(user);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(user);
        //}

        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> AdminEdit(int? id)
        //{
        //    if (id == null || _context.Users == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(user);
        //}

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AdminEdit(int id, [Bind("Id,UserName,DisplayName,Description,FollowerCount,FollowingCount,ProfileImage,CoverImage,DateCreated")] User user)
        //{
        //    if (id != user.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(user);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UserExists(user.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(user);
        //}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDelete(int? userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("AdminDelete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDeleteConfirmed(int id)
        {
            User user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("AdminIndex");
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
