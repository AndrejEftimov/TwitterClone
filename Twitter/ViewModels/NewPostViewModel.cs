using System.ComponentModel.DataAnnotations;

namespace Twitter.ViewModels
{
    public class NewPostViewModel
    {
        [StringLength(255)]
        public string? Text { get; set; }

        public IFormFile? formFile { get; set; }
    }
}
