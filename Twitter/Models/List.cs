using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class List
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [ForeignKey("Creator")]
        [Required]
        public int CreatorId { get; set; }

        public User Creator { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [Display(Name = "List Name")]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public string? CoverImage { get; set; } = "_DefaultListCover.png";

        public ICollection<ListMember> ListMembers { get; set; }

        public ICollection<ListFollower> ListFollowers { get; set; }
    }
}
