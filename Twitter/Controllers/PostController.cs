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
    public class PostController : BaseController
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public PostController(TwitterContext context, UserManager<TwitterUser> UserManager, IWebHostEnvironment hostEnvironment) : base(context, UserManager)
        {
            webHostEnvironment = hostEnvironment;
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index(int? postId)
        {
            if (_LoggedInUser == null)
                return LocalRedirect("/Identity/Account/Login");

            if (postId == null)
                return NotFound();

            Post post = _context.Posts.FirstOrDefault(p => p.Id == postId);

            _context.Entry(post).Reference(p => p.User).Load();
            _context.Entry(post).Collection(p => p.Replies).Query().Include(r => r.User).Include(r => r.Hearts).OrderByDescending(p => p.DateCreated).Load();
            _context.Entry(post).Collection(p => p.Hearts).Load();

            if (post == null)
                return NotFound();

            PostIndexViewModel viewModel = new PostIndexViewModel
            {
                LoggedInUser = _LoggedInUser,
                Post = post
            };

            return View(viewModel);
        }

        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BaseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.NewPost.Text != null || viewModel.NewPost.formFile != null)
                {
                    string uniqueFileName = "";

                    if (viewModel.NewPost.formFile != null)
                        uniqueFileName = UploadedFile(viewModel.NewPost.formFile);

                    Post post = new Post
                    {
                        UserId = _LoggedInUser.Id,
                        Text = viewModel.NewPost.Text,
                        Image = uniqueFileName
                    };

                    _context.Posts.Add(post);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "User");
                }
            }

            return RedirectToAction("Index", "User");
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(PostIndexViewModel viewModel)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(viewModel.ReplyText))
            {
                Reply reply = new Reply
                {
                    PostId = viewModel.Post.Id,
                    UserId = _LoggedInUser.Id,
                    Text = viewModel.ReplyText
                };

                Post post = _context.Posts.FirstOrDefault(p => p.Id == viewModel.Post.Id);
                post.ReplyCount++;

                _context.Replies.Add(reply);
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Post", new { postId = viewModel.Post.Id });
            }

            return RedirectToAction("Index", "Post", new { postId = viewModel.Post.Id });
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        // doesn't work with this DataAnnotation
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> PostHeart(int? postId)
        {
            if (postId == null)
                return Ok();

            Post post = _context.Posts.FirstOrDefault(p => p.Id == postId);
            if (post == null)
                return Ok();

            Heart heart = _context.Hearts.FirstOrDefault(h => h.PostId == postId && h.UserId == _LoggedInUser.Id);
            if (heart == null)
            {
                post.HeartCount++;

                _context.Hearts.Add(
                new Heart { PostId = postId, UserId = _LoggedInUser.Id }
                );
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        // doesn't work with this DataAnnotation
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> PostUnheart(int? postId)
        {
            if (postId == null)
                return Ok();

            Post post = _context.Posts.FirstOrDefault(p => p.Id == postId);
            if (post == null)
                return Ok();

            post.HeartCount--;
            Heart heart = _context.Hearts.FirstOrDefault(h => h.PostId == postId && h.UserId == _LoggedInUser.Id);
            _context.Hearts.Remove(heart);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        // doesn't work with this DataAnnotation
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyHeart(int? replyId)
        {
            if (replyId == null)
                return Ok();

            Reply reply = _context.Replies.FirstOrDefault(r => r.Id == replyId);
            if (reply == null)
                return Ok();

            Heart heart = _context.Hearts.FirstOrDefault(h => h.ReplyId == replyId && h.UserId == _LoggedInUser.Id);
            if (heart == null)
            {
                reply.HeartCount++;

                _context.Hearts.Add(
                new Heart { ReplyId = replyId, UserId = _LoggedInUser.Id }
                );
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        // doesn't work with this DataAnnotation
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyUnheart(int? replyId)
        {
            if (replyId == null)
                return Ok();

            Reply reply = _context.Replies.FirstOrDefault(r => r.Id == replyId);
            if (reply == null)
                return Ok();

            reply.HeartCount--;
            Heart heart = _context.Hearts.FirstOrDefault(h => h.ReplyId == replyId && h.UserId == _LoggedInUser.Id);
            _context.Hearts.Remove(heart);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            return View(_context.Posts.Include(p => p.User).OrderByDescending(p => p.DateCreated).ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AdminIndex(int? userId, int? postId, string? postText, string? userName)
        {
            var posts = _context.Posts.AsQueryable();

            if (postId != null)
                posts = posts.Where(p => p.Id == postId);

            if(userId != null)
                posts = posts.Where(p => p.UserId == userId);

            if(postText != null)
            {
                postText = postText.ToUpper();
                posts = posts.Where(p => p.Text.ToUpper().Contains(postText));
            }

            if(userName != null)
            {
                userName = userName.ToUpper();
                posts = posts.Where(p => p.User.UserName.ToUpper().Contains(userName) || p.User.DisplayName.ToUpper().Contains(userName));
            }

            posts = posts.Include(p => p.User).OrderByDescending(p => p.DateCreated);
                
            return View(posts.ToList());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminReplies(int? postId)
        {
            var replies = _context.Replies.Where(r => r.PostId == postId).Include(r => r.User).OrderByDescending(r => r.DateCreated).ToList();

            return View(replies);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AdminReplies(int? postId, int? replyId, int? userId, string? userName, string? replyText)
        {
            if(postId == null)
                return NotFound();

            var replies = _context.Replies.Where(r => r.PostId == postId);

            if (replyId != null)
                replies = replies.Where(r => r.Id == replyId);

            if(userId != null)
                replies = replies.Where(r => r.UserId == userId);

            if(userName != null)
            {
                userName = userName.ToUpper();
                replies = replies.Where(r => r.User.UserName.ToUpper().Contains(userName) || r.User.DisplayName.ToUpper().Contains(userName));
            }

            if (replyText != null)
            {
                replyText = replyText.ToUpper();
                replies = replies.Where(r => r.Text.ToUpper().Contains(replyText));
            }

            replies = replies.Include(r => r.User).OrderByDescending(r => r.DateCreated);

            return View(replies.ToList());
        }

        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Posts == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts.FindAsync(id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "DisplayName", post.UserId);
        //    return View(post);
        //}

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,DateCreated,Text,Image,Video,HeartCount,ReplyCount")] Post post)
        //{
        //    if (id != post.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(post);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PostExists(post.Id))
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
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "DisplayName", post.UserId);
        //    return View(post);
        //}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("AdminDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminDeleteConfirmed(int id)
        {
            Post post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("AdminIndex");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDeleteReply(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Reply reply = await _context.Replies.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
            if (reply == null)
            {
                return NotFound();
            }

            return View(reply);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("AdminDeleteReply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminDeleteReplyConfirmed(int id)
        {
            Reply reply = await _context.Replies.FindAsync(id);
            int postId = reply.PostId;
            if (reply != null)
            {
                _context.Replies.Remove(reply);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("AdminReplies", new { postId = postId});
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
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
