using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Twitter.Models
{
    // Define composite primary key with HasKey(PostId, UserId) in DbContext
    // ako ne funkcionira so composite key probaj so unique auto-incremented key
    public class ListFollower
    {
        [Required]
        [ForeignKey("List")]
        public int ListId { get; set; }

        public List List { get; set; }

        [Required]
        [ForeignKey("Follower")]
        public int FollowerId { get; set; }

        public User Follower { get; set; }
    }
}
