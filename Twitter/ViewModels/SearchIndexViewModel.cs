using Twitter.Models;

namespace Twitter.ViewModels
{
    public class SearchIndexViewModel : BaseViewModel
    {
        public ICollection<User>? Users { get; set; }

        public ICollection<Post>? Posts { get; set; }

        public ICollection<List>? Lists { get; set; }

        public string? SearchString { get; set; }

        public string? SearchOption { get; set; }

        public string[] SearchOptions = new[] { "Users", "Posts", "Lists" };
    }
}