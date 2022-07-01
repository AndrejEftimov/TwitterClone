using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class Reply
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [ForeignKey("Post")]
        [Required]
        public int PostId { get; set; }

        public Post? Post { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }

        public User? User { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Text { get; set; }

        [Required]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public int HeartCount { get; set; } = 0;

        public ICollection<Heart>? Hearts { get; set; }
    }
}
