using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Display(Name = "Follower Count")]
        public int FollowerCount { get; set; } = 0;

        [Display(Name = "Users Following")]
        public int FollowingCount { get; set; } = 0;

        public string ProfileImage { get; set; } = "_DefaultProfile.png";

        public string CoverImage { get; set; } = "_DefaultCover.png";

        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; } = DateTime.Now.Date;

        public ICollection<Post>? Posts { get; set; }

        public ICollection<Following>? Followers { get; set; }

        public ICollection<Following>? Followed { get; set; }

        public ICollection<Reply>? Replies { get; set; }

        public ICollection<Heart>? Hearts { get; set; }

        public ICollection<List>? ListsCreated { get; set; }

        public ICollection<ListFollower>? ListsFollowing { get; set; }

        public ICollection<ListMember>? MemberOf { get; set; } // not used/needed in application
    }
}
