using Twitter.Models;

namespace Twitter.ViewModels
{
    public class BaseViewModel
    {
        public User? LoggedInUser { get; set; }

        public NewPostViewModel? NewPost { get; set; }
    }
}
