using Twitter.Models;

namespace Twitter.ViewModels
{
    public class ListIndexViewModel : BaseViewModel
    {
        public ICollection<List>? Lists { get; set; }
    }
}
