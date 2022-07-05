using Twitter.Models;

namespace Twitter.ViewModels
{
    public class ListIndexViewModel : BaseViewModel
    {
        public ICollection<List>? ListsCreated { get; set; }

        public ICollection<List>? ListsFollowing { get; set; }
    }
}
