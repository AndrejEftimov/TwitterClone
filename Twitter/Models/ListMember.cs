using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class ListMember
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [ForeignKey("List")]
        [Required]
        public int ListId { get; set; }

        public List? List { get; set; }

        [ForeignKey("Member")]
        [Required]
        public int MemberId { get; set; }

        public User? Member { get; set; }
    }
}
