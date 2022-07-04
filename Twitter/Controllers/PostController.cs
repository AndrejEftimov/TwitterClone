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
    public class PostController : BaseController
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public PostController(TwitterContext context, UserManager<TwitterUser> UserManager, IWebHostEnvironment hostEnvironment) : base(context, UserManager)
        {
            webHostEnvironment = hostEnvironment;
        }

        // GET: Post
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

        // GET: Post
        public async Task<IActionResult> AdminIndex()
        {
            var twitterContext = _context.Posts.Include(p => p.User);
            return View(await twitterContext.ToListAsync());
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
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

        // GET: Post/Create
        public IActionResult Create()
        {


            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "DisplayName", post.UserId);
            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,DateCreated,Text,Image,Video,HeartCount,ReplyCount")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "DisplayName", post.UserId);
            return View(post);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
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

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'TwitterContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
