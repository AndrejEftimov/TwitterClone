using System.ComponentModel.DataAnnotations;

namespace Twitter.ViewModels
{
    public class NewPostViewModel
    {
        [StringLength(255)]
        public string? Text { get; set; }

        public string? Image { get; set; }

        public string? Video { get; set; }
    }
}
