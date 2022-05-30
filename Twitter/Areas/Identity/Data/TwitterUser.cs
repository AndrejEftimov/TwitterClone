using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Twitter.Models;

namespace Twitter.Areas.Identity.Data;

// Add profile data for application users by adding properties to the TwitterUser class
public class TwitterUser : IdentityUser
{
    [ForeignKey("User")]
    public int? UserId { get; set; }

    public User? User { get; set; }

    public string? Role { get; set; }
}

