using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class Following  // Define composite primary key with HasKey(UserId, FollowerId) in DbContext
    {                       // ako ne funkcionira so composite key probaj so unique auto-incremented key
        [ForeignKey("FollowedUser")]
        [Required]
        public int FollowedUserId { get; set; }

        public User FollowedUser { get; set; }

        [ForeignKey("Follower")]
        [Required]
        public int FollowerId { get; set; }

        public User Follower { get; set; }
    }
}
