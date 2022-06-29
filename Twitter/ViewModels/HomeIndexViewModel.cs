using Twitter.Models;

namespace Twitter.ViewModels
{
    public class HomeIndexViewModel : BaseViewModel
    {
        public ICollection<Post>? Posts { get; set; }
    }
}
