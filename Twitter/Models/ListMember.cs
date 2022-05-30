using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class ListMember // Define composite primary key with HasKey(PostId, UserId) in DbContext
    {                       // ako ne funkcionira so composite key probaj so unique auto-incremented key
        [ForeignKey("List")]
        [Required]
        public int ListId { get; set; }

        public List List { get; set; }

        [ForeignKey("User")]
        [Required]
        public int MemberId { get; set; }

        public User Member { get; set; }
    }
}
