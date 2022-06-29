using Twitter.Models;

namespace Twitter.ViewModels
{
    public class ListDetailsViewModel : BaseViewModel
    {
        public List? List { get; set; }

        public ICollection<Post>? Posts { get; set; }
    }
}
