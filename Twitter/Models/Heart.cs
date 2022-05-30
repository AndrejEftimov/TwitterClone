using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class Heart  // Define composite primary key with HasKey(PostId, UserId) in DbContext
    {                   // ako ne funkcionira so composite key probaj so unique auto-incremented key
        [ForeignKey("Post")]
        [Required]
        public int PostId { get; set; }

        public Post Post { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
