using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Twitter.Models
{
    public class ListFollower
    {
        [Key]
        [Required]
        public int Id { get; set; }

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
