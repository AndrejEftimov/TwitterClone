using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Twitter.Areas.Identity.Data;
using Twitter.Data;
using Twitter.Models;

namespace Twitter.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly TwitterContext _context;
        protected User? _LoggedInUser;
        protected UserManager<TwitterUser> userManager;

        public BaseController(TwitterContext context, UserManager<TwitterUser> UserManager)
        {
            _context = context;
            userManager = UserManager;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await SetLoggedInUser();
            await base.OnActionExecutionAsync(context, next);
        }

        public async Task SetLoggedInUser()
        {
            TwitterUser currUser = await userManager.GetUserAsync(User);

            if(currUser != null)
            {
                _context.Entry(currUser).Reference(u => u.User).Load();
                _LoggedInUser = currUser.User;
            }
        }
    }
}
