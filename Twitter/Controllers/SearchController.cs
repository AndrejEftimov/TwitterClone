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
    [Authorize(Roles = "User")]
    public class SearchController : BaseController
    {
        public SearchController(TwitterContext context, UserManager<TwitterUser> userManager) : base(context, userManager) { }

        // GET: Search
        public async Task<IActionResult> Index()
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            SearchIndexViewModel viewModel = new SearchIndexViewModel
            {
                LoggedInUser = _LoggedInUser
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SearchIndexViewModel? viewModel, string? searchString)
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            viewModel.LoggedInUser = _LoggedInUser;

            if (viewModel.SearchOption == null || viewModel.SearchOption == "Users")
            {
                if (searchString != null)
                {
                    viewModel.Users = _context.Users.Where(
                        u => u.UserName.Contains(searchString)
                        || u.DisplayName.Contains(searchString) 
                        || u.Description.Contains(searchString))
                        .Include(u => u.Followers)
                        .OrderByDescending(u => u.DateCreated).ToList();
                }

                else if (viewModel.SearchString != null)
                {
                    viewModel.Users = _context.Users.Where(
                        u => u.UserName.Contains(viewModel.SearchString)
                        || u.DisplayName.Contains(viewModel.SearchString) 
                        || u.Description.Contains(viewModel.SearchString))
                        .Include(u => u.Followers)
                        .OrderByDescending(u => u.DateCreated).ToList();
                }
            }

            else if (viewModel.SearchOption == "Posts")
            {
                if (searchString != null)
                {
                    viewModel.Posts = _context.Posts.Where(
                        p => p.Text.Contains(searchString)
                        || p.User.UserName.Contains(searchString)
                        || p.User.DisplayName.Contains(searchString))
                        .Include(p => p.User).Include(p => p.Hearts)
                        .OrderByDescending(p => p.DateCreated).ToList();
                }

                else
                {
                    viewModel.Posts = _context.Posts.Where(
                        p => p.Text.Contains(viewModel.SearchString)
                        || p.User.UserName.Contains(viewModel.SearchString)
                        || p.User.DisplayName.Contains(viewModel.SearchString))
                        .Include(p => p.User).Include(p => p.Hearts)
                        .OrderByDescending(p => p.DateCreated).ToList();
                }
            }

            else if (viewModel.SearchOption == "Lists")
            {
                if (searchString != null)
                {
                    viewModel.Lists = _context.Lists.Where(
                        l => l.Name.Contains(searchString)
                        || l.Description.Contains(searchString)
                        || l.Creator.UserName.Contains(searchString)
                        || l.Creator.DisplayName.Contains(searchString))
                        .OrderByDescending(l => l.DateCreated).ToList();
                }

                else
                {
                    viewModel.Lists = _context.Lists.Where(
                        l => l.Name.Contains(viewModel.SearchString)
                        || l.Description.Contains(viewModel.SearchString)
                        || l.Creator.UserName.Contains(viewModel.SearchString)
                        || l.Creator.DisplayName.Contains(viewModel.SearchString))
                        .OrderByDescending(l => l.DateCreated).ToList();
                }
            }

            return View(viewModel);
        }
    }
}
