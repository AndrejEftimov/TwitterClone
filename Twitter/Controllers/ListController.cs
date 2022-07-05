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
    public class ListController : BaseController
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public ListController(TwitterContext context, UserManager<TwitterUser> UserManager, IWebHostEnvironment hostEnvironment) : base(context, UserManager)
        {
            webHostEnvironment = hostEnvironment;
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            ICollection<List> listsCreated = _context.Lists.Where(l => l.CreatorId == _LoggedInUser.Id).ToList();
            _context.Entry(_LoggedInUser).Collection(u => u.ListsFollowing).Query().Include(lf => lf.List).Load();
            ICollection<List> listsFollowing = _LoggedInUser.ListsFollowing.Select(lf => lf.List).ToList();

            ListIndexViewModel viewModel = new ListIndexViewModel
            {
                LoggedInUser = _LoggedInUser,
                ListsCreated = listsCreated,
                ListsFollowing = listsFollowing
            };

            return View(viewModel);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Details(int? listId)
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            if (listId == null)
                return NotFound();

            List list = _context.Lists.FirstOrDefault(l => l.Id == listId);

            if (list == null)
                return NotFound();

            _context.Entry(list).Collection(l => l.ListFollowers).Load();
            _context.Entry(list).Collection(l => l.ListMembers).Load();

            var memberIds = _context.ListMember.Where(l => l.ListId == listId).Select(l => l.MemberId);

            ICollection<Post> posts = _context.Posts.Where(p => memberIds.Contains(p.UserId)).Include(p => p.User).Include(p => p.Hearts)
                .OrderByDescending(p => p.DateCreated).ToList();

            ListDetailsViewModel viewModel = new ListDetailsViewModel
            {
                LoggedInUser = _LoggedInUser,
                List = list,
                Posts = posts
            };

            return View(viewModel);
        }

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
        [HttpPost]
        // doesn't work with this DataAnnotation
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(int? listId)
        {
            if (listId == null)
                return Ok();

            List list = _context.Lists.FirstOrDefault(l => l.Id == listId);
            if (list == null)
                return Ok();

            ListFollower listFollower = _context.ListFollower.FirstOrDefault(lf => lf.ListId == list.Id && lf.FollowerId == _LoggedInUser.Id);
            if (listFollower == null)
            {
                list.FollowerCount++;

                listFollower = new ListFollower
                {
                    ListId = list.Id,
                    FollowerId = _LoggedInUser.Id,
                };

                _context.ListFollower.Add(listFollower);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        // doesn't work with this DataAnnotation
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow(int? listId)
        {
            if (listId == null)
                return Ok();

            List list = _context.Lists.FirstOrDefault(l => l.Id == listId);
            if (list == null)
                return Ok();

            ListFollower listFollower = _context.ListFollower.FirstOrDefault(lf => lf.ListId == list.Id && lf.FollowerId == _LoggedInUser.Id);
            if (listFollower != null)
            {
                list.FollowerCount--;

                _context.ListFollower.Remove(listFollower);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            return View(_context.Lists.Include(l => l.Creator).OrderByDescending(l => l.DateCreated).ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AdminIndex(int? listId, int? creatorId, string? creatorName, string? listName, string? listDesc)
        {
            var lists = _context.Lists.AsQueryable();

            if(listId != null)
                lists = lists.Where(l => l.Id == listId);

            if(creatorId != null)
                lists = lists.Where(l => l.CreatorId == creatorId);

            if(creatorName != null)
            {
                creatorName = creatorName.ToUpper();
                lists = lists.Where(l => l.Creator.UserName.ToUpper().Contains(creatorName) || l.Creator.DisplayName.ToUpper().Contains(creatorName));
            }

            if(listName != null)
            {
                listName = listName.ToUpper();
                lists = lists.Where(l => l.Name.ToUpper().Contains(listName));
            }

            if(listDesc != null)
            {
                listDesc = listDesc.ToUpper();
                lists = lists.Where(l => l.Description.ToUpper().Contains(listDesc));
            }

            lists = lists.Include(l => l.Creator).OrderByDescending(l => l.DateCreated);

            return View(lists.ToList());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List list = await _context.Lists.Include(l => l.Creator).FirstOrDefaultAsync(l => l.Id == id);
            if (list == null)
            {
                return NotFound();
            }

            return View(list);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("AdminDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminDeleteConfirmed(int id)
        {
            List list = await _context.Lists.FindAsync(id);
            if (list != null)
            {
                _context.Lists.Remove(list);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("AdminIndex");
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
