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
                Post post;
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
                await context.SaveChangesAsync();

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
                await context.SaveChangesAsync();

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
                await context.SaveChangesAsync();

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
                await context.SaveChangesAsync();

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
                await context.SaveChangesAsync();

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

                // Add Posts
                context.Posts.AddRange(
                new Post
                {
                    UserId = context.Users.FirstOrDefault(u => u.UserName == "TomCruise").Id,
                    Text = "36 years after the first film, #TopGun: Maverick is finally here. " +
                    "We made it for the big screen. And we made it for you, the fans. I hope you enjoy the ride this weekend."
                },
                new Post
                {
                    UserId = context.Users.FirstOrDefault(u => u.UserName == "TomCruise").Id,
                    Text = "This has been a long time coming. #TopGun",
                    Image = "TomCruisePostImage1.jpg"
                },
                new Post
                {
                    UserId = context.Users.FirstOrDefault(u => u.UserName == "CillianMurphy").Id,
                    Text = "What's your favourite Peaky Blinders scene?",
                    Image = "CillianMurphyPostImage1.jpg"
                },
                new Post
                {
                    UserId = context.Users.FirstOrDefault(u => u.UserName == "KevinRichardson").Id,
                    Text = "We're highlighting some pretty incredible moments over the last few years in today's video. " +
                    "Trust us, you don't want to miss it! CLICK HERE: https://youtu.be/NIIQfQ8nv7Y",
                    Image = "KevinRichardsonPostImage1.png"
                },
                new Post
                {
                    UserId = context.Users.FirstOrDefault(u => u.UserName == "KevinRichardson").Id,
                    Text = "Displaying Loving Bond With Hyenas",
                    Image = "KevinRichardsonPostImage2.jpg"
                },
                new Post
                {
                    UserId = context.Users.FirstOrDefault(u => u.UserName == "RobertDowneyJr").Id,
                    Text = "Hitting pen to paper today and making the planet greener.",
                    Video = "RobertDowneyJrPostVideo1.mp4"
                },
                new Post
                {
                    UserId = context.Users.FirstOrDefault(u => u.UserName == "RobertDowneyJr").Id,
                    Text = "",
                    Image = "RobertDowneyJrPostImage1.jpg"
                },
                new Post
                {
                    UserId = context.Users.FirstOrDefault(u => u.UserName == "JovaMusique").Id,
                    Text = "Guys! Heat Waves by Glass Animals (piano visualizer cover) link to watch " +
                    "full video: https://youtu.be/dHsQYTqKN4A hope u like the piano version! have a good day!",
                    Image = "JovaMusiquePostImage1.jpg"
                }
                );
                await context.SaveChangesAsync();

                // Add Lists
                context.Lists.Add(
                    new List
                    {
                        CreatorId = context.Users.FirstOrDefault(u => u.UserName == "JovaMusique").Id,
                        Name = "Oppenheimer 2023 Cast",
                        Description = "Overview of the Oppenheimer 2023 Cast",
                        CoverImage = "_DefaultListCover.png"
                    }
                    );
                await context.SaveChangesAsync();

                // Add List Followers and Members
                context.ListMember.AddRange(
                    new ListMember
                    {
                        ListId = context.Lists.FirstOrDefault(l => l.Name == "Oppenheimer 2023 Cast").Id,
                        MemberId = context.Users.FirstOrDefault(u => u.UserName == "CillianMurphy").Id
                    },
                    new ListMember
                    {
                        ListId = context.Lists.FirstOrDefault(l => l.Name == "Oppenheimer 2023 Cast").Id,
                        MemberId = context.Users.FirstOrDefault(u => u.UserName == "RobertDowneyJr").Id
                    }
                    );

                foreach (ListMember listMember in context.ListMember)
                {
                    context.Lists.FirstOrDefault(l => l.Id == listMember.ListId).MemberCount++;
                }

                await context.SaveChangesAsync();

                context.ListFollower.AddRange(
                    new ListFollower
                    {
                        ListId = context.Lists.FirstOrDefault(l => l.Name == "Oppenheimer 2023 Cast").Id,
                        FollowerId = context.Users.FirstOrDefault(u => u.UserName == "CillianMurphy").Id
                    },
                    new ListFollower
                    {
                        ListId = context.Lists.FirstOrDefault(l => l.Name == "Oppenheimer 2023 Cast").Id,
                        FollowerId = context.Users.FirstOrDefault(u => u.UserName == "RobertDowneyJr").Id
                    }
                    ,
                    new ListFollower
                    {
                        ListId = context.Lists.FirstOrDefault(l => l.Name == "Oppenheimer 2023 Cast").Id,
                        FollowerId = context.Users.FirstOrDefault(u => u.UserName == "JovaMusique").Id
                    }
                    ,
                    new ListFollower
                    {
                        ListId = context.Lists.FirstOrDefault(l => l.Name == "Oppenheimer 2023 Cast").Id,
                        FollowerId = context.Users.FirstOrDefault(u => u.UserName == "KevinRichardson").Id
                    }
                    ,
                    new ListFollower
                    {
                        ListId = context.Lists.FirstOrDefault(l => l.Name == "Oppenheimer 2023 Cast").Id,
                        FollowerId = context.Users.FirstOrDefault(u => u.UserName == "TomCruise").Id
                    }
                    );

                foreach (ListFollower listFollower in context.ListFollower)
                {
                    context.Lists.FirstOrDefault(l => l.Id == listFollower.ListId).FollowerCount++;
                }

                await context.SaveChangesAsync();

                // Add Followings
                context.Followings.AddRange(
                    new Following
                    {
                        FollowerId = 1,
                        FollowedUserId = 2
                    },
                    new Following
                    {
                        FollowerId = 1,
                        FollowedUserId = 3
                    },
                    new Following
                    {
                        FollowerId = 1,
                        FollowedUserId = 4
                    },
                    new Following
                    {
                        FollowerId = 1,
                        FollowedUserId = 5
                    },
                    new Following
                    {
                        FollowerId = 2,
                        FollowedUserId = 3
                    },
                    new Following
                    {
                        FollowerId = 2,
                        FollowedUserId = 4
                    },
                    new Following
                    {
                        FollowerId = 2,
                        FollowedUserId = 5
                    }
                    );
                await context.SaveChangesAsync();

                foreach (Following following in context.Followings)
                {
                    context.Users.FirstOrDefault(u => u.Id == following.FollowerId).FollowingCount++;
                    context.Users.FirstOrDefault(u => u.Id == following.FollowedUserId).FollowerCount++;
                }

                await context.SaveChangesAsync();

                // Add Replies
                context.Replies.AddRange(
                    new Reply
                    {
                        PostId = 3,
                        UserId = 1,
                        Text = "For me it's the one where Tommy is about to be executed by the Red Right Hand"
                    },
                    new Reply
                    {
                        PostId = 3,
                        UserId = 4,
                        Text = "When Alfie gets \"killed\" on the beach"
                    }
                    );

                foreach (var reply in context.Replies)
                {
                    context.Posts.FirstOrDefault(p => p.Id == reply.PostId).ReplyCount++;
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
