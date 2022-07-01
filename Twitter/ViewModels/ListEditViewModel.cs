using Microsoft.AspNetCore.Mvc.Rendering;
using Twitter.Models;

namespace Twitter.ViewModels
{
    public class ListEditViewModel : BaseViewModel
    {
        public List? List { get; set; }

        public IFormFile? formFile { get; set; }

        // Id's of selected users
        public IEnumerable<int>? UserIds { get; set; }

        // SelectList of all users
        public MultiSelectList? UserList { get; set; }
    }
}
