namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            if (!context.Roles.Any(r => r.Name == "Demo User"))
            {
                roleManager.Create(new IdentityRole { Name = "Demo User" });
            }

            ApplicationUser adminUser = new ApplicationUser();

            if (!context.Users.Any(item => item.UserName == "admin@admin.com"))
            {
                adminUser.UserName = "admin@admin.com";
                adminUser.Email = "admin@admin.com";
                adminUser.LastName = "Admin";
                adminUser.FirstName = "Zerg";
                adminUser.DisplayName = "z3rg";


                userManager.Create(adminUser, "admin@admin.com");
            }
            else
            {
                adminUser = context.Users.FirstOrDefault(item => item.UserName == "admin@admin.com");
            }

            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            //seeding types table
            if(!context.ProjectTypes.Any())
            {
                context.ProjectTypes.AddOrUpdate(item => item.Id,
                    new ProjectType() { Type = "Angular" },
                    new ProjectType() { Type = "Vuejs" },
                    new ProjectType() { Type = "React" },
                    new ProjectType() { Type = "ASP.NET" },
                    new ProjectType() { Type = "Nodejs" }
                    );
            }

        }
    }
}
