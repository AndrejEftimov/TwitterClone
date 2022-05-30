using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics;
using Twitter.Areas.Identity.Data;
using Twitter.Models;

namespace Twitter.Data
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<TwitterUser>>();
            IdentityResult roleResult;
            //Add Roles
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            roleCheck = await RoleManager.RoleExistsAsync("User");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("User")); }

            TwitterUser user = await UserManager.FindByEmailAsync("admin@twitter.com");
            if (user == null)
            {
                var User = new TwitterUser();
                User.Email = "admin@twitter.com";
                User.UserName = "TwitterAdmin";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<TwitterUser>>();

            using (var context = new TwitterContext(
            serviceProvider.GetRequiredService<
            DbContextOptions<TwitterContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Users.Any() || context.Replies.Any() || context.Posts.Any() || context.ListMember.Any() ||
                    context.ListFollower.Any() || context.Lists.Any() || context.Hearts.Any() || context.Followings.Any())
                {
                    return; // DB contains data
                }

                TwitterUser twitterUser;
                User user;
                string pass;
                IdentityResult result;

                // Add User
                user = new User()
                {
                    UserName = "TomCruise",
                    DisplayName = "Tom Cruise",
                    Description = "Actor. Producer. Running in movies since 1981.",
                    ProfileImage = "TomCruiseProfile.jpg",
                    CoverImage = "TomCruiseCover.jpg"
                };

                context.Users.Add(user);
                context.SaveChanges();

                twitterUser = new TwitterUser()
                {
                    Email = user.UserName + "@twitter.com",
                    UserName = user.UserName,
                    UserId = user.Id,
                    Role = "User"
                };

                pass = user.UserName + "123";

                result = await UserManager.CreateAsync(twitterUser, pass);
                if (result.Succeeded) { await UserManager.AddToRoleAsync(twitterUser, "User"); }

                // Add User
                user = new User()
                {
                    UserName = "CillianMurphy",
                    DisplayName = "Cillian Murphy",
                    Description = "Currently filming #Oppenheimer.",
                    ProfileImage = "CillianMurphyProfile.jpeg",
                    CoverImage = "CillianMurphyCover.jpg"
                };

                context.Users.Add(user);
                context.SaveChanges();

                twitterUser = new TwitterUser()
                {
                    Email = user.UserName + "@twitter.com",
                    UserName = user.UserName,
                    UserId = user.Id,
                    Role = "User"
                };

                pass = user.UserName + "123";

                result = await UserManager.CreateAsync(twitterUser, pass);
                if (result.Succeeded) { await UserManager.AddToRoleAsync(twitterUser, "User"); }

                // Add User
                user = new User()
                {
                    UserName = "KevinRichardson",
                    DisplayName = "Kevin Richardson",
                    Description = "This channel aims to raise awareness about The Kevin Richardson Wildlife Sanctuary" +
                    " and its mission to provide a self-sustaining African carnivore sanctuary" +
                    " for the purposes of wild species preservation, education, awareness and funding.",
                    ProfileImage = "KevinRichardsonProfile.jpg",
                    CoverImage = "KevinRichardsonCover.jpg"
                };

                context.Users.Add(user);
                context.SaveChanges();

                twitterUser = new TwitterUser()
                {
                    Email = user.UserName + "@twitter.com",
                    UserName = user.UserName,
                    UserId = user.Id,
                    Role = "User"
                };

                pass = user.UserName + "123";

                result = await UserManager.CreateAsync(twitterUser, pass);
                if (result.Succeeded) { await UserManager.AddToRoleAsync(twitterUser, "User"); }

                // Add User
                user = new User()
                {
                    UserName = "RobertDowneyJr",
                    DisplayName = "Robert Downey Jr",
                    Description = "You know who I am.",
                    ProfileImage = "RobertDowneyJrProfile.jpg",
                    CoverImage = "RobertDowneyJrCover.jpg"
                };

                context.Users.Add(user);
                context.SaveChanges();

                twitterUser = new TwitterUser()
                {
                    Email = user.UserName + "@twitter.com",
                    UserName = user.UserName,
                    UserId = user.Id,
                    Role = "User"
                };

                pass = user.UserName + "123";

                result = await UserManager.CreateAsync(twitterUser, pass);
                if (result.Succeeded) { await UserManager.AddToRoleAsync(twitterUser, "User"); }

                // Add User
                user = new User()
                {
                    UserName = "JovaMusique",
                    DisplayName = "Jova Musique - Pianella",
                    Description = "Piano Visualizer Cover, Piano Videos (Mashup, 3-7 hands piano, Piano Beat)",
                    ProfileImage = "JovaMusiqueProfile.jpg",
                    CoverImage = "JovaMusiqueCover.jpg"
                };

                context.Users.Add(user);
                context.SaveChanges();

                twitterUser = new TwitterUser()
                {
                    Email = user.UserName + "@twitter.com",
                    UserName = user.UserName,
                    UserId = user.Id,
                    Role = "User"
                };

                pass = user.UserName + "123";

                result = await UserManager.CreateAsync(twitterUser, pass);
                if (result.Succeeded) { await UserManager.AddToRoleAsync(twitterUser, "User"); }

                // Add Post
                // ...
            }
        }
    }
}
