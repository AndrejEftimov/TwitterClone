using Twitter.Models;

namespace Twitter.ViewModels
{
    public class UserEditViewModel : BaseViewModel
    {
        public User? User { get; set; }

        public IFormFile? ProfileImageFile { get; set; }

        public IFormFile? CoverImageFile { get; set; }
    }
}
