using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class Post
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string Text { get; set; }

        public string? Image { get; set; }

        public string? Video { get; set; }

        public int HeartCount { get; set; } = 0;

        public int ReplyCount { get; set; } = 0;

        public ICollection<Reply> Replies { get; set; }

        public ICollection<Heart> Hearts { get; set; }
    }
}
