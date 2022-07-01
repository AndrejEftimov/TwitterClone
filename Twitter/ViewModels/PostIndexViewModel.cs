using System.ComponentModel.DataAnnotations;
using Twitter.Models;

namespace Twitter.ViewModels
{
    public class PostIndexViewModel : BaseViewModel
    {
        public Post? Post { get; set; }

        public string? ReplyText { get; set; }
    }
}
