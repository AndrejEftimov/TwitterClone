using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class Heart
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [ForeignKey("Post")]
        public int? PostId { get; set; }

        public Post Post { get; set; }

        [ForeignKey("Reply")]
        public int? ReplyId { get; set; }

        public Reply Reply { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
