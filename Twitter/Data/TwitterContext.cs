using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Twitter.Areas.Identity.Data;
using Twitter.Models;

namespace Twitter.Data
{
    public class TwitterContext : IdentityDbContext<TwitterUser>
    {
        public TwitterContext(DbContextOptions<TwitterContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Heart> Hearts { get; set; }
        public DbSet<Following> Followings { get; set; }
        public DbSet<List> Lists { get; set; }
        public DbSet<ListMember> ListMember { get; set; }
        public DbSet<ListFollower> ListFollower { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasOne<User>(p => p.User)
                .WithMany(p => p.Posts).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Reply>()
                .HasOne<Post>(p => p.Post)
                .WithMany(p => p.Replies).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.PostId);

            modelBuilder.Entity<Reply>()
                .HasOne<User>(p => p.User)
                .WithMany(p => p.Replies).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Heart>()
                .HasOne<Post>(p => p.Post)
                .WithMany(p => p.Hearts).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.PostId);

            modelBuilder.Entity<Heart>()
                .HasOne<Reply>(h => h.Reply)
                .WithMany(r => r.Hearts).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(h => h.ReplyId);

            modelBuilder.Entity<Heart>()
                .HasOne<User>(p => p.User)
                .WithMany(p => p.Hearts).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Following>()
                .HasOne<User>(p => p.FollowedUser)
                .WithMany(p => p.Followers).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.FollowedUserId);

            modelBuilder.Entity<Following>()
                .HasOne<User>(p => p.Follower)
                .WithMany(p => p.Followed).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.FollowerId);

            modelBuilder.Entity<List>()
                .HasOne<User>(p => p.Creator)
                .WithMany(p => p.ListsCreated).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.CreatorId);

            modelBuilder.Entity<ListMember>()
                .HasOne<List>(p => p.List)
                .WithMany(p => p.ListMembers).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.ListId);

            modelBuilder.Entity<ListMember>()
                .HasOne<User>(p => p.Member)
                .WithMany(p => p.MemberOf).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.MemberId);

            modelBuilder.Entity<ListFollower>()
                .HasOne<List>(p => p.List)
                .WithMany(p => p.ListFollowers).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.ListId);

            modelBuilder.Entity<ListFollower>()
                .HasOne<User>(p => p.Follower)
                .WithMany(p => p.ListsFollowing).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(p => p.FollowerId);

            //add indexing
            modelBuilder.Entity<Post>()
                .HasIndex(p => p.Id)
                .IsUnique()
                .IncludeProperties(p => new { p.UserId, p.DateCreated }); //included columns
        }
    }
}
