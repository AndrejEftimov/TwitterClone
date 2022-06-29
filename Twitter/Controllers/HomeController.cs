using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Twitter.Areas.Identity.Data;
using Twitter.Data;
using Twitter.Models;
using Twitter.ViewModels;

namespace Twitter.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(TwitterContext context, UserManager<TwitterUser> UserManager) : base(context, UserManager) { }

        // CONTINUE
        // https://www.slideshare.net/MarkusWinand/p2d2-pagination-done-the-postgresql-way
        // https://docs.microsoft.com/en-us/ef/core/querying/pagination#keyset-pagination
        // https://docs.microsoft.com/en-us/ef/core/modeling/indexes?tabs=data-annotations#included-columns
        // https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-6.0#add-paging-to-students-index
        public async Task<IActionResult> Index()
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            var usersFollowingIds = _context.Followings.Where(f => f.FollowerId == _LoggedInUser.Id).Select(f => f.FollowedUserId);

            var posts = _context.Posts.Where(p => usersFollowingIds.Contains(p.UserId)).Include(p => p.User);

            HomeIndexViewModel viewModel = new HomeIndexViewModel
            {
                LoggedInUser = _LoggedInUser,
                Posts = posts.ToList()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}