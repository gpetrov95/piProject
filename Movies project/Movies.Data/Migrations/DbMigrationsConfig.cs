namespace Movies.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public sealed class DbMigrationsConfig : DbMigrationsConfiguration<Movies.Data.ApplicationDbContext>
    {
        public DbMigrationsConfig()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            //ContextKey = "Movies.Data.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            // Seed initial data only if the database is empty
            if (!context.Users.Any())
            {
                var adminEmail = "georgi@petrov.com";
                var adminUserName = adminEmail;
                var adminFullName = "Georgi Petrov";
                var adminPassword = "gogo95";
                string adminRole = "Administrator";
                CreateAdminUser(context, adminEmail, adminUserName, adminFullName, adminPassword, adminRole);
                CreateSeveralEvents(context);
            }
        }

        private void CreateAdminUser(ApplicationDbContext context, string adminEmail, string adminUserName, string adminFullName, string adminPassword, string adminRole)
        {
            // Create the "admin" user
            var adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                FullName = adminFullName,
                Email = adminEmail
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var userCreateResult = userManager.Create(adminUser, adminPassword);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            // Create the "Administrator" role
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roleCreateResult = roleManager.Create(new IdentityRole(adminRole));
            if (!roleCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", roleCreateResult.Errors));
            }

            // Add the "admin" user to "Administrator" role
            var addAdminRoleResult = userManager.AddToRole(adminUser.Id, adminRole);
            if (!addAdminRoleResult.Succeeded)
            {
                throw new Exception(string.Join("; ", addAdminRoleResult.Errors));
            }
        }

        private void CreateSeveralEvents(ApplicationDbContext context)
        {
            context.Movies.Add(new Movie()
            {
                Title = "Fast and furious",
                StartDateTime = DateTime.Now.Date.AddDays(5).AddHours(21).AddMinutes(30)
            });

            context.Movies.Add(new Movie()
            {
                Title = "Rambo",
                StartDateTime = DateTime.Now.Date.AddDays(7).AddHours(23).AddMinutes(00),
                Comments = new HashSet<Comment>() {
                    new Comment() { Text = "User comment", Author = context.Users.First() }
                }
            });

            context.Movies.Add(new Movie()
            {
                Title = "Terminator",
                StartDateTime = DateTime.Now.Date.AddDays(8).AddHours(22).AddMinutes(15)
            });

            context.Movies.Add(new Movie()
            {
                Title = "Iron man",
                StartDateTime = DateTime.Now.Date.AddDays(-2).AddHours(10).AddMinutes(30),
                Duration = TimeSpan.FromHours(1.5),
                Comments = new HashSet<Comment>() {
                    new Comment() { Text = "that movie sucks" },

                }
            });



            context.SaveChanges();
        }
    }
}


